using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApeVolo.IBusiness.RequestModel;

[Serializable]
public class IdCollection
{
    [Required]
    public HashSet<long> IdArray { get; set; }
}