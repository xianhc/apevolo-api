using System;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Entity.Seed;
using log4net;
using Microsoft.AspNetCore.Builder;

namespace ApeVolo.Api.Middleware;

public static class DataSeederMiddleware
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(DataSeederMiddleware));

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
            Log.Error($"Error occured seeding the Database.\n{e.Message}");
            throw;
        }
    }
}