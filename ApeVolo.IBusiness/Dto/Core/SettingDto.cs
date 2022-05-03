using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.Dto.Core;

[AutoMapping(typeof(Setting), typeof(SettingDto))]
public class SettingDto : EntityDtoRoot<long>
{
    public string Name { get; set; }

    public string Value { get; set; }

    public bool Enabled { get; set; }

    public string Description { get; set; }
}