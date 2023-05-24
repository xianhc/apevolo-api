using ApeVolo.Api.Middleware;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ApeVolo.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args)
            .ConfigureAppConfiguration(r => r.AddJsonFile("IpRateLimit.json"))
            .Build();
        host.Run();
    }


    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilogMiddleware()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseStartup<Startup>()
                    .UseUrls("http://*:8002");
            });
}