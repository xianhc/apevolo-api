using System;
using System.Collections.Generic;

namespace ApeVolo.IBusiness.QueryModel;

public class DeptQueryCriteria
{
    public string DeptName { get; set; }

    public bool? Enabled { get; set; }

    public long? ParentId { get; set; }

    public List<DateTime> CreateTime { get; set; }
}
