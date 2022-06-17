using System.Net;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);
var certPath = builder.Configuration.GetSection("Kestrel").GetSection("Endpoints").GetSection("HttpsInlineCertFile").GetSection("Certificate").GetValue<string>("Path");
var certKey = builder.Configuration.GetSection("Kestrel").GetSection("Endpoints").GetSection("HttpsInlineCertFile").GetSection("Certificate").GetValue<string>("KeyPath");

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.WebHost.UseKestrel(so =>
{
    so.Limits.MaxConcurrentConnections = 100000;
    so.Limits.MaxConcurrentUpgradedConnections = 100000;
    so.Limits.MaxRequestBodySize = 252428800;
    
    so.Listen(IPAddress.Loopback, 443,
        listenOptions =>
        {
            listenOptions.UseHttps(certPath,
                certKey);
        });

});

var routes = new[]
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
    }
};

var app = builder.Build();

app.MapReverseProxy();
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
        proxyPipeline.UseSessionAffinity();
        proxyPipeline.UseLoadBalancing();
    });
});
app.UseHttpsRedirection();
app.Run();

