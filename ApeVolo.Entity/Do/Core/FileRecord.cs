using ApeVolo.Common.DI;
using ApeVolo.Entity.Do.Base;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core;

/// <summary>
/// 文件记录
/// </summary>
[SugarTable("sys_file_record", "文件记录")]
public class FileRecord : EntityRoot<long>, ILocalizedTable
{
    //[SugarColumn(ColumnName = "app_id", ColumnDescription = "应用ID", ColumnDataType = "nvarchar", Length = 255,
    //IsNullable = false)]
    //public string AppId { get; set; }
    /// <summary>
    /// 文件描述
    /// </summary>
    [SugarColumn(ColumnName = "description", ColumnDescription = "文件描述", IsNullable = false)]
    public string Description { get; set; }

    /// <summary>
    /// 文件类型
    /// </summary>
    [SugarColumn(ColumnName = "content_type", ColumnDescription = "文件类型")]
    public string ContentType { get; set; }

    /// <summary>
    /// 文件类别
    /// </summary>
    [SugarColumn(ColumnName = "content_type_name", ColumnDescription = "文件类别")]
    public string ContentTypeName { get; set; }

    /// <summary>
    /// 文件类别英文名称
    /// </summary>
    [SugarColumn(ColumnName = "content_type_name_en", ColumnDescription = "文件类别英文名称")]
    public string ContentTypeNameEn { get; set; }

    /// <summary>
    /// 文件原名称
    /// </summary>
    [SugarColumn(ColumnName = "original_name", ColumnDescription = "文件原名称")]
    public string OriginalName { get; set; }

    /// <summary>
    /// 文件新名称
    /// </summary>
    [SugarColumn(ColumnName = "new_name", ColumnDescription = "文件新名称")]
    public string NewName { get; set; }

    /// <summary>
    /// 文件存储路径
    /// </summary>
    [SugarColumn(ColumnName = "file_path", ColumnDescription = "存储路径")]
    public string FilePath { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    [SugarColumn(ColumnName = "size", ColumnDescription = "文件大小")]
    public string Size { get; set; }
}