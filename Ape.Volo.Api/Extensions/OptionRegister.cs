using System;
using System.Linq;
using System.Reflection;
using Ape.Volo.Common.Attributes;
using Ape.Volo.Common.ConfigOptions.Core;
using Ape.Volo.Common.Global;
using Microsoft.Extensions.DependencyInjection;

namespace Ape.Volo.Api.Extensions;

public static class OptionRegister
{
    /// <summary>
    /// 注册配置选项
    /// </summary>
    /// <param name="services"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void AddOptionRegister(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        var optionTypes = GlobalType.CommonTypes
            .Where(x => x.GetCustomAttribute<OptionsSettingsAttribute>() != null).ToList();

        foreach (var optionType in optionTypes)
        {
            services.AddConfigurableOptions(optionType);
        }
    }
}
