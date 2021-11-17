﻿using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Dictionary;
using Newtonsoft.Json;

namespace ApeVolo.IBusiness.Dto.Dictionary
{
    [AutoMapping(typeof(DictDetail), typeof(DictDetailDto))]
    public class DictDetailDto : BaseEntityDto
    {
        [JsonIgnore]
        //[JsonProperty]
        public string DictId { get; set; }
        
        public string Label { get; set; }

        public string Value { get; set; }

        public string DictSort { get; set; }

        public DictDto2 Dict { get; set; }
    }
}
