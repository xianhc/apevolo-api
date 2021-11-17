using System;
using System.Collections.Generic;

namespace ApeVolo.IBusiness.QueryModel
{
    public class EmailAccountQueryCriteria
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }

        public List<DateTime> CreateTime { get; set; }
    }
}
