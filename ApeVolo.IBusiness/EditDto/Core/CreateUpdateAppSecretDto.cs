using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.EditDto.Core;

[AutoMapping(typeof(AppSecret), typeof(CreateUpdateAppSecretDto))]
public class CreateUpdateAppSecretDto : BaseCreateUpdateEntityDto
{
    public string AppId { get; set; }

    public string AppSecretKey { get; set; }

    [ApeVoloRequired(Message = "应用名称不能为空！")]
    public string AppName { get; set; }

    public string Remark { get; set; }
}