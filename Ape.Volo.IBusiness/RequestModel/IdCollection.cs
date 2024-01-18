using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ape.Volo.IBusiness.RequestModel;

public class IdCollection
{
    [Required]
    public HashSet<long> IdArray { get; set; }
}
