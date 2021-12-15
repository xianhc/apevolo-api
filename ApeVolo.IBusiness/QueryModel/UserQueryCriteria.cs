using System;
using System.Collections.Generic;

namespace ApeVolo.IBusiness.QueryModel
{
    public class UserQueryCriteria
    {
        public long Id { get; set; }

        public List<long> DeptIds { get; set; }

        public string KeyWords { get; set; }

        public bool? Enabled { get; set; }

        public long DeptId { get; set; }

        public List<DateTime> CreateTime { get; set; }
    }
}