using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Log4Net;
using Microsoft.Extensions.Logging;

namespace ApeVolo.Api.Middleware;

/// <summary>
/// log4net扩展
/// </summary>
public static class Log4NetExtensions
{
    public static ILoggerFactory AddLog4Net(this ILoggerFactory factory)
    {
        if (AppSettings.GetValue("Middleware", "RecordAllLogs", "Enabled").ToBool())
        {
            factory.AddProvider(new Log4NetProvider("Log4net.config"));
        }

        return factory;
    }
}