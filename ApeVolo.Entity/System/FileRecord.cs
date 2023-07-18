using ApeVolo.Common.DI;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.System;

/// <summary>
/// 文件记录
/// </summary>
[SugarTable("sys_file_record", "文件记录")]
public class FileRecord : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 文件描述
    /// </summary>
    [SugarColumn(ColumnDescription = "文件描述", IsNullable = false)]
    public string Description { get; set; }

    /// <summary>
    /// 文件类型
    /// </summary>
    [SugarColumn(ColumnDescription = "文件类型")]
    public string ContentType { get; set; }

    /// <summary>
    /// 文件类别
    /// </summary>
    [SugarColumn(ColumnDescription = "文件类别")]
    public string ContentTypeName { get; set; }

    /// <summary>
    /// 文件类别英文名称
    /// </summary>
    [SugarColumn(ColumnDescription = "文件类别英文名称")]
    public string ContentTypeNameEn { get; set; }

    /// <summary>
    /// 文件原名称
    /// </summary>
    [SugarColumn(ColumnDescription = "文件原名称")]
    public string OriginalName { get; set; }

    /// <summary>
    /// 文件新名称
    /// </summary>
    [SugarColumn(ColumnDescription = "文件新名称")]
    public string NewName { get; set; }

    /// <summary>
    /// 文件存储路径
    /// </summary>
    [SugarColumn(ColumnDescription = "存储路径")]
    public string FilePath { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    [SugarColumn(ColumnDescription = "文件大小")]
    public string Size { get; set; }

    public bool IsDeleted { get; set; }
}
