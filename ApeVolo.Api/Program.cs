using Autofac.Extensions.DependencyInjection;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using ApeVolo.Entity.Seed;

namespace ApeVolo.Api
{
    public class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        public static void Main(string[] args)
        {
            XmlDocument log4NetConfig = new XmlDocument();
            log4NetConfig.Load(File.OpenRead("Log4net.config"));

            var repo = log4net.LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

            log4net.Config.XmlConfigurator.Configure(repo, log4NetConfig["log4net"]);

            var host = CreateHostBuilder(args)
                .ConfigureAppConfiguration(r => r.AddJsonFile("IpRateLimit.json"))
                .Build();

            //Microsoft.Extensions.DependencyInjection.IServiceScope
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();

                try
                {
                    var configuration = services.GetRequiredService<IConfiguration>();
                    if (true)
                    {
                        var myContext = services.GetRequiredService<MyContext>();
                        var env = services.GetRequiredService<IWebHostEnvironment>();
                        SeedData.InitSystemDataAsync(myContext, env.ContentRootPath).Wait();
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Error occured seeding the Database.\n{e.Message}");
                    throw;
                }
            }

            host.Run();
        }


        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory()) //<--NOTE THIS
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
}