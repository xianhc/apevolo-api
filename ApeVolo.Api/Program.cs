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
        //这里注意下，固定读取appsettings.json了
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var host = CreateHostBuilder(args, configuration)
            .ConfigureAppConfiguration(r => r.AddJsonFile("IpRateLimit.json"))
            .Build();
        host.Run();
    }


    private static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilogMiddleware(configuration)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseStartup<Startup>()
                    .UseUrls("http://*:8002");
            });
}
