using System;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Helper.Serilog;
using ApeVolo.Entity.Seed;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace ApeVolo.Api.Middleware;

public static class DataSeederMiddleware
{
    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(DataSeederMiddleware));

    public static void UseDataSeederMiddleware(this IApplicationBuilder app, MyContext myContext)
    {
        if (app == null) throw new ArgumentNullException(nameof(app));

        try
        {
            if (AppSettings.GetValue<bool>("InitDbTable"))
            {
                DataSeeder.InitSystemDataAsync(myContext, AppSettings.WebRootPath).Wait();
            }
        }
        catch (Exception e)
        {
            Logger.Error($"Error occured seeding the Database.\n{e.Message}");
            throw;
        }
    }
}