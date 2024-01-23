using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.System;

[AutoMapping(typeof(FileRecord), typeof(FileRecordDto))]
public class FileRecordDto : BaseEntityDto<long>
{
    public string Description { get; set; }

    public string ContentType { get; set; }

    public string ContentTypeName { get; set; }

    public string ContentTypeNameEn { get; set; }

    public string OriginalName { get; set; }

    public string NewName { get; set; }

    public string FilePath { get; set; }

    public string Size { get; set; }
}
