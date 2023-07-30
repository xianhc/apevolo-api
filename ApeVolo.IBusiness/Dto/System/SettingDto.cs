using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.System;

[AutoMapping(typeof(Entity.System.Setting), typeof(SettingDto))]
public class SettingDto : BaseEntityDto<long>
{
    public string Name { get; set; }

    public string Value { get; set; }

    public bool Enabled { get; set; }

    public string Description { get; set; }
}
