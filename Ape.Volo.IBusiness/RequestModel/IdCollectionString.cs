using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ape.Volo.IBusiness.RequestModel;

/// <summary>
/// id模型(string)
/// </summary>
public class IdCollectionString
{
    [Required]
    public HashSet<string> IdArray { get; set; }
}
