namespace ApeVolo.Common.Global
{
    public class GlobalVar
    {

        static GlobalVar()
        {
        }


        #region 运行

        public static readonly string CurrentDbConnId = AppSettings.GetValue(new[] {"MainDB"});
        
        

        #endregion

        #region 安全相关

        public const string JwtTokenPrefix = "Bearer ";

        /// <summary>
        /// 权限策略名称
        /// </summary>
        public const string AuthPolicysName = "Permission";

        /// <summary>
        /// 路由前缀
        /// </summary>
        public const string RoutePrefixName = "";

        #endregion
    }
}