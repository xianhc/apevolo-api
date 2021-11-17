using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core
{
    [InitTable(typeof(UserJobs))]
    [SugarTable("sys_users_jobs", "用户岗位关联表")]
    public class UserJobs
    {
        [SugarColumn(ColumnName = "user_id", ColumnDataType = "char", Length = 19, IsNullable = false, IsPrimaryKey = true)]
        public string UserId { get; set; }

        [SugarColumn(ColumnName = "job_id", ColumnDataType = "char", Length = 19, IsNullable = false, IsPrimaryKey = true)]
        public string JobId { get; set; }
    }
}
