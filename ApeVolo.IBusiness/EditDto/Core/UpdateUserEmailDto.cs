using ApeVolo.Common.AttributeExt;

namespace ApeVolo.IBusiness.EditDto.Core;

public class UpdateUserEmailDto
{
    [ApeVoloRequired(Message = "密码不能为空！")] public string Password { get; set; }
    [ApeVoloRequired(Message = "邮箱不能为空！")] public string Email { get; set; }

    [ApeVoloRequired(Message = "邮箱验证码不能为空！")]
    public string Code { get; set; }
}