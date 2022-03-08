using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.EditDto.Core;

[AutoMapping(typeof(FileRecord), typeof(CreateUpdateFileRecordDto))]
public class CreateUpdateFileRecordDto : BaseCreateUpdateEntityDto
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