using System.IO;
using System.Reflection;
using System.Xml;
using Autofac.Extensions.DependencyInjection;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApeVolo.Api;

public class Program
{
    public static void Main(string[] args)
    {
        XmlDocument log4NetConfig = new XmlDocument();
        log4NetConfig.Load(File.OpenRead("Log4net.config"));

        var repo = LogManager.CreateRepository(
            Assembly.GetEntryAssembly(), typeof(Hierarchy));

        XmlConfigurator.Configure(repo, log4NetConfig["log4net"]);

        var host = CreateHostBuilder(args)
            .ConfigureAppConfiguration(r => r.AddJsonFile("IpRateLimit.json"))
            .Build();
        host.Run();
    }


    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .ConfigureKestrel(serverOptions => { serverOptions.AllowSynchronousIO = true; })
                    .UseStartup<Startup>()
                    .UseUrls("http://*:8002")
                    .ConfigureLogging((hostingContext, builder) =>
                    {
                        builder.ClearProviders();
                        builder.SetMinimumLevel(LogLevel.Trace);
                        builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        builder.AddConsole();
                        builder.AddDebug();
                    });
            });
}