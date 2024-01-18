using Ape.Volo.Common.AttributeExt;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.System;

[AutoMapping(typeof(ApeVolo.Entity.System.Setting), typeof(SettingDto))]
public class SettingDto : BaseEntityDto<long>
{
    public string Name { get; set; }

    public string Value { get; set; }

    public bool Enabled { get; set; }

    public string Description { get; set; }
}
