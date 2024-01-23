using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.System;

[AutoMapping(typeof(AppSecret), typeof(CreateUpdateAppSecretDto))]
public class CreateUpdateAppSecretDto : BaseEntityDto<long>
{
    public string AppId { get; set; }

    public string AppSecretKey { get; set; }

    [Required]
    public string AppName { get; set; }

    public string Remark { get; set; }
}
