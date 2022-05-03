using ApeVolo.Common.DI;
using ApeVolo.Entity.Do.Base;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core;

/// <summary>
/// 用户岗位关联
/// </summary>
[SugarTable("sys_users_jobs", "用户岗位关联")]
public class UserJobs : EntityRoot<long>, ILocalizedTable
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(ColumnName = "user_id", ColumnDataType = "bigint", Length = 19, IsNullable = false,
        ColumnDescription = "用户ID")]
    public long UserId { get; set; }

    /// <summary>
    /// 岗位ID
    /// </summary>
    [SugarColumn(ColumnName = "job_id", ColumnDataType = "bigint", Length = 19, IsNullable = false,
        ColumnDescription = "岗位ID")]
    public long JobId { get; set; }
}