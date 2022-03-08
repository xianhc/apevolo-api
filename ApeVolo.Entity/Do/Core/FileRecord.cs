using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core
{
    [InitTable(typeof(FileRecord))]
    [SugarTable("sys_file_record", "文件记录表")]
    public class FileRecord : BaseEntity
    {
        //[SugarColumn(ColumnName = "app_id", ColumnDescription = "应用ID", ColumnDataType = "varchar", Length = 255,
        //IsNullable = false)]
        //public string AppId { get; set; }

        [SugarColumn(ColumnName = "description", ColumnDescription = "文件描述", ColumnDataType = "varchar", Length = 255,
            IsNullable = false)]
        public string Description { get; set; }

        [SugarColumn(ColumnName = "content_type", ColumnDescription = "文件类型", ColumnDataType = "varchar", Length = 255
        )]
        public string ContentType { get; set; }

        [SugarColumn(ColumnName = "content_type_name", ColumnDescription = "文件类别", ColumnDataType = "varchar",
            Length = 50
        )]
        public string ContentTypeName { get; set; }
        
        [SugarColumn(ColumnName = "content_type_name_en", ColumnDescription = "文件类别英文名称", ColumnDataType = "varchar",
            Length = 50
        )]
        public string ContentTypeNameEn { get; set; }

        [SugarColumn(ColumnName = "original_name", ColumnDescription = "文件原名称", ColumnDataType = "varchar",
            Length = 255
        )]
        public string OriginalName { get; set; }

        [SugarColumn(ColumnName = "new_name", ColumnDescription = "文件新名称", ColumnDataType = "varchar", Length = 255
        )]
        public string NewName { get; set; }

        [SugarColumn(ColumnName = "file_path", ColumnDescription = "存储路径", ColumnDataType = "varchar", Length = 255
        )]
        public string FilePath { get; set; }

        [SugarColumn(ColumnName = "size", ColumnDescription = "文件大小", ColumnDataType = "varchar", Length = 50
        )]
        public string Size { get; set; }
    }
}