using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ApeVolo.IBusiness.Base
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public class BaseEntityDto<T> where T : IEquatable<T>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Display(Name = "Sys.ID")]
        public T Id { get; set; }

        /// <summary>
        /// 创建者名称
        /// </summary>
        [Display(Name = "Sys.CreateBy")]
        public string CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "Sys.CreateTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新者名称
        /// </summary>
        [Display(Name = "Sys.UpdateBy")]
        public string UpdateBy { get; set; }


        /// <summary>
        /// 最后更新时间
        /// </summary>
        [Display(Name = "Sys.UpdateTime")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [JsonIgnore]
        [Display(Name = "Sys.IsDeleted")]
        public bool IsDeleted { get; set; } = false;
    }
}
