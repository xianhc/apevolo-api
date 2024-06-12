using System;
using System.IO;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Global;
using IP2Region.Net.Abstractions;
using IP2Region.Net.XDB;
using Microsoft.Extensions.DependencyInjection;

namespace Ape.Volo.Api.Extensions
{
    public static class IpSearcherSetup
    {
        public static void AddIpSearcherSetup(this IServiceCollection services)
        {
            if (services.IsNull()) throw new ArgumentNullException(nameof(services));
            services.AddSingleton<ISearcher>(new Searcher(CachePolicy.Content,
                Path.Combine(AppSettings.WebRootPath, "resources", "ip", "ip2region.xdb")));
        }
    }
}
