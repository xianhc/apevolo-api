using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

[OptionsSettings]
public class RsaOptions
{
    public string PrivateKey { get; set; }
    public string PublicKey { get; set; }
}
