using System;
using System.Collections.Generic;

namespace ApeVolo.IBusiness.QueryModel
{
    public class MenuQueryCriteria
    {
        public string Title { get; set; }

        public List<DateTime> CreateTime { get; set; }

        public string PId { get; set; }
    }
}