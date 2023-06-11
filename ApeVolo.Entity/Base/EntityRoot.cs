using System;
using SqlSugar;

namespace ApeVolo.Entity.Base;

/// <summary>
/// 实体基类
/// </summary>
public class EntityRoot<T> where T : IEquatable<T>
{
    /// <summary>
    /// 主键
    /// </summary>
    [SugarColumn(ColumnName = "id", ColumnDataType = "bigint", IsNullable = false, IsPrimaryKey = true,
        ColumnDescription = "ID主键")]
    public T Id { get; set; }

    /// <summary>
    /// 创建者名称
    /// </summary>
    [SugarColumn(ColumnName = "create_by", IsNullable = true, ColumnDescription = "创建者账号")]
    public string CreateBy { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnName = "create_time", IsNullable = true, ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新者名称
    /// </summary>
    [SugarColumn(ColumnName = "update_by", IsNullable = true, ColumnDescription = "更新者账户")]
    public string UpdateBy { get; set; }

    /// <summary>
    /// 最后更新时间
    /// </summary>
    [SugarColumn(ColumnName = "update_time", IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    [SugarColumn(ColumnName = "is_deleted", IsNullable = true, ColumnDescription = "软删除(1:删除，0:未删除)")]
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// 删除者名称
    /// </summary>
    [SugarColumn(ColumnName = "delete_by", IsNullable = true, ColumnDescription = "删除者账号")]
    public string DeletedBy { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    [SugarColumn(ColumnName = "delete_time", IsNullable = true, ColumnDescription = "删除时间")]
    public DateTime? DeletedTime { get; set; }
}