using System;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Entity.Seed;
using log4net;
using Microsoft.AspNetCore.Builder;

namespace ApeVolo.Api.Middleware
{
    public static class InitDbMiddleware
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(IpLimitMiddleware));
        
        public static void UseSeedDataMildd(this IApplicationBuilder app, MyContext myContext, string webRootPath)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            try
            {
                if (AppSettings.GetValue("InitDbTable").ToBool())
                {
                    SeedData.InitSystemDataAsync(myContext, webRootPath).Wait();
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error occured seeding the Database.\n{e.Message}");
                throw;
            }
        }
    }
}