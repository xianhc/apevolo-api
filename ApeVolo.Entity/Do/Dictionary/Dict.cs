using System.Collections.Generic;
using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Dictionary
{
    [InitTable(typeof(Dict))]
    [SugarTable("sys_dict", "字典表")]
    public class Dict : BaseEntity
    {
        [SugarColumn(ColumnName = "name", ColumnDataType = "varchar", Length = 255, IsNullable = false)]
        public string Name { get; set; }

        [SugarColumn(ColumnName = "description", ColumnDataType = "varchar", Length = 255, IsNullable = false)]
        public string Description { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<DictDetail> DictDetails { get; set; }
    }
}