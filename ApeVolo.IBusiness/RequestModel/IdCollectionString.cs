using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApeVolo.IBusiness.RequestModel;

public class IdCollectionString
{
    [Required]
    public HashSet<string> IdArray { get; set; }
}