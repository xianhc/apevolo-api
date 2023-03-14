using System;
using System.Globalization;
using ApeVolo.Common.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace ApeVolo.Api.Extensions;

public static class MultiLanguagesSetup
{
    /// <summary>
    /// 多语言支持
    /// </summary>
    /// <param name="services"></param>
    /// <param name="op"></param>
    /// <returns></returns>
    public static IServiceCollection AddMultiLanguages(this IServiceCollection services,
        Action<LocalizationOption> op = null)
    {
        services.AddLocalization(delegate(LocalizationOptions options) { options.ResourcesPath = "Resources"; });
        var supportedCultures = new[]
        {
            new CultureInfo("zh-CN"),
            new CultureInfo("en-US")
        };
        services.Configure(delegate(RequestLocalizationOptions options)
        {
            options.DefaultRequestCulture = new RequestCulture(culture: "zh-CN", uiCulture: "zh-CN");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });
        LocalizationOption localizationOption = new LocalizationOption();
        op?.Invoke(localizationOption);
        if (localizationOption.LocalizationType != null)
        {
            services.AddSingleton(localizationOption);
        }

        return services;
    }

    public static IMvcBuilder AddDataAnnotationsLocalization(this IMvcBuilder builder, Type languageType)
    {
        builder.AddDataAnnotationsLocalization(delegate(MvcDataAnnotationsLocalizationOptions options)
        {
            options.DataAnnotationLocalizerProvider = (_, factory) =>
                factory.Create(languageType);
        });
        return builder;
    }
}