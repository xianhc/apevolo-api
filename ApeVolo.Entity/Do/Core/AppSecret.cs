using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core
{
    [InitTable(typeof(AppSecret))]
    [SugarTable("sys_app_secret", "三方应用秘钥表")]
    public class AppSecret : BaseEntity
    {
        [SugarColumn(ColumnName = "app_id", ColumnDescription = "应用ID", ColumnDataType = "varchar", Length = 255,
            IsNullable = false)]
        public string AppId { get; set; }

        [SugarColumn(ColumnName = "app_secret_key", ColumnDescription = "应用秘钥", ColumnDataType = "varchar",
            Length = 255,
            IsNullable = false)]
        public string AppSecretKey { get; set; }

        [SugarColumn(ColumnName = "app_name", ColumnDescription = "应用名称", ColumnDataType = "varchar", Length = 255,
            IsNullable = false)]
        public string AppName { get; set; }

        [SugarColumn(ColumnName = "remark", ColumnDescription = "备注", ColumnDataType = "varchar", Length = 255,
            IsNullable = true)]
        public string Remark { get; set; }
    }
}