using System;
using System.Collections.Generic;

namespace ApeVolo.IBusiness.QueryModel
{
    public class JobQueryCriteria
    {
        public string JobName { get; set; }

        public bool? Enabled { get; set; }
        public List<DateTime> CreateTime { get; set; }
    }
}
