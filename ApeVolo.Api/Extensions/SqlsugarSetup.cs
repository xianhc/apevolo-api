using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApeVolo.Common.DB;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper;
using ApeVolo.Entity.Do.Core;
using ApeVolo.Entity.Do.Dictionary;
using ApeVolo.Entity.Do.Email;
using ApeVolo.Entity.Do.Logs;
using ApeVolo.Entity.Do.Queued;
using ApeVolo.Entity.Do.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using StackExchange.Profiling;

namespace ApeVolo.Api.Extensions;

/// <summary>
/// SqlSugar 启动器
/// </summary>
public static class SqlsugarSetup
{
    public static void AddSqlsugarSetup(this IServiceCollection services)
    {
        if (services.IsNull())
            throw new ArgumentNullException(nameof(services));
        if (BaseDbConfig.GetDataBaseOperate.MasterDb.IsNull())
            throw new ArgumentNullException(nameof(services));


        ConnectionConfig masterDb = null; //主库
        var slaveDbs = new List<SlaveConnectionConfig>(); //从库列表
        BaseDbConfig.GetDataBaseOperate.SlaveDbs.ForEach(db =>
        {
            slaveDbs.Add(new SlaveConnectionConfig
            {
                HitRate = db.HitRate,
                ConnectionString = db.ConnectionString
            });
        });
        masterDb = new ConnectionConfig
        {
            ConfigId = BaseDbConfig.GetDataBaseOperate.MasterDb.ConnId.ToLower(),
            ConnectionString = BaseDbConfig.GetDataBaseOperate.MasterDb.ConnectionString,
            DbType = (DbType)BaseDbConfig.GetDataBaseOperate.MasterDb.DbType,
            LanguageType = LanguageType.Chinese,
            IsAutoCloseConnection = true,
            //IsShardSameThread = false,
            MoreSettings = new ConnMoreSettings
            {
                IsAutoRemoveDataCache = true
            },
            // 从库
            SlaveConnectionConfigs = slaveDbs
        };

        var sugar = new SqlSugarScope(masterDb,
            config =>
            {
                #region 条件过滤器

                config.QueryFilter.Add(new TableFilterItem<User>(it => !it.IsDeleted));
                config.QueryFilter.Add(new TableFilterItem<UserJobs>(it => !it.IsDeleted));
                config.QueryFilter.Add(new TableFilterItem<UserRoles>(it => !it.IsDeleted));

                config.QueryFilter.Add(new TableFilterItem<Department>(it => !it.IsDeleted));
                config.QueryFilter.Add(new TableFilterItem<Job>(it => !it.IsDeleted));
                config.QueryFilter.Add(new TableFilterItem<Menu>(it => !it.IsDeleted));

                config.QueryFilter.Add(new TableFilterItem<Role>(it => !it.IsDeleted));
                config.QueryFilter.Add(new TableFilterItem<RoleMenu>(it => !it.IsDeleted));
                config.QueryFilter.Add(new TableFilterItem<RolesDepartments>(it => !it.IsDeleted));

                config.QueryFilter.Add(new TableFilterItem<Setting>(it => !it.IsDeleted));
                config.QueryFilter.Add(new TableFilterItem<FileRecord>(it => !it.IsDeleted));
                config.QueryFilter.Add(new TableFilterItem<AppSecret>(it => !it.IsDeleted));

                config.QueryFilter.Add(new TableFilterItem<Dict>(it => !it.IsDeleted));
                config.QueryFilter.Add(new TableFilterItem<DictDetail>(it => !it.IsDeleted));

                config.QueryFilter.Add(new TableFilterItem<EmailAccount>(it => !it.IsDeleted));
                config.QueryFilter.Add(new TableFilterItem<EmailMessageTemplate>(it => !it.IsDeleted));

                config.QueryFilter.Add(new TableFilterItem<QueuedEmail>(it => !it.IsDeleted));

                config.QueryFilter.Add(new TableFilterItem<QuartzNet>(it => !it.IsDeleted));
                config.QueryFilter.Add(new TableFilterItem<QuartzNetLog>(it => !it.IsDeleted));

                config.QueryFilter.Add(new TableFilterItem<AuditLog>(it => !it.IsDeleted));
                config.QueryFilter.Add(new TableFilterItem<ExceptionLog>(it => !it.IsDeleted));

                config.QueryFilter.Add(new TableFilterItem<TestApeVolo>(it => !it.IsDeleted));

                #endregion

                #region 执行的SQL

                config.Aop.OnLogExecuting = (sql, pars) =>
                {
                    if (AppSettings.GetValue<bool>("IsMiniProfiler"))
                    {
                        MiniProfiler.Current.CustomTiming("SQL",
                            "【SQL参数】：\n" + GetParams(pars) + "【SQL语句】：\n" + sql);
                    }

                    if (AppSettings.GetValue<bool>("IsSqlAOP"))
                    {
                        LogHelper.WriteSqlLog($"SqlLog{DateTime.Now:yyyy-MM-dd}",
                            new[] { "【SQL参数】：\n" + GetParams(pars), "【SQL语句】：\n" + sql });
                    }
                };

                #endregion

                //OnLogExecuted = //执行完毕
            });
        services.AddSingleton<ISqlSugarClient>(sugar);
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