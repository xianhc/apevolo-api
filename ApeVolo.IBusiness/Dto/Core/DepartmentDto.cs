using System.Collections.Generic;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;
using Newtonsoft.Json;

namespace ApeVolo.IBusiness.Dto.Core
{
    [AutoMapping(typeof(Department), typeof(DepartmentDto))]
    public class DepartmentDto : BaseEntityDto
    {
        public string Name { get; set; }
        public long? PId { get; set; }

        public int Sort { get; set; }
        public bool Enabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<DepartmentDto> Children { get; set; }

        public int SubCount { get; set; }

        public bool HasChildren => SubCount > 0;

        public bool Leaf => SubCount == 0;

        public string Label
        {
            get { return Name; }
        }
    }
}