using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ape.Volo.IBusiness.RequestModel;

public class IdCollectionString
{
    [Required]
    public HashSet<string> IdArray { get; set; }
}
