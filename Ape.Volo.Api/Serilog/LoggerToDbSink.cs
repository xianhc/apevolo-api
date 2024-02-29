using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ape.Volo.Common.DI;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.SnowflakeIdHelper;
using Ape.Volo.Entity.Monitor;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;
using SqlSugar;

namespace Ape.Volo.Api.Serilog;

public class LoggerToDbSink : IBatchedLogEventSink
{
    public async Task EmitBatchAsync(IEnumerable<LogEvent> batch)
    {
        var sugar = AutofacHelper.GetService<ISqlSugarClient>();

        var logEvents = batch.ToList();
        await RecordSql(sugar, logEvents.RecordSql());
        await RecordLog(sugar, logEvents.RecordLog());
    }

    public Task OnEmptyBatchAsync()
    {
        return Task.CompletedTask;
    }


    private async Task RecordLog(ISqlSugarClient db, List<LogEvent> logEvents)
    {
        if (!logEvents.Any())
        {
            return;
        }

        var group = logEvents.GroupBy(s => s.Level);
        foreach (var v in group)
        {
            switch (v.Key)
            {
                case LogEventLevel.Information:
                    await RecordInformation(db, v.ToList());
                    break;
                case LogEventLevel.Warning:
                    await RecordWarning(db, v.ToList());
                    break;
                case LogEventLevel.Error:
                    await RecordError(db, v.ToList());
                    break;
                case LogEventLevel.Fatal:
                    await RecordFatal(db, v.ToList());
                    break;
            }
        }
    }

    private async Task RecordInformation(ISqlSugarClient db, List<LogEvent> logEvents)
    {
        if (!logEvents.Any())
        {
            return;
        }

        var logs = new List<InformationLog>();
        foreach (var logEvent in logEvents)
        {
            var log = new InformationLog
            {
                Id = IdHelper.GetLongId(),
                CreateTime = logEvent.Timestamp.DateTime,
                Level = logEvent.Level.ToString(),
                Message = logEvent.RenderMessage(),
                MessageTemplate = logEvent.MessageTemplate.Text,
                Properties = logEvent.Properties.ToJson()
            };
            logs.Add(log);
        }

        try
        {
            await db.AsTenant().InsertableWithAttr(logs).SplitTable().ExecuteCommandAsync();
        }
        catch (Exception e)
        {
            ConsoleHelper.WriteLine(e.ToString(), ConsoleColor.Red);
        }
    }

    private async Task RecordWarning(ISqlSugarClient db, List<LogEvent> batch)
    {
        if (!batch.Any())
        {
            return;
        }

        var logs = new List<WarningLog>();
        foreach (var logEvent in batch)
        {
            var log = new WarningLog
            {
                Id = IdHelper.GetLongId(),
                CreateTime = logEvent.Timestamp.DateTime,
                Level = logEvent.Level.ToString(),
                Message = logEvent.RenderMessage(),
                MessageTemplate = logEvent.MessageTemplate.Text,
                Properties = logEvent.Properties.ToJson()
            };
            logs.Add(log);
        }

        try
        {
            await db.AsTenant().InsertableWithAttr(logs).SplitTable().ExecuteCommandAsync();
        }
        catch (Exception e)
        {
            ConsoleHelper.WriteLine(e.ToString(), ConsoleColor.Red);
        }
    }

    private async Task RecordError(ISqlSugarClient db, List<LogEvent> logEvents)
    {
        if (!logEvents.Any())
        {
            return;
        }

        var logs = new List<ErrorLog>();
        foreach (var logEvent in logEvents)
        {
            var log = new ErrorLog
            {
                Id = IdHelper.GetLongId(),
                CreateTime = logEvent.Timestamp.DateTime,
                Level = logEvent.Level.ToString(),
                Message = logEvent.RenderMessage(),
                MessageTemplate = logEvent.MessageTemplate.Text,
                Properties = logEvent.Properties.ToJson(),
            };
            logs.Add(log);
        }

        try
        {
            await db.AsTenant().InsertableWithAttr(logs).SplitTable().ExecuteCommandAsync();
        }
        catch (Exception e)
        {
            ConsoleHelper.WriteLine(e.ToString(), ConsoleColor.Red);
        }
    }

    private async Task RecordFatal(ISqlSugarClient db, List<LogEvent> logEvents)
    {
        if (!logEvents.Any())
        {
            return;
        }

        var logs = new List<FatalLog>();
        foreach (var logEvent in logEvents)
        {
            var log = new FatalLog
            {
                Id = IdHelper.GetLongId(),
                CreateTime = logEvent.Timestamp.DateTime,
                Level = logEvent.Level.ToString(),
                Message = logEvent.RenderMessage(),
                MessageTemplate = logEvent.MessageTemplate.Text,
                Properties = logEvent.Properties.ToJson()
            };
            logs.Add(log);
        }

        try
        {
            await db.AsTenant().InsertableWithAttr(logs).SplitTable().ExecuteCommandAsync();
        }
        catch (Exception e)
        {
            ConsoleHelper.WriteLine(e.ToString(), ConsoleColor.Red);
        }
    }


    private async Task RecordSql(ISqlSugarClient db, List<LogEvent> logEvents)
    {
        if (!logEvents.Any())
        {
            return;
        }

        var logs = new List<AopSqlLog>();
        foreach (var logEvent in logEvents)
        {
            var log = new AopSqlLog
            {
                Id = IdHelper.GetLongId(),
                CreateTime = logEvent.Timestamp.DateTime,
                Level = logEvent.Level.ToString(),
                Message = logEvent.RenderMessage(),
                MessageTemplate = logEvent.MessageTemplate.Text,
                Properties = logEvent.Properties.ToJson()
            };
            logs.Add(log);
        }

        try
        {
            await db.AsTenant().InsertableWithAttr(logs).SplitTable().ExecuteCommandAsync();
        }
        catch (Exception e)
        {
            ConsoleHelper.WriteLine(e.ToString(), ConsoleColor.Red);
        }
    }
}
