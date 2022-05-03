using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Dictionary;
using ApeVolo.IBusiness.Dto;
using ApeVolo.IBusiness.Dto.Dictionary;

namespace ApeVolo.IBusiness.EditDto.Dict;

[AutoMapping(typeof(DictDetail), typeof(CreateUpdateDictDetailDto))]
public class CreateUpdateDictDetailDto : EntityDtoRoot<long>
{
    public string DictId { get; set; }

    public string Label { get; set; }

    public string Value { get; set; }

    public string DictSort { get; set; }

    public DictDto2 dict { get; set; }
}