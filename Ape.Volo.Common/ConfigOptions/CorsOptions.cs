using System.Collections.Generic;
using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

[OptionsSettings]
public class CorsOptions
{
    public string Name { get; set; }
    public bool EnableAll { get; set; }
    public List<Policy> Policy { get; set; }
}

public class Policy
{
    public string Name { get; set; }
    public string Domain { get; set; }
}
