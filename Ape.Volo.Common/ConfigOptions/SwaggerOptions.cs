using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

[OptionsSettings]
public class SwaggerOptions
{
    public bool Enabled { get; set; }
    public string Name { get; set; }
    public string Version { get; set; }
    public string Title { get; set; }
}
