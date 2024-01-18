using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.System;

[AutoMapping(typeof(ApeVolo.Entity.System.FileRecord), typeof(CreateUpdateFileRecordDto))]
public class CreateUpdateFileRecordDto : BaseEntityDto<long>
{
    [Required]
    public string Description { get; set; }

    public string ContentType { get; set; }

    public string ContentTypeName { get; set; }

    public string ContentTypeNameEn { get; set; }

    public string OriginalName { get; set; }

    public string NewName { get; set; }

    public string FilePath { get; set; }

    public string Size { get; set; }
}
