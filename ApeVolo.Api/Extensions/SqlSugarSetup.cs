using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApeVolo.Common.ConfigOptions;
using ApeVolo.Common.DI;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model;
using ApeVolo.Common.SnowflakeIdHelper;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Base;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using StackExchange.Profiling;
using MiniProfiler = StackExchange.Profiling.MiniProfiler;

namespace ApeVolo.Api.Extensions;

/// <summary>
/// SqlSugar 启动器
/// </summary>
public static class SqlSugarSetup
{
    public static void AddSqlSugarSetup(this IServiceCollection services, Configs configs)
    {
        if (services.IsNull())
            throw new ArgumentNullException(nameof(services));
        var dataConnection = configs.DataConnection;
        if (!dataConnection.ConnectionItem.Any())
        {
            throw new Exception("请确保配置数据库配置DataConnection无误;");
        }

        var connectionMaster =
            dataConnection.ConnectionItem.FirstOrDefault(x => x.ConnId == configs.DefaultDataBase && x.Enabled);
        if (connectionMaster == null)
        {
            throw new Exception($"请确保数据库ID:{configs.DefaultDataBase}的Enabled为true;");
        }

        if (connectionMaster.DbType == (int)DataBaseType.Sqlite)
        {
            connectionMaster.ConnectionString = "DataSource=" + Path.Combine(AppSettings.ContentRootPath,
                connectionMaster.ConnectionString ?? string.Empty);
        }

        List<ConnectionItem> connectionSlaves = new List<ConnectionItem>();
        if (configs.IsCqrs)
        {
            connectionSlaves = dataConnection.ConnectionItem
                .Where(x => x.DbType == connectionMaster.DbType && x.ConnId != configs.DefaultDataBase && x.Enabled)
                .ToList();
            if (!connectionSlaves.Any())
            {
                throw new Exception($"请确保数据库ID:{configs.DefaultDataBase}对应的从库的Enabled为true;");
            }
        }

        ConnectionConfig masterDb = null; //主库
        var slaveDbs = new List<SlaveConnectionConfig>(); //从库列表
        if (configs.IsCqrs)
        {
            connectionSlaves.ForEach(db =>
            {
                slaveDbs.Add(new SlaveConnectionConfig
                {
                    HitRate = db.HitRate,
                    ConnectionString = db.ConnectionString
                });
            });
        }

        masterDb = new ConnectionConfig
        {
            ConfigId = connectionMaster.ConnId.ToLower(),
            ConnectionString = connectionMaster.ConnectionString,
            DbType = (DbType)connectionMaster.DbType,
            LanguageType = LanguageType.Chinese,
            IsAutoCloseConnection = true,
            //IsShardSameThread = false,
            MoreSettings = new ConnMoreSettings
            {
                IsAutoRemoveDataCache = true,
                SqlServerCodeFirstNvarchar = true, //sqlserver默认使用nvarchar
            },
            ConfigureExternalServices = new ConfigureExternalServices
            {
                EntityService = (c, p) =>
                {
                    p.DbColumnName = UtilMethods.ToUnderLine(p.DbColumnName); //字段使用驼峰转下划线，不需要请注释
                    if ((DbType)connectionMaster.DbType == DbType.MySql && p.DataType == "varchar(max)")
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
        };

        var sugar = new SqlSugarScope(masterDb,
            config =>
            {
                #region 接口过滤器

                config.QueryFilter.AddTableFilter<ISoftDeletedEntity>(x => x.IsDeleted == false);

                #endregion

                #region 读写事件

                config.Aop.DataExecuting = DataExecuting;

                #endregion

                #region 日志

                config.Aop.OnLogExecuting = (sql, pars) => OnLogExecuting(sql, pars, configs);

                #endregion

                #region 耗时

                config.Aop.OnLogExecuted =
                    (sql, pars) => OnLogExecuted(sql, pars, configs, config.Ado);

                #endregion
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

        if (entityInfo.EntityValue is not BaseEntity baseEntity) return;
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
                if (baseEntity.UpdateTime == DateTime.MinValue)
                {
                    baseEntity.UpdateTime = DateTime.Now;
                }

                break;
            case DataFilterType.DeleteByObject:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var httpUser = AutofacHelper.GetScopeService<IHttpUser>();
        if (!httpUser.IsNotNull()) return;
        switch (entityInfo.OperationType)
        {
            case DataFilterType.InsertByObject:
            {
                if (baseEntity.CreateBy.IsNullOrEmpty())
                {
                    baseEntity.CreateBy = httpUser.Account;
                }

                break;
            }
            case DataFilterType.UpdateByObject:
                if (baseEntity.UpdateBy.IsNullOrEmpty())
                {
                    baseEntity.UpdateBy = httpUser.Account;
                }

                break;
            case DataFilterType.DeleteByObject:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion

    #region 日志

    private static void OnLogExecuting(string sql, SugarParameter[] pars, Configs configs)
    {
        if (sql.Contains("log_audit") || sql.Contains("log_exception"))
        {
            return;
        }

        if (configs.IsMiniProfiler)
        {
            MiniProfiler.Current.CustomTiming("SQL",
                "【SQL参数】：\n" + GetParams(pars) + "【SQL语句】：\n" + sql);
        }

        if (configs.IsSqlLog)
        {
            LogHelper.WriteSqlLog($"SqlLog{DateTime.Now:yyyy-MM-dd}",
                new[] { "【SQL参数】：\n" + GetParams(pars), "【SQL语句】：\n" + sql });
        }

        if (configs.IsOutSqlToConsole)
        {
            ConsoleHelper.WriteLine($"{DateTime.Now}\n【SQL参数】：\n{GetParams(pars)}【SQL语句】：\n{sql}");
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

    private static void OnLogExecuted(string sql, SugarParameter[] pars, Configs configs, IAdo ado)
    {
        if (sql.Contains("log_audit") || sql.Contains("log_exception"))
        {
            return;
        }

        if (configs.IsMiniProfiler)
        {
            MiniProfiler.Current.CustomTiming("SQL", $"【SQL耗时】：:{ado.SqlExecutionTime.TotalMilliseconds}毫秒");
        }

        if (configs.IsSqlLog)
        {
            LogHelper.WriteSqlLog($"SqlLog{DateTime.Now:yyyy-MM-dd}",
                new[] { $"【SQL耗时】：{ado.SqlExecutionTime.TotalMilliseconds}毫秒" });
        }

        if (configs.IsOutSqlToConsole)
        {
            ConsoleHelper.WriteLine($"【SQL耗时】：{ado.SqlExecutionTime.TotalMilliseconds}毫秒");
        }

        if (ado.SqlExecutionTime.TotalSeconds > 1)
        {
            //大于1秒
        }
    }

    #endregion
}
