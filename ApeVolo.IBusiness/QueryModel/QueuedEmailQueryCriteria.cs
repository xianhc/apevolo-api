using System;
using System.Collections.Generic;

namespace ApeVolo.IBusiness.QueryModel
{
    public class QueuedEmailQueryCriteria
    {
        public string Id { get; set; }
        public int MaxTries { get; set; }

        public string Form { get; set; }

        public string To { get; set; }

        public List<DateTime> CreateTime { get; set; }

        public bool? IsSend { get; set; }
    }
}