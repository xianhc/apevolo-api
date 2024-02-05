using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.System;

/// <summary>
/// 应用密钥Dto
/// </summary>
[AutoMapping(typeof(AppSecret), typeof(AppSecretDto))]
public class AppSecretDto : BaseEntityDto<long>
{
    /// <summary>
    /// 应用ID
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// 签名Key
    /// </summary>
    public string AppSecretKey { get; set; }

    /// <summary>
    /// 应用名称
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }
}
