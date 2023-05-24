using System;
using ApeVolo.Common.DI;
using ApeVolo.Common.Global;
using ApeVolo.Common.WebApp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace ApeVolo.Api.Middleware;

public static class SerilogMiddleware
{
    private static readonly string SerilogPath = Environment.CurrentDirectory + "\\Logs\\{0}\\.log";

    private const string SerilogOutputTemplate =
        "{NewLine}时间:{Timestamp:yyyy-MM-dd HH:mm:ss.fff}{NewLine}所在类:{SourceContext}{NewLine}等级:{Level}{NewLine}信息:{Message}{NewLine}{Exception}";

    public static IHostBuilder UseSerilogMiddleware(this IHostBuilder builder)
    {
        return builder.UseSerilog((context, logger) => //注册Serilog
        {
            //如要想使用setting配置方式，打开下面行注释。注释后面代码即可
            //logger.ReadFrom.Configuration(context.Configuration);
            logger.Enrich.FromLogContext();
            //logger.Enrich.WithProperty("UserName", "system");
            logger.MinimumLevel.Debug(); //日志记录起始等级
            logger.MinimumLevel.Override("Microsoft", LogEventLevel.Information);
            logger.MinimumLevel.Override("Default", LogEventLevel.Information);
            logger.MinimumLevel.Override("System", LogEventLevel.Information);
            if (AppSettings.IsDevelopment)
            {
                //开发环境下输出日志到控制台
                logger.WriteTo.Console();
            }

            logger.WriteTo.Logger(lg =>
                    lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Debug).WriteTo.Async(a =>
                        a.File(string.Format(SerilogPath, LogEventLevel.Debug.ToString()),
                            rollingInterval: RollingInterval.Day,
                            outputTemplate: SerilogOutputTemplate)))
                .WriteTo.Logger(lg =>
                    lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Information).WriteTo.Async(a =>
                        a.File(string.Format(SerilogPath, LogEventLevel.Information.ToString()),
                            rollingInterval: RollingInterval.Day,
                            outputTemplate: SerilogOutputTemplate)))
                .WriteTo.Logger(lg =>
                    lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Warning).WriteTo.Async(a =>
                        a.File(string.Format(SerilogPath, LogEventLevel.Warning.ToString()),
                            rollingInterval: RollingInterval.Day,
                            outputTemplate: SerilogOutputTemplate)))
                .WriteTo.Logger(lg =>
                    lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Error).WriteTo.Async(a =>
                        a.File(string.Format(SerilogPath, LogEventLevel.Error.ToString()),
                            rollingInterval: RollingInterval.Day,
                            outputTemplate: SerilogOutputTemplate)))
                .WriteTo.Logger(lg =>
                    lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Fatal).WriteTo.Async(a =>
                        a.File(string.Format(SerilogPath, LogEventLevel.Fatal.ToString()),
                            rollingInterval: RollingInterval.Day,
                            outputTemplate: SerilogOutputTemplate)));
            //需要配置elasticsearch环境使用
            //docker run --name elasticsearch -d -e ES_JAVA_OPTS="-Xms512m -Xmx512m" -e "discovery.type=single-node" -p 9200:9200 -p 9300:9300 elasticsearch:7.5.0
            if (AppSettings.GetValue<bool>("Middleware", "Elasticsearch", "Enabled"))
            {
                logger.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://127.0.0.1:9200/"))
                {
                    AutoRegisterTemplate = true
                });
            }
        });
    }
}