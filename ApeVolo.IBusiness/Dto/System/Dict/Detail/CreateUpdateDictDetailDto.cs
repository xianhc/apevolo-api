using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.System.Dictionary;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.System.Dict.Detail;

[AutoMapping(typeof(DictDetail), typeof(CreateUpdateDictDetailDto))]
public class CreateUpdateDictDetailDto : EntityDtoRoot<long>
{
    public string DictId { get; set; }

    public string Label { get; set; }

    public string Value { get; set; }

    public string DictSort { get; set; }

    public DictDto2 dict { get; set; }
}