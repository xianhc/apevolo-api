using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ape.Volo.Api.Serilog;
using Ape.Volo.Common;
using Ape.Volo.Common.Caches.SqlSugar;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.Helper.Serilog;
using Ape.Volo.Common.Model;
using Ape.Volo.Common.SnowflakeIdHelper;
using Ape.Volo.Entity.Base;
using Ape.Volo.IBusiness.Interface.Permission;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SqlSugar;
using StackExchange.Profiling;
using MiniProfiler = StackExchange.Profiling.MiniProfiler;

namespace Ape.Volo.Api.Extensions;

/// <summary>
/// SqlSugar 启动器
/// </summary>
public static class SqlSugarSetup
{
    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(SqlSugarSetup));

    public static void AddSqlSugarSetup(this IServiceCollection services)
    {
        if (services.IsNull())
            throw new ArgumentNullException(nameof(services));

        var settingsOptions = App.GetOptions<SettingsOptions>();
        var options = App.GetOptions<DataConnectionOptions>();
        if (options.ConnectionItem.Count == 0)
        {
            throw new Exception("请确保配置数据库配置DataConnection无误;");
        }

        // var connectionMaster =
        //     dataConnection.ConnectionItem.Where(x => x.ConnId == configs.DefaultDataBase && x.Enabled).ToList();
        var allConnectionItem =
            options.ConnectionItem.Where(x => x.Enabled).ToList();
        if (allConnectionItem.Count == 0 || allConnectionItem.All(x => x.ConnId != settingsOptions.DefaultDataBase))
        {
            throw new Exception($"请确保主库ID:{settingsOptions.DefaultDataBase}的Enabled为true;");
        }

        if (allConnectionItem.All(x => x.ConnId != settingsOptions.LogDataBase))
        {
            throw new Exception($"请确保日志库ID:{settingsOptions.LogDataBase}的Enabled为true;");
        }


        List<ConnectionConfig> allConnectionConfig = new List<ConnectionConfig>();
        List<SlaveConnectionConfig> slaveDbs = null; //从库列表

        foreach (var connectionItem in allConnectionItem)
        {
            if (connectionItem.DbType == DbType.Sqlite)
            {
                connectionItem.ConnectionString = "DataSource=" + Path.Combine(App.WebHostEnvironment.ContentRootPath,
                    connectionItem.ConnectionString ?? string.Empty);
            }

            List<ConnectionItem> connectionSlaves = new List<ConnectionItem>();
            if (settingsOptions.IsCqrs)
            {
                connectionSlaves = options.ConnectionItem
                    .Where(x => x.DbType == connectionItem.DbType && x.ConnId != connectionItem.ConnId && x.Enabled)
                    .ToList();
                if (!connectionSlaves.Any())
                {
                    throw new Exception($"请确保数据库ID:{connectionItem.ConnId}对应的从库的Enabled为true;");
                }
            }

            if (settingsOptions.IsCqrs)
            {
                slaveDbs = new List<SlaveConnectionConfig>();
                connectionSlaves.ForEach(db =>
                {
                    slaveDbs.Add(new SlaveConnectionConfig
                    {
                        HitRate = db.HitRate,
                        ConnectionString = db.ConnectionString
                    });
                });
            }

            var masterDb = new ConnectionConfig
            {
                ConfigId = connectionItem.ConnId,
                ConnectionString = connectionItem.ConnectionString,
                DbType = connectionItem.DbType,
                LanguageType = LanguageType.Chinese,
                IsAutoCloseConnection = true,
                //IsShardSameThread = false,
                MoreSettings = new ConnMoreSettings
                {
                    IsAutoRemoveDataCache = true,
                    SqlServerCodeFirstNvarchar = true //sqlserver默认使用nvarchar
                },
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    DataInfoCacheService = App.GetOptions<CacheOptions>().RedisCacheSwitch.Enabled
                        ? new SqlSugarRedisCache()
                        : new SqlSugarDistributedCache(),
                    EntityService = (c, p) =>
                    {
                        p.DbColumnName = UtilMethods.ToUnderLine(p.DbColumnName); //字段使用驼峰转下划线，不需要请注释
                        if (connectionItem.DbType == DbType.MySql && p.DataType == "varchar(max)")
                        {
                            p.DataType = "longtext";
                        }

                        /***低版本C#写法***/
                        // int?  decimal?这种 isnullable=true 不支持string(下面.NET 7支持)
                        if (p.IsPrimarykey == false && c.PropertyType.IsGenericType &&
                            c.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            p.IsNullable = true;
                        }

                        /***高版C#写法***/
                        //支持string?和string  
                        // if (p.IsPrimarykey == false && new NullabilityInfoContext()
                        //         .Create(c).WriteState is NullabilityState.Nullable)
                        // {
                        //     p.IsNullable = true;
                        // }
                    },
                },
                // 从库
                SlaveConnectionConfigs = slaveDbs
            }; //主库
            allConnectionConfig.Add(masterDb);
        }

        var sugar = new SqlSugarScope(allConnectionConfig,
            db =>
            {
                allConnectionConfig.Where(x => x.ConfigId.ToString() != settingsOptions.LogDataBase).ForEach(
                    config =>
                    {
                        var sugarScopeProvider = db.GetConnectionScope((string)config.ConfigId);

                        #region 配置过滤器

                        //软删除
                        sugarScopeProvider.ConfiguringSoftDeletedFilter();

                        //租户
                        sugarScopeProvider.ConfiguringTenantFilter();

                        //数据权限  这个要放在最后
                        sugarScopeProvider.ConfiguringUserDataScopeFilter();

                        #endregion

                        #region 读写事件

                        sugarScopeProvider.Aop.DataExecuting = DataExecuting;

                        #endregion

                        #region 日志

                        sugarScopeProvider.Aop.OnLogExecuting = (sql, pars) => OnLogExecuting(sugarScopeProvider,
                            Enum.GetName(typeof(SugarActionType), sugarScopeProvider.SugarActionType), sql, pars,
                            config);

                        #endregion

                        #region 耗时

                        sugarScopeProvider.Aop.OnLogExecuted = (_, _) => OnLogExecuted(sugarScopeProvider.Ado);

                        #endregion
                    });
            });
        services.AddSingleton<ISqlSugarClient>(sugar);
    }


    #region 读写事件

    /// <summary>
    /// 读写事件
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entityInfo"></param>
    private static void DataExecuting(object value, DataFilterModel entityInfo)
    {
        if (entityInfo.EntityValue is RootKey<long> { Id: 0 } rootEntity)
        {
            rootEntity.Id = IdHelper.GetLongId();
        }

        #region BaseEntity

        if (entityInfo.EntityValue is BaseEntity baseEntity)
        {
            switch (entityInfo.OperationType)
            {
                case DataFilterType.InsertByObject:
                {
                    if (baseEntity.CreateTime == DateTime.MinValue)
                    {
                        baseEntity.CreateTime = DateTime.Now;
                    }

                    break;
                }
                case DataFilterType.UpdateByObject:
                    baseEntity.UpdateTime = DateTime.Now;
                    break;
            }

            if (App.HttpUser.IsNotNull() && !App.HttpUser.Account.IsNullOrEmpty())
            {
                switch (entityInfo.OperationType)
                {
                    case DataFilterType.InsertByObject:
                    {
                        if (baseEntity.CreateBy.IsNullOrEmpty())
                        {
                            baseEntity.CreateBy = App.HttpUser.Account;
                        }

                        var tenant = baseEntity as ITenantEntity;
                        if (tenant != null && App.HttpUser.TenantId > 0)
                        {
                            if (tenant.TenantId == 0)
                            {
                                tenant.TenantId = App.HttpUser.TenantId;
                            }
                        }

                        break;
                    }
                    case DataFilterType.UpdateByObject:
                        baseEntity.UpdateBy = App.HttpUser.Account;
                        break;
                }
            }
        }

        #endregion

        #region BaseEntityNoDataScope

        if (entityInfo.EntityValue is BaseEntityNoDataScope baseEntityNoDataScope)
        {
            switch (entityInfo.OperationType)
            {
                case DataFilterType.InsertByObject:
                {
                    if (baseEntityNoDataScope.CreateTime == DateTime.MinValue)
                    {
                        baseEntityNoDataScope.CreateTime = DateTime.Now;
                    }

                    break;
                }
                case DataFilterType.UpdateByObject:
                    baseEntityNoDataScope.UpdateTime = DateTime.Now;
                    break;
            }

            if (App.HttpUser.IsNotNull() && !App.HttpUser.Account.IsNullOrEmpty())
            {
                switch (entityInfo.OperationType)
                {
                    case DataFilterType.InsertByObject:
                    {
                        if (baseEntityNoDataScope.CreateBy.IsNullOrEmpty())
                        {
                            baseEntityNoDataScope.CreateBy = App.HttpUser.Account;
                        }

                        var tenant = baseEntityNoDataScope as ITenantEntity;
                        if (tenant != null && App.HttpUser.TenantId > 0)
                        {
                            if (tenant.TenantId == 0)
                            {
                                tenant.TenantId = App.HttpUser.TenantId;
                            }
                        }

                        break;
                    }
                    case DataFilterType.UpdateByObject:
                        baseEntityNoDataScope.UpdateBy = App.HttpUser.Account;
                        break;
                }
            }
        }

        #endregion
    }

    #endregion

    #region 日志

    private static void OnLogExecuting(ISqlSugarClient sqlSugar, string operate, string sql,
        SugarParameter[] pars, ConnectionConfig connection)
    {
        try
        {
            var sqlLogOptions = App.GetOptions<SqlLogOptions>();
            var settingsOptions = App.GetOptions<SettingsOptions>();
            var middlewareOptions = App.GetOptions<MiddlewareOptions>();
            if (!sqlLogOptions.Enabled)
            {
                return;
            }

            if (settingsOptions.IsQuickDebug && middlewareOptions.MiniProfiler.Enabled)
            {
                MiniProfiler.Current.CustomTiming("SQL",
                    "【SQL参数】:\n" + GetParams(pars) + "【SQL语句】：\n" + sql);
            }

            if (sqlLogOptions.ToDb.Enabled || sqlLogOptions.ToFile.Enabled ||
                sqlLogOptions.ToConsole.Enabled)
            {
                using (LoggerPropertyConfiguration.Create.AddAopSqlProperty(sqlSugar, sqlLogOptions))
                {
                    Log.Information(
                        "Executed Sql--> User:[{User}] Operate:[{Operate}] ConnId:[{ConnId}] {Sql}",
                        App.HttpUser.Account, operate, connection.ConfigId, UtilMethods.GetNativeSql(sql, pars));
                }
            }
        }
        catch (Exception e)
        {
            Log.Fatal("Error occured OnLogExecuting:" + e);
        }
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

    private static void OnLogExecuted(IAdo ado)
    {
        var sqlLogOptions = App.GetOptions<SqlLogOptions>();
        var settingsOptions = App.GetOptions<SettingsOptions>();
        var middlewareOptions = App.GetOptions<MiddlewareOptions>();

        if (!sqlLogOptions.Enabled)
        {
            return;
        }

        if (settingsOptions.IsQuickDebug && middlewareOptions.MiniProfiler.Enabled)
        {
            MiniProfiler.Current.CustomTiming("SQL",
                $"【Sql耗时】:{Math.Round(ado.SqlExecutionTime.TotalMilliseconds / 1000d, 4)}秒\r\n");
        }

        if (sqlLogOptions.ToConsole.Enabled)
        {
            if (ado.SqlExecutionTime.TotalMilliseconds > 5000)
            {
                ConsoleHelper.WriteLine($"[Time]:{Math.Round(ado.SqlExecutionTime.TotalMilliseconds / 1000d, 4)}秒",
                    ConsoleColor.DarkCyan);
                ConsoleHelper.WriteLine($"[提示]:当前sql执行耗时较长,请检查进行优化\r\n",
                    ConsoleColor.Red);
            }
            else
            {
                ConsoleHelper.WriteLine($"[Time]:{Math.Round(ado.SqlExecutionTime.TotalMilliseconds / 1000d, 4)}秒\r\n",
                    ConsoleColor.DarkCyan);
            }
        }
    }

    #endregion


    /// <summary>
    /// 配置软删除过滤器
    /// </summary>
    private static void ConfiguringSoftDeletedFilter(this SqlSugarScopeProvider db)
    {
        db.QueryFilter.AddTableFilter<ISoftDeletedEntity>(it => it.IsDeleted == false);
    }

    /// <summary>
    /// 配置多租户过滤器
    /// </summary>
    private static void ConfiguringTenantFilter(this SqlSugarScopeProvider db)
    {
        if (App.HttpUser.IsNotNull() && App.HttpUser.TenantId > 0)
        {
            db.QueryFilter.AddTableFilter<ITenantEntity>(it => it.TenantId == App.HttpUser.TenantId);
        }
    }

    /// <summary>
    /// 配置用户数据权限
    /// </summary>
    /// <param name="db"></param>
    private static void ConfiguringUserDataScopeFilter(this SqlSugarScopeProvider db)
    {
        if (App.HttpUser.IsNull() || App.HttpUser.Account.IsNullOrEmpty()) return;
        var dataScopeService = App.GetService<IDataScopeService>();
        if (dataScopeService == null) return;

        try
        {
            var accounts = AsyncHelper.RunSync(() =>
                dataScopeService.GetDataScopeAccountsAsync(App.HttpUser.Id));
            if (accounts.Count > 0)
            {
                if (accounts.Count == 1)
                {
                    if (!accounts[0].Equals("All"))
                    {
                        db.QueryFilter.AddTableFilter<ICreateByEntity>(it => it.CreateBy == accounts[0]);
                    }
                }
                else
                {
                    db.QueryFilter.AddTableFilter<ICreateByEntity>(it => accounts.Contains(it.CreateBy));
                }
            }
            else
            {
                db.QueryFilter.AddTableFilter<ICreateByEntity>(it => it.CreateBy == App.HttpUser.Account);
            }
        }
        catch (Exception e)
        {
            Logger.Fatal("配置用户数据权限错误：\r\n" + ExceptionHelper.GetExceptionAllMsg(e));
            db.QueryFilter.AddTableFilter<ICreateByEntity>(it => it.CreateBy == App.HttpUser.Account);
        }
    }
}
