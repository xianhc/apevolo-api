using Newtonsoft.Json;
using System;

namespace ApeVolo.IBusiness.EditDto
{
    public class BaseCreateUpdateEntityDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 创建者名称
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 更新者名称
        /// </summary>
        public string UpdateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;//DateTime.Now.ToUnixTimeStampMillisecond();

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = DateTime.Now;//DateTime.Now.ToUnixTimeStampMillisecond();
        
        /// <summary>
        /// 是否删除
        /// </summary>
        [JsonIgnore]
        public bool IsDeleted { get; set; } = false;
    }
}
