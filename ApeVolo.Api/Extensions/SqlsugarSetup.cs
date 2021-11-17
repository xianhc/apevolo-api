using ApeVolo.Common.DB;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApeVolo.Api.Extensions
{
    /// <summary>
    /// SqlSugar 启动器
    /// </summary>
    public static class SqlsugarSetup
    {
        public static void AddSqlsugarSetup(this IServiceCollection services)
        {
            if (services.IsNull())
                throw new ArgumentNullException(nameof(services));

            // 默认添加主数据库连接
            DatabaseEntry.CurrentDbConnId = AppSettings.GetValue(new[] {"MainDB"});

            // 把多个连接对象注入服务，这里必须采用Scope，因为有事务操作
            services.AddScoped<ISqlSugarClient>(_ =>
            {
                ConnectionConfig dbConfig = null;
                if (BaseDbConfig.GetDataBaseOperate.IsNotNull())
                {
                    dbConfig = new ConnectionConfig()
                    {
                        ConfigId = BaseDbConfig.GetDataBaseOperate.ConnId.ToString().ToLower(),
                        ConnectionString = BaseDbConfig.GetDataBaseOperate.Conn,
                        DbType = (DbType) BaseDbConfig.GetDataBaseOperate.DbType,
                        IsAutoCloseConnection = true,
                        IsShardSameThread = false,
                        AopEvents = new AopEvents
                        {
                            OnLogExecuting = (sql, pars) => //执行前
                            {
                                Parallel.For(0, 1, _ =>
                                {
                                    if (AppSettings.GetValue("IsMiniProfiler").ToBool())
                                    {
                                        MiniProfiler.Current.CustomTiming("SQL",
                                            "【SQL参数】：\n" + GetParams(pars) + "【SQL语句】" + sql);
                                    }

                                    if (AppSettings.GetValue("IsSqlAOP").ToBool())
                                    {
                                        LogHelper.WriteSqlLog($"SqlLog{DateTime.Now:yyyy-MM-dd}",
                                            new[] {"【SQL参数】：\n" + GetParams(pars), "【SQL语句】" + sql});
                                    }
                                });
                            }
                            //OnLogExecuted = //执行完毕
                        },
                        MoreSettings = new ConnMoreSettings()
                        {
                            IsAutoRemoveDataCache = true
                        }
                    };
                }

                return new SqlSugarClient(dbConfig);
            });
        }

        /// <summary>
        /// 参数拼接字符串
        /// </summary>
        /// <param name="pars"></param>
        /// <returns></returns>
        private static string GetParams(SugarParameter[] pars)
        {
            return pars.Aggregate("", (current, p) => current + $"{p.ParameterName}:{p.Value}\n");
        }
    }
}