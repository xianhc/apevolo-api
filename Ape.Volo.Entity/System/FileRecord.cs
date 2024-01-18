using Ape.Volo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.System;

/// <summary>
/// 文件记录
/// </summary>
[SugarTable("sys_file_record")]
public class FileRecord : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 文件描述
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string Description { get; set; }

    /// <summary>
    /// 文件类型
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string ContentType { get; set; }

    /// <summary>
    /// 文件类别
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string ContentTypeName { get; set; }

    /// <summary>
    /// 文件类别英文名称
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string ContentTypeNameEn { get; set; }

    /// <summary>
    /// 文件原名称
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string OriginalName { get; set; }

    /// <summary>
    /// 文件新名称
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string NewName { get; set; }

    /// <summary>
    /// 文件存储路径
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string FilePath { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Size { get; set; }

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsDeleted { get; set; }
}
