using System;
using System.Reflection;
using Ape.Volo.Common.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ape.Volo.Common.ConfigOptions.Core;

public static class ConfigurableOptions
{
    public static IServiceCollection AddConfigurableOptions(this IServiceCollection services, Type type)
    {
        string path = GetConfigurationPath(type);
        var config = App.Configuration.GetSection(path);

        Type iOptionsChangeTokenSource = typeof(IOptionsChangeTokenSource<>);
        Type iConfigureOptions = typeof(IConfigureOptions<>);
        Type configurationChangeTokenSource = typeof(ConfigurationChangeTokenSource<>);
        Type namedConfigureFromConfigurationOptions = typeof(NamedConfigureFromConfigurationOptions<>);
        iOptionsChangeTokenSource = iOptionsChangeTokenSource.MakeGenericType(type);
        iConfigureOptions = iConfigureOptions.MakeGenericType(type);
        configurationChangeTokenSource = configurationChangeTokenSource.MakeGenericType(type);
        namedConfigureFromConfigurationOptions = namedConfigureFromConfigurationOptions.MakeGenericType(type);

        services.AddOptions();
        services.AddSingleton(iOptionsChangeTokenSource,
            Activator.CreateInstance(configurationChangeTokenSource, Options.DefaultName, config) ??
            throw new InvalidOperationException());
        return services.AddSingleton(iConfigureOptions,
            Activator.CreateInstance(namedConfigureFromConfigurationOptions, Options.DefaultName, config) ??
            throw new InvalidOperationException());
    }

    /// <summary>获取配置路径</summary>
    /// <param name="optionsType">选项类型</param>
    /// <returns></returns>
    public static string GetConfigurationPath(Type optionsType)
    {
        var optionsSettings = optionsType.GetCustomAttribute<OptionsSettingsAttribute>(false);

        // Default suffix to be removed
        var defaultSuffix = nameof(Options);

        // Determine the configuration path
        string path = optionsSettings switch
        {
            // If there's no [OptionsSettings] attribute, remove the "Options" suffix if present
            null => optionsType.Name.EndsWith(defaultSuffix)
                ? optionsType.Name.Substring(0, optionsType.Name.Length - defaultSuffix.Length)
                : optionsType.Name,

            // If [OptionsSettings] attribute exists, use its Path if specified, otherwise use the class name
            _ => !string.IsNullOrWhiteSpace(optionsSettings.Path)
                ? optionsSettings.Path
                : optionsType.Name.EndsWith(defaultSuffix)
                    ? optionsType.Name.Substring(0, optionsType.Name.Length - defaultSuffix.Length)
                    : optionsType.Name
        };

        return path;
    }
}
