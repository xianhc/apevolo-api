using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApeVolo.Common.Resources;

/// <summary>
/// 语言本地化
/// </summary>
public static class Localized
{
    private static readonly IStringLocalizer StringLocalizer =
        new ResourceManagerStringLocalizerFactory(
            Options.Create(new LocalizationOptions { ResourcesPath = "" }),
            new LoggerFactory()).Create(typeof(Localized));

    /// <summary>
    /// Gets the string resource with the given name.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <returns>The string resource as a <see cref="LocalizedString"/>.</returns>
    public static string Get(string name)
    {
        return StringLocalizer[name];
    }

    /// <summary>
    /// Gets the string resource with the given name and formatted with the supplied arguments.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <param name="arguments">The values to format the string with.</param>
    /// <returns>The formatted string resource as a <see cref="LocalizedString"/>.</returns>
    public static string Get(string name, params object[] arguments)
    {
        return StringLocalizer[name, arguments];
    }

    /// <summary>
    /// Gets all string resources.
    /// </summary>
    /// <param name="includeParentCultures">
    /// A <see cref="System.Boolean"/> indicating whether to include strings from parent cultures.
    /// </param>
    /// <returns>The strings.</returns>
    public static IEnumerable<LocalizedString> GetAllStrings(
        bool includeParentCultures)
    {
        return StringLocalizer.GetAllStrings();
    }
}