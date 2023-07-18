using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.System;

[AutoMapping(typeof(Entity.System.FileRecord), typeof(FileRecordDto))]
public class FileRecordDto : BaseEntityDto<long>
{
    [Display(Name = "File.Description")]
    public string Description { get; set; }

    [Display(Name = "File.ContentType")]
    public string ContentType { get; set; }

    [Display(Name = "File.ContentTypeName")]
    public string ContentTypeName { get; set; }

    [Display(Name = "File.ContentTypeNameEn")]
    public string ContentTypeNameEn { get; set; }

    [Display(Name = "File.OriginalName")]
    public string OriginalName { get; set; }

    [Display(Name = "File.NewName")]
    public string NewName { get; set; }

    [Display(Name = "File.FilePath")]
    public string FilePath { get; set; }

    [Display(Name = "File.Size")]
    public string Size { get; set; }
}
