using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.System;

/// <summary>
/// 文件记录Dto
/// </summary>
[AutoMapping(typeof(FileRecord), typeof(CreateUpdateFileRecordDto))]
public class CreateUpdateFileRecordDto : BaseEntityDto<long>
{
    /// <summary>
    /// 描述
    /// </summary>
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// 文件类型
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// 文件类型名称
    /// </summary>
    public string ContentTypeName { get; set; }

    /// <summary>
    /// 文件类型名称(EN)
    /// </summary>
    public string ContentTypeNameEn { get; set; }

    /// <summary>
    /// 源名称
    /// </summary>
    public string OriginalName { get; set; }

    /// <summary>
    /// 新名称
    /// </summary>
    public string NewName { get; set; }

    /// <summary>
    /// 存储路径
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    public string Size { get; set; }
}
