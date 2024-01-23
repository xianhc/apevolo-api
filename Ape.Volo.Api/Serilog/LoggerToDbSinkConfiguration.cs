using System;
using Serilog;
using Serilog.Sinks.PeriodicBatching;

namespace Ape.Volo.Api.Serilog;

public static class LoggerToDbSinkConfiguration
{
    public static LoggerConfiguration WriteToDb(this LoggerConfiguration loggerConfiguration)
    {
        var loggerToDbSink = new LoggerToDbSink();

        var batchingOptions = new PeriodicBatchingSinkOptions
        {
            BatchSizeLimit = 500,
            Period = TimeSpan.FromSeconds(1),
            EagerlyEmitFirstEvent = true,
            QueueLimit = 10000
        };

        var batchingSink = new PeriodicBatchingSink(loggerToDbSink, batchingOptions);

        return loggerConfiguration.WriteTo.Sink(batchingSink);
    }
}
