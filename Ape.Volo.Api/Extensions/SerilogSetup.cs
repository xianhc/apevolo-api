using System;
using Ape.Volo.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Ape.Volo.Api.Extensions;

/// <summary>
/// Serilog日志 替换内置Logging
/// </summary>
public static class SerilogSetup
{
    public static void AddSerilogSetup(this IServiceCollection services)
    {
        if (services.IsNull()) throw new ArgumentNullException(nameof(services));

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(dispose: true);
        });
        //services.AddSingleton<Serilog.Extensions.Hosting.DiagnosticContext>();
    }
}
