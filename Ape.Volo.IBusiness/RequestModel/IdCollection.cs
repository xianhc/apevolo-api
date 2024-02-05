using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ape.Volo.IBusiness.RequestModel;

/// <summary>
/// id模型(log)
/// </summary>
public class IdCollection
{
    /// <summary>
    /// ids
    /// </summary>
    [Required]
    public HashSet<long> IdArray { get; set; }
}
