using System;
using System.Collections.Generic;

namespace Ape.Volo.IBusiness.QueryModel;

public class EmailMessageTemplateQueryCriteria
{
    public string Name { get; set; }

    public bool? IsActive { get; set; }
    public List<DateTime> CreateTime { get; set; }
}
