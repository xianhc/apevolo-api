using ApeVolo.Common.AttributeExt;

namespace ApeVolo.IBusiness.EditDto.Core;

public class UpdateUserPassDto
{
    [ApeVoloRequired(Message = "旧密码不能为空！")]
    public string OldPassword { get; set; }

    [ApeVoloRequired(Message = "新密码不能为空！")]
    public string NewPassword { get; set; }
}