using System;
using System.Collections.Generic;

namespace ApeVolo.IBusiness.QueryModel;

public class FileRecordQueryCriteria
{
    public string KeyWords { get; set; }
    public List<DateTime> CreateTime { get; set; }
}