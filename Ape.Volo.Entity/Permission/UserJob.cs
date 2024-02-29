using SqlSugar;

namespace Ape.Volo.Entity.Permission
{
    /// <summary>
    /// 用户岗位关联
    /// </summary>
    [SugarTable("sys_user_job")]
    public class UserJob
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long UserId { get; set; }

        /// <summary>
        /// 岗位ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long JobId { get; set; }
    }
}
