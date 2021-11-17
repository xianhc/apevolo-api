using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core
{
    [InitTable(typeof(TestApeVolo))]
    [SugarTable("test_apevolo", "测试表")]
    public class TestApeVolo:BaseEntity
    {
        [SugarColumn(ColumnName = "label", ColumnDataType = "varchar", Length = 255, IsNullable = false,
            ColumnDescription = "label")]
        public string Label { get; set; }

        [SugarColumn(ColumnName = "content", ColumnDataType = "varchar", Length = 255, IsNullable = true,
            ColumnDescription = "content")]
        public string Content { get; set; }

        [SugarColumn(ColumnName = "sort", IsNullable = true, ColumnDescription = "排序")]
        public int Sort { get; set; }
    }
}