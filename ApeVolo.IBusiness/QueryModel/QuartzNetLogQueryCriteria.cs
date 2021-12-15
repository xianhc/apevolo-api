using System;
using System.Collections.Generic;

namespace ApeVolo.IBusiness.QueryModel
{
    public class QuartzNetLogQueryCriteria
    {
        public long Id { get; set; }

        public bool? IsSuccess { get; set; }

        public List<DateTime> CreateTime { get; set; }
    }
}
