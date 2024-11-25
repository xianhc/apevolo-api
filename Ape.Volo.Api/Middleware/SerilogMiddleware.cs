using System;
using Ape.Volo.Api.Serilog;
using Ape.Volo.Common;
using Ape.Volo.Common.ConfigOptions;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Ape.Volo.Api.Middleware;

/// <summary>
/// Serilog 处理中间件
/// </summary>
public static class SerilogMiddleware
{
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


            var serilogOptions = App.GetOptions<SerilogOptions>();
            if (serilogOptions.ToConsole.Enabled)
            {
                foreach (LogEventLevel logEvent in Enum.GetValues(typeof(LogEventLevel)))
                {
                    //输出到文件
                    logger.WriteToFile(logEvent);
                }
            }

            if (App.GetOptions<SystemOptions>().IsQuickDebug && serilogOptions.ToConsole.Enabled)
            {
                //输出到控制台
                logger.WriteToConsole();
            }


            if (serilogOptions.ToDb.Enabled)
            {
                //输出到数据库
                logger.WriteToDb();
            }

            if (serilogOptions.ToElasticsearch.Enabled && App.GetOptions<MiddlewareOptions>().Elasticsearch.Enabled)
            {
                //输出到Elasticsearch
                //需要配置elasticsearch环境使用
                //docker run --name elasticsearch -d -e ES_JAVA_OPTS="-Xms512m -Xmx512m" -e "discovery.type=single-node" -p 9200:9200 -p 9300:9300 elasticsearch:7.5.0
                logger.WriteToElasticsearch();
            }
        });
    }
}
