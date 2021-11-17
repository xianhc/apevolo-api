using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core
{
    [InitTable(typeof(Job))]
    [SugarTable("sys_job", "岗位表")]
    public class Job : BaseEntity
    {
        [SugarColumn(ColumnName = "name", ColumnDataType = "varchar", Length = 255, IsNullable = false)]
        public string Name { get; set; }

        [SugarColumn(ColumnName = "sort", IsNullable = true)]
        public int Sort { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        [SugarColumn(ColumnName = "enabled", IsNullable = false)]
        public bool Enabled { get; set; }
    }
}
