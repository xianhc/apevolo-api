using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ape.Volo.Common.Global;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Sinks.Elasticsearch;
using SqlSugar;

namespace Ape.Volo.Api.Serilog;

/// <summary>
/// 日志输出扩展
/// </summary>
public static class LoggerOutExtensions
{
    /// <summary>
    /// 控制台
    /// </summary>
    /// <param name="loggerConfiguration"></param>
    /// <returns></returns>
    public static LoggerConfiguration WriteToConsole(this LoggerConfiguration loggerConfiguration)
    {
        //系统日志
        loggerConfiguration = loggerConfiguration.WriteTo.Logger(lg =>
            lg.RecordLog().WriteTo.Console());

        //sql日志
        loggerConfiguration = loggerConfiguration.WriteTo.Logger(lg =>
            lg.RecordSql(LoggerProperty.ToConsole).WriteTo.Console());

        return loggerConfiguration;
    }

    /// <summary>
    /// 文件
    /// </summary>
    /// <param name="loggerConfiguration"></param>
    /// <param name="logEvent"></param>
    /// <returns></returns>
    public static LoggerConfiguration WriteToFile(this LoggerConfiguration loggerConfiguration, LogEventLevel logEvent)
    {
        //系统日志
        loggerConfiguration = loggerConfiguration.WriteTo.Logger(lg =>
            lg.Filter.ByIncludingOnly(p => p.Level == logEvent).RecordLog().WriteTo.Async(s =>
                s.File(Path.Combine(AppSettings.ContentRootPath, "Logs", logEvent.ToString(), ".log"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: LoggerProperty.MessageTemplate, retainedFileCountLimit: 31,
                    encoding: Encoding.UTF8)));

        //sql日志
        loggerConfiguration = loggerConfiguration.WriteTo.Logger(lg =>
            lg.Filter.ByIncludingOnly(p => p.Level == logEvent).RecordSql(LoggerProperty.ToFile)
                .WriteTo.Async(s =>
                    s.File(Path.Combine(AppSettings.ContentRootPath, "Logs", "AopSql", ".log"),
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: LoggerProperty.MessageTemplate, retainedFileCountLimit: 31,
                        encoding: Encoding.UTF8)));
        return loggerConfiguration;
    }

    /// <summary>
    /// Elasticsearch
    /// </summary>
    /// <param name="loggerConfiguration"></param>
    /// <returns></returns>
    public static LoggerConfiguration WriteToElasticsearch(this LoggerConfiguration loggerConfiguration)
    {
        //系统日志
        loggerConfiguration = loggerConfiguration.WriteTo.Logger(lg =>
            lg.RecordLog().WriteTo.Elasticsearch(
                new ElasticsearchSinkOptions(new Uri("http://127.0.0.1:9200/"))
                {
                    AutoRegisterTemplate = true
                }));

        //sql日志
        loggerConfiguration = loggerConfiguration.WriteTo.Logger(lg =>
            lg.RecordSql(LoggerProperty.ToElasticsearch)
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://127.0.0.1:9200/"))
                {
                    AutoRegisterTemplate = true
                }));

        return loggerConfiguration;
    }


    public static LoggerConfiguration RecordSql(this LoggerConfiguration lc, string propertyName)
    {
        lc = lc.Filter.ByIncludingOnly(Matching.WithProperty<bool>(LoggerProperty.RecordSqlLog,
            s => s)).Filter.ByIncludingOnly(Matching.WithProperty<bool>(
            propertyName, s => s));
        return lc;
    }

    public static List<LogEvent> RecordSql(this List<LogEvent> batch)
    {
        //只记录 Insert、Update、Delete语句
        return batch.Where(s =>
                s.WithProperty<bool>(LoggerProperty.RecordSqlLog, q => q))
            .Where(s => s.WithProperty<SugarActionType>(LoggerProperty.SugarActionType,
                q => !new[] { SugarActionType.UnKnown, SugarActionType.Query }.Contains(q))).ToList();
    }

    public static LoggerConfiguration RecordLog(this LoggerConfiguration lc)
    {
        lc = lc.Filter.ByIncludingOnly(WithProperty<bool>(LoggerProperty.RecordSqlLog,
            s => !s));
        return lc;
    }

    public static List<LogEvent> RecordLog(this IEnumerable<LogEvent> batch)
    {
        return batch.Where(s => WithProperty<bool>(LoggerProperty.RecordSqlLog,
            q => !q)(s)).ToList();
    }

    public static Func<LogEvent, bool> WithProperty<T>(string propertyName, Func<T, bool> predicate)
    {
        return e =>
        {
            if (!e.Properties.TryGetValue(propertyName, out var propertyValue)) return true;

            return propertyValue is ScalarValue { Value: T value } && predicate(value);
        };
    }

    public static bool WithProperty<T>(this LogEvent e, string key, Func<T, bool> predicate)
    {
        if (!e.Properties.TryGetValue(key, out var propertyValue)) return false;

        return propertyValue is ScalarValue { Value: T value } && predicate(value);
    }
}
