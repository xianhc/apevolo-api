using System;
using System.Collections.Generic;

namespace ApeVolo.IBusiness.QueryModel
{
    public class UserQueryCriteria
    {
        public string Id { get; set; }

        public List<string> DeptIds { get; set; }

        public string KeyWords { get; set; }

        public bool? Enabled { get; set; }

        public string DeptId{ get; set; }

        public List<DateTime> CreateTime { get; set; }
    }
}