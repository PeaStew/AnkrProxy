
using System.Text.Json;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


var routes = new[]
{
    //Tron
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(),
        ClusterId = "tron_mainnet_archive_rpc",
        Match = new RouteMatch
        {
            Path = "/tron_mainnet_archive_rpc/wallet/{*any}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/tron_mainnet_archive_rpc"),
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(),
        ClusterId = "tron_mainnet_archive_solidity_rpc",
        Match = new RouteMatch
        {
            Path = "/tron_mainnet_archive_rpc/walletsolidity/{*any}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/tron_mainnet_archive_rpc"),
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(),
        ClusterId = "tron_mainnet_archive_jsonrpc",
        Match = new RouteMatch
        {
            Path = "/tron_mainnet_archive_jsonrpc/{*any}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/tron_mainnet_archive_rpc"),
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(),
        ClusterId = "tron_mainnet_archive_solidity_jsonrpc",
        Match = new RouteMatch
        {
            Path = "/tron_mainnet_archive_solidity_jsonrpc/{*any}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/tron_mainnet_archive_rpc"),
    //BTTC FULL
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(),
        ClusterId = "bttc_mainnet_full_rpc",
        Match = new RouteMatch
        {

            Path = "/bttc_mainnet_full_rpc/{*any}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/bttc_mainnet_full_rpc"),
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(),
        ClusterId = "bttc_mainnet_full_ws",
        Match = new RouteMatch
        {

            Path = "/bttc_mainnet_full_ws/{*any}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/bttc_mainnet_full_ws"),
    //BTTC ARCHIVE
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(),
        ClusterId = "bttc_mainnet_archive_rpc",
        Match = new RouteMatch
        {

            Path = "/bttc_mainnet_archive_rpc/{*any}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/bttc_mainnet_archive_rpc"),
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(),
        ClusterId = "bttc_mainnet_archive_ws",
        Match = new RouteMatch
        {

            Path = "/bttc_mainnet_archive_ws/{*any}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/bttc_mainnet_archive_ws"),
    //Optimism
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(),
        ClusterId = "optimism_mainnet_archive_rpc_1",
        Match = new RouteMatch
        {

            Path = "/optimism_mainnet_archive_rpc_1/{*any}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/optimism_mainnet_archive_rpc_1"),
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(),
        ClusterId = "optimism_mainnet_archive_ws_1",
        Match = new RouteMatch
        {

            Path = "/optimism_mainnet_archive_ws_1/{*any}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/optimism_mainnet_archive_ws_1"),
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(),
        ClusterId = "optimism_mainnet_archive_rpc_2",
        Match = new RouteMatch
        {

            Path = "/optimism_mainnet_archive_rpc_2/{*any}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/optimism_mainnet_archive_rpc_2"),
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(),
        ClusterId = "optimism_mainnet_archive_ws_2",
        Match = new RouteMatch
        {

            Path = "/optimism_mainnet_archive_ws_2/{*any}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/optimism_mainnet_archive_ws_2"),
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(),
        ClusterId = "optimism_mainnet_archive_rpc_3",
        Match = new RouteMatch
        {

            Path = "/optimism_mainnet_archive_rpc_3/{*any}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/optimism_mainnet_archive_rpc_3"),
    new RouteConfig()
    {
        RouteId = "route" + Random.Shared.Next(),
        ClusterId = "optimism_mainnet_archive_ws_3",
        Match = new RouteMatch
        {

            Path = "/optimism_mainnet_archive_ws_3/{*any}"
        }
    }.WithTransformPathRemovePrefix(prefix: "/optimism_mainnet_archive_ws_3")
};

var clusters = new[]
{
    new ClusterConfig()
    {
        ClusterId = "tron_mainnet_archive_rpc",

        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:22311/" } },
        }
    },
    new ClusterConfig()
    {
        ClusterId = "tron_mainnet_archive_grpc",

        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:22311/" } },
        }
    },
    new ClusterConfig()
    {
        ClusterId = "tron_mainnet_archive_solidity_rpc",

        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:22312/" } }
        }
    },
    new ClusterConfig()
    {
        ClusterId = "tron_mainnet_archive_jsonrpc",

        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:22315/" } },
        }
    },
    new ClusterConfig()
    {
        ClusterId = "tron_mainnet_archive_solidity_jsonrpc",

        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:22316/" } }
        }
    },
    new ClusterConfig()
    {
        ClusterId = "bttc_mainnet_full_rpc",

        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:22511/" } }
        }
    },
    new ClusterConfig()
    {
        ClusterId = "bttc_mainnet_full_ws",

        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:22512/" } }
        }
    },
    new ClusterConfig()
    {
        ClusterId = "bttc_mainnet_archive_rpc",

        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:32511/" } }
        }
    },
    new ClusterConfig()
    {
        ClusterId = "bttc_mainnet_archive_ws",

        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:32512/" } }
        }
    },
    new ClusterConfig()
    {
        ClusterId = "optimism_mainnet_archive_rpc_1",

        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:22211/" } }
        }
    },
    new ClusterConfig()
    {
        ClusterId = "optimism_mainnet_archive_ws_1",

        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:22212/" } }
        }
    },
    new ClusterConfig()
    {
        ClusterId = "optimism_mainnet_archive_rpc_2",

        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:22221/" } }
        }
    },
    new ClusterConfig()
    {
        ClusterId = "optimism_mainnet_archive_ws_2",

        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:22222/" } }
        }
    },
    new ClusterConfig()
    {
        ClusterId = "optimism_mainnet_archive_rpc_3",

        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:22231/" } }
        }
    },
    new ClusterConfig()
    {
        ClusterId = "optimism_mainnet_archive_ws_3",

        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "node1", new DestinationConfig() { Address = "http://localhost:22232/" } }
        }
    }

};

builder.Services.AddReverseProxy().LoadFromMemory(routes, clusters);

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

public static class GetRouteConfiguration
{
    public static RouteConfig[] GetRoutes(ConfigurationSection config)
    {
        if (config.Value == null) throw new Exception("No NodeConfig found");

        var ret = new List<RouteConfig>();

        var nodeConfigs = JsonSerializer.Deserialize<NodeConfig[]>(config.Value);

        foreach (var nodeConfig in nodeConfigs)
        {
            var routeConfig = new RouteConfig
            {

                RouteId = "route" + Random.Shared.Next(),
                ClusterId = nodeConfig.ClusterId,
                Match = new RouteMatch
                {
                    Path = $"{nodeConfig.NodePath}{nodeConfig.NodePathExtension}" + "/{*any}"
                }
            }.WithTransformPathRemovePrefix(prefix: $"{nodeConfig.NodePath}");


            ret.Add(routeConfig);
        }
        return ret.ToArray();
    }

}


public class NodeConfig
{
    public string ClusterId { get; set; }
    public string NodePath { get; set; }
    public string NodePathExtension { get; set; }
    public NodeDetail NodeDetail { get; set; }
    
}

public class NodeDetail
{
    public string RPCAddress { get; set; }
    public string WSAddress { get; set; }
}


