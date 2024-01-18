using System;
using System.Collections.Generic;

namespace Ape.Volo.IBusiness.QueryModel;

public class QuartzNetQueryCriteria
{
    public string TaskName { get; set; }

    public List<DateTime> CreateTime { get; set; }
}
