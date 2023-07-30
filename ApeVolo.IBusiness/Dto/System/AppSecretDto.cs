using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.System;

[AutoMapping(typeof(Entity.System.AppSecret), typeof(AppSecretDto))]
public class AppSecretDto : BaseEntityDto<long>
{
    public string AppId { get; set; }

    public string AppSecretKey { get; set; }

    public string AppName { get; set; }

    public string Remark { get; set; }
}
