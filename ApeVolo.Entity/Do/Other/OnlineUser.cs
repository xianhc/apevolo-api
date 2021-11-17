using System;

namespace ApeVolo.Entity.Do.Other
{
    /// <summary>
    /// 在线用户
    /// </summary>
    public class OnlineUser
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 用户部门
        /// </summary>
        public string Dept { get; set; }
        /// <summary>
        /// 浏览器
        /// </summary>
        public string Browser { get; set; }
        /// <summary>
        /// 请求IP
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// IP详细地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 在线唯一表示KEY
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }
        /// <summary>
        /// 当前权限信息
        /// </summary>
        public CurrentPermission currentPermission { get; set; }
    }
}