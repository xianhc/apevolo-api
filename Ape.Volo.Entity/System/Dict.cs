using System.Collections.Generic;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Base;
using SqlSugar;

namespace Ape.Volo.Entity.System
{
    /// <summary>
    /// 字典
    /// </summary>
    [SugarTable("sys_dict")]
    public class Dict : BaseEntity, ISoftDeletedEntity
    {
        /// <summary>
        /// 字典名称
        /// </summary>
        /// <returns></returns>
        [SugarColumn(IsNullable = false)]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Description { get; set; }


        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }

        #region 扩展属性

        /// <summary>
        /// 字典详情
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(DictDetail.DictId))]
        public List<DictDetail> DictDetails { get; set; }

        #endregion
    }
}
