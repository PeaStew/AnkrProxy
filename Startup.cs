// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Model;

namespace AnkrProxy
{
    /// <summary>
    /// Initialiaztion for ASP.NET using YARP reverse proxy
    /// </summary>
    public class Startup
    {
        //private const string DEBUG_HEADER = "Debug";
        //private const string DEBUG_METADATA_KEY = "debug";
        //private const string DEBUG_VALUE = "true";

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // Programatically creating route and cluster configs. This allows loading the data from an arbitrary source.
            services.AddReverseProxy()
                .LoadFromMemory(GetRoutes(), GetClusters());
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.Map("/update", context =>
                {
                    context.RequestServices.GetRequiredService<InMemoryConfigProvider>().Update(GetRoutes(), GetClusters());
                    return Task.CompletedTask;
                });
                // We can customize the proxy pipeline and add/remove/replace steps
                endpoints.MapReverseProxy(proxyPipeline =>
                {
                    // Use a custom proxy middleware, defined below
                    //proxyPipeline.Use(MyCustomProxyStep);
                    // Don't forget to include these two middleware when you make a custom proxy pipeline (if you need them).
                    //proxyPipeline.UseSessionAffinity();
                    proxyPipeline.UseLoadBalancing();
                });
            });
            app.UseHttpsRedirection();
        }

        private RouteConfig[] GetRoutes()
        {
            return new[]
            {
                new RouteConfig()
                {
                    RouteId = "route" + Random.Shared.Next(), // Forces a new route id each time GetRoutes is called.
                    ClusterId = "tron-mainnet-archive-rpc",
                    Match = new RouteMatch
                    {
                        Path = "/wallet/{**catch-all}"
                    }
                },
                new RouteConfig()
                {
                    RouteId = "route" + Random.Shared.Next(), // Forces a new route id each time GetRoutes is called.
                    ClusterId = "tron-mainnet-archive-solidity-rpc",
                    Match = new RouteMatch
                    {
                        // Path or Hosts are required for each route. This catch-all pattern matches all request paths.
                        Path = "/walletsolidity/{**catch-all}"
                    }
                }
            };
        }
        private ClusterConfig[] GetClusters()
        {
            return new[]
            {
                new ClusterConfig()
                {
                    ClusterId = "tron-mainnet-archive-rpc",
                    SessionAffinity = new SessionAffinityConfig { Enabled = true, Policy = "Cookie", AffinityKeyName = ".Yarp.ReverseProxy.Affinity" },
                    Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "node1", new DestinationConfig() { Address = "http://localhost:22311/" } },
                    }
                },
                new ClusterConfig()
                {
                    ClusterId = "tron-mainnet-archive-solidity-rpc",
                    SessionAffinity = new SessionAffinityConfig { Enabled = true, Policy = "Cookie", AffinityKeyName = ".Yarp.ReverseProxy.Affinity" },
                    Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "node1", new DestinationConfig() { Address = "http://localhost:22312/" } }
                    }
                }
            };
        }
    }
}
