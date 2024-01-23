using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Base;
using SqlSugar;

namespace Ape.Volo.Entity.System
{
    /// <summary>
    /// 字典详情
    /// </summary>
    [SugarTable("sys_dict_detail")]
    public class DictDetail : BaseEntity, ISoftDeletedEntity
    {
        /// <summary>
        /// 字典ID
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long DictId { get; set; }

        /// <summary>
        /// 字典标签
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Label { get; set; }

        /// <summary>
        /// 字典值
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Value { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string DictSort { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
