using System;
using Newtonsoft.Json;

namespace Ape.Volo.IBusiness.Base
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public class BaseEntityDto<T> where T : IEquatable<T>
    {
        /// <summary>
        /// 主键
        /// </summary>
        public T Id { get; set; }

        /// <summary>
        /// 创建者名称
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新者名称
        /// </summary>
        public string UpdateBy { get; set; }


        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [JsonIgnore]
        public bool IsDeleted { get; set; }
    }
}
