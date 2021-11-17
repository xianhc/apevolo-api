using ApeVolo.Common.AttributeExt;

namespace ApeVolo.IBusiness.QueryModel
{
    public class LoginAuthUser
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [ApeVoloRequired(Message = "用户名不能为空！")]
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [ApeVoloRequired(Message = "密码不能为空！")]
        public string Password { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [ApeVoloRequired(Message = "验证码不能为空！")]
        public string Code { get; set; }

        /// <summary>
        /// 唯一ID，这里使用雪花ID
        /// </summary>
        [ApeVoloRequired(Message = "UUID不能为空！")]
        public string Uuid { get; set; }
    }
}
