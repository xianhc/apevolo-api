using ApeVolo.Common.DI;
using SqlSugar;

namespace ApeVolo.Entity.Permission;

/// <summary>
/// 用户岗位关联
/// </summary>
[SugarTable("sys_users_jobs", "用户岗位关联")]
public class UserJobs
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(ColumnDataType = "bigint", IsPrimaryKey = true, ColumnDescription = "用户ID")]
    public long UserId { get; set; }

    /// <summary>
    /// 岗位ID
    /// </summary>
    [SugarColumn(ColumnDataType = "bigint", IsPrimaryKey = true, ColumnDescription = "岗位ID")]
    public long JobId { get; set; }
}
