using System;

namespace ApeVolo.Common.Global
{
    /// <summary>
    /// redis相关的KEY
    /// </summary>
    public static class RedisKey
    {
        /// <summary>
        /// redis默认缓存时间
        /// </summary>
        public static readonly TimeSpan RedisTime = new TimeSpan(0, 0, 20, 0);

        /// <summary>
        /// 在线
        /// </summary>
        public const string OnlineKey = "OnlineToken:";

        /// <summary>
        /// 验证码
        /// </summary>
        public const string CodeKey = "CodeKey:";
        /// <summary>
        /// 邮箱验证码
        /// </summary>
        public const string EmailCaptchaKey = "EmailCaptcha:";
        /// <summary>
        /// 加载用户信息
        /// </summary>
        public const string UserInfoByName = "user:info:name:";

        public const string UserInfoById = "user:info:id:";

        public const string UserRolesById = "user:roles:id:";

        public const string UserJobsById = "user:jobs:id:";

        public const string UserBuildMenuById = "user:build:menu:id:";

        public const string UserPermissionById = "user:permission:id:";

        public const string LoadMenusByPId = "menu:pid:";

        public const string LoadMenusById = "menu:id:";

        public const string LoadSettingByName = "setting:name:";
    }
}