using System;
using SqlSugar;

namespace ApeVolo.Entity.Do
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "id", ColumnDataType = "char", IsNullable = false, Length = 19,
            IsPrimaryKey = true, ColumnDescription = "ID主键")]
        public string Id { get; set; }

        /// <summary>
        /// 创建者名称
        /// </summary>
        [SugarColumn(ColumnName = "create_by", ColumnDataType = "varchar", Length = 255, IsNullable = true,
            ColumnDescription = "创建者账号")]
        public string CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "create_time", IsNullable = true, ColumnDescription = "创建时间")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新者名称
        /// </summary>
        [SugarColumn(ColumnName = "update_by", ColumnDataType = "varchar", Length = 255, IsNullable = true,
            ColumnDescription = "更新者账户")]
        public string UpdateBy { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [SugarColumn(ColumnName = "update_time", IsNullable = true, ColumnDescription = "更新时间")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn(ColumnName = "is_deleted", IsNullable = true, ColumnDescription = "软删除(1:删除，0:未删除)",
            DefaultValue = "0")]
        public bool IsDeleted { get; set; } = false;
    }
}