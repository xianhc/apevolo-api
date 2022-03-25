using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Do.Core;

namespace ApeVolo.IBusiness.Dto.Core;

[AutoMapping(typeof(AppSecret), typeof(AppSecretDto))]
public class AppSecretDto : BaseEntityDto
{
    public string AppId { get; set; }

    public string AppSecretKey { get; set; }

    public string AppName { get; set; }

    public string Remark { get; set; }
}