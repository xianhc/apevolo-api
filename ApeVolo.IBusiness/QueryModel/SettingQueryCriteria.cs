using System;
using System.Collections.Generic;

namespace ApeVolo.IBusiness.QueryModel;

public class SettingQueryCriteria
{
    public string KeyWords { get; set; }
    public bool? Enabled { get; set; }
    public List<DateTime> CreateTime { get; set; }
}