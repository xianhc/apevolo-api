using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Dictionary
{
    [InitTable(typeof(DictDetail))]
    [SugarTable("sys_dict_detail", "字典详细表")]
    public class DictDetail : BaseEntity
    {
        [SugarColumn(ColumnName = "dict_id", ColumnDataType = "bigint", Length = 19, IsNullable = false)]
        public long DictId { get; set; }

        [SugarColumn(ColumnName = "label", ColumnDataType = "varchar", Length = 255, IsNullable = false)]
        public string Label { get; set; }

        [SugarColumn(ColumnName = "value", ColumnDataType = "varchar", Length = 255, IsNullable = false)]
        public string Value { get; set; }

        [SugarColumn(ColumnName = "dict_sort", ColumnDataType = "varchar", Length = 255, IsNullable = false)]
        public string DictSort { get; set; }
    }
}
