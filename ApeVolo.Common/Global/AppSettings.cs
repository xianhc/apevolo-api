using System;
using System.Linq;
using ApeVolo.Common.DI;
using Microsoft.Extensions.Configuration;

namespace ApeVolo.Common.Global
{
    /// <summary>
    /// 配置文件帮助类
    /// </summary>
    public static class AppSettings
    {
        static AppSettings()
        {
            IConfiguration config = null;
            try
            {
                config = AutofacHelper.GetScopeService<IConfiguration>();
            }
            catch
            {
                // ignored
            }

            if (config == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json");

                config = builder.Build();
            }

            Config = config;
        }

        private static IConfiguration Config { get; }

        /// <summary>
        /// 从AppSettings获取key的值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            try
            {
                return Config[key];
            }
            catch (System.Exception)
            {
                // ignored
            }

            return "";
        }

        /// <summary>
        /// 从AppSettings获取key的值
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string GetValue(params string[] keys)
        {
            try
            {
                if (keys.Any())
                {
                    return Config[string.Join(":", keys)];
                }
            }
            catch (System.Exception)
            {
                // ignored
            }

            return "";
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="nameOfCon">连接字符串名</param>
        /// <returns></returns>
        public static string GetConnectionString(string nameOfCon)
        {
            return Config.GetConnectionString(nameOfCon);
        }
    }
}