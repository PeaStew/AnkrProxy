using System.Collections.ObjectModel;
using System.Net;using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);
var transform = new Dictionary<string, string>();

var routes = new[]
{
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(), // Forces a new route id each time GetRoutes is called.
        ClusterId = "tron-mainnet-archive-rpc",
        Match = new RouteMatch
        {
            Path = "/tron-mainnet-archive-rpc/wallet/{**catch-all}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/tron-mainnet-archive-rpc"),
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(), // Forces a new route id each time GetRoutes is called.
        ClusterId = "tron-mainnet-archive-solidity-rpc",
        Match = new RouteMatch
        {
            // Path or Hosts are required for each route. This catch-all pattern matches all request paths.
            Path = "/tron-mainnet-archive-solidity-rpc/walletsolidity/{**catch-all}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/tron-mainnet-archive-solidity-rpc"),
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(), // Forces a new route id each time GetRoutes is called.
        ClusterId = "bttc-mainnet-archive-rpc",
        Match = new RouteMatch
        {
            // Path or Hosts are required for each route. This catch-all pattern matches all request paths.
            Path = "/bttc-mainnet-archive-rpc/{**catch-all}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/bttc-mainnet-archive-rpc")
};

var clusters = new[]
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
    },
    new ClusterConfig()
    {
        ClusterId = "bttc-mainnet-archive-rpc",
        SessionAffinity = new SessionAffinityConfig { Enabled = true, Policy = "Cookie", AffinityKeyName = ".Yarp.ReverseProxy.Affinity" },
        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:22511/" } }
        }
    }

};

builder.Services.AddReverseProxy()
    .LoadFromMemory(routes, clusters);

builder.WebHost.UseKestrel(so =>
{
    so.Limits.MaxConcurrentConnections = 100000;
    so.Limits.MaxConcurrentUpgradedConnections = 100000;
    so.Limits.MaxRequestBodySize = 252428800;
    var kestrelSection = builder.Configuration.GetSection("Kestrel");
    so.Configure(kestrelSection);

});

var app = builder.Build();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.Map("/update", context =>
    {
        context.RequestServices.GetRequiredService<InMemoryConfigProvider>().Update(routes, clusters);
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
//app.MapReverseProxy();

app.Run();