using System;
using System.Collections.Generic;

namespace Ape.Volo.IBusiness.QueryModel;

public class MenuQueryCriteria
{
    public string Title { get; set; }

    public List<DateTime> CreateTime { get; set; }

    public long? ParentId { get; set; }
}
