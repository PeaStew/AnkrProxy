using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Model;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

var routes = new[]
{
    //Tron
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(),
        ClusterId = "aptos_aptos_mainnet_full_http_1",
        Match = new RouteMatch
        {
            Path = "/aptos_aptos_mainnet_full_http_1/{*any}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/aptos_aptos_mainnet_full_http_1"),
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(),
        ClusterId = "aptos_aptos_mainnet_full_http_2",
        Match = new RouteMatch
        {
            Path = "/aptos_aptos_mainnet_full_http_2/{*any}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/aptos_aptos_mainnet_full_http_2"),
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(),
        ClusterId = "aptos_aptos_mainnet_full_http_3",
        Match = new RouteMatch
        {
            Path = "/aptos_aptos_mainnet_full_http_3/{*any}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/aptos_aptos_mainnet_full_http_3")

};

var clusters = new[]
{
    new ClusterConfig()
    {
        ClusterId = "aptos_aptos_mainnet_full_http_1",

        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:26711/" } },
        }
    },
    new ClusterConfig()
    {
        ClusterId = "aptos_aptos_mainnet_full_http_2",

        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:26721/" } },
        }
    },
    new ClusterConfig()
    {
        ClusterId = "aptos_aptos_mainnet_full_http_3",

        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:26731/" } },
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

Console.WriteLine(JsonSerializer.Serialize(routes));
Console.WriteLine(JsonSerializer.Serialize(clusters));

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
