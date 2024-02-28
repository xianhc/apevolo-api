using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Helper;
using Ape.Volo.Entity.Base;
using Ape.Volo.Entity.Message.Email;
using Ape.Volo.Entity.Monitor;
using Ape.Volo.Entity.Permission;
using Ape.Volo.Entity.System;
using Newtonsoft.Json;
using SqlSugar;

namespace Ape.Volo.Entity.Seed;

/// <summary>
/// 
/// </summary>
public class DataSeeder
{
    /// <summary>
    /// 异步添加种子数据
    /// </summary>
    /// <param name="dataContext"></param>
    /// <param name="isInitData"></param>
    /// <param name="isQuickDebug"></param>
    /// <returns></returns>
    public static async Task InitSystemDataAsync(DataContext dataContext, bool isInitData, bool isQuickDebug)
    {
        try
        {
            ConsoleHelper.WriteLine($"程序正在启动....", ConsoleColor.Green);
            ConsoleHelper.WriteLine($"是否开发环境: {isQuickDebug}");
            ConsoleHelper.WriteLine($"ContentRootPath: {AppSettings.ContentRootPath}");
            ConsoleHelper.WriteLine($"WebRootPath: {AppSettings.WebRootPath}");
            ConsoleHelper.WriteLine($"Master Db Id: {dataContext.Db.CurrentConnectionConfig.ConfigId}");
            ConsoleHelper.WriteLine($"Master Db Type: {dataContext.Db.CurrentConnectionConfig.DbType}");
            ConsoleHelper.WriteLine(
                $"Master Db ConnectString: {dataContext.Db.CurrentConnectionConfig.ConnectionString}");
            ConsoleHelper.WriteLine("初始化主库....");
            if (dataContext.DbType != DbType.Oracle)
            {
                dataContext.Db.DbMaintenance.CreateDatabase();
            }
            else
            {
                throw new Exception("sqlsugar官方表示Oracle不支持代码建库，请先建库再启动项目");
            }

            ConsoleHelper.WriteLine("初始化主库成功。", ConsoleColor.Green);
            ConsoleHelper.WriteLine("初始化主库数据表....");

            #region 初始化主库数据表

            //继承自BaseEntity或者RootKey<>的类型
            //一些没有继承的需手动维护添加
            //例如，用户与岗位(UserJobs)
            var entityList = GlobalData.GetEntityAssembly().GetTypes()
                .Where(x => (x.BaseType == typeof(BaseEntity) || x.BaseType == typeof(RootKey<long>)) &&
                            x != typeof(BaseEntity) && x.Namespace != null &&
                            !x.Namespace.StartsWith("Ape.Volo.Entity.Monitor")).ToList();
            entityList.Add(typeof(UserRole));
            entityList.Add(typeof(UserJob));
            entityList.Add(typeof(RoleMenu));
            entityList.Add(typeof(RoleDepartment));
            entityList.Add(typeof(RoleApis));

            var masterTables = dataContext.Db.DbMaintenance.GetTableInfoList();
            entityList.ForEach(entity =>
            {
                var entityInfo = dataContext.Db.EntityMaintenance.GetEntityInfo(entity);
                // var attr = entity.GetCustomAttribute<SugarTable>();
                // var tableName = attr == null ? entity.Name : attr.TableName;
                if (entityInfo.DbTableName.IsNullOrEmpty())
                {
                    throw new Exception($"类{entityInfo.EntityName}缺少SugarTable表名");
                }

                if (!masterTables.Any(x => x.Name.Contains(entityInfo.DbTableName)))
                {
                    if (entity.GetCustomAttribute<SplitTableAttribute>() != null)
                    {
                        dataContext.Db.CodeFirst.SplitTables().InitTables(entity);
                        ConsoleHelper.WriteLine(
                            $"Entity:{entity.Name}-->Table:{entityInfo.DbTableName}-->Desc:{entityInfo.TableDescription}-->创建完成！");
                    }
                    else
                    {
                        dataContext.Db.CodeFirst.InitTables(entity);
                        ConsoleHelper.WriteLine(
                            $"Entity:{entity.Name}-->Table:{entityInfo.DbTableName}-->Desc:{entityInfo.TableDescription}-->创建完成！");
                    }
                }
            });

            ConsoleHelper.WriteLine("初始化主库数据表成功！", ConsoleColor.Green);
            ConsoleHelper.WriteLine();

            #endregion

            #region 初始化主库数据

            if (isInitData)
            {
                ConsoleHelper.WriteLine("初始化主库种子数据....");
                JsonSerializerSettings setting = new JsonSerializerSettings();
                JsonConvert.DefaultSettings = () =>
                {
                    setting.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                    setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    setting.NullValueHandling = NullValueHandling.Ignore;
                    return setting;
                };
                string seedDataFolder = "resources/db/{0}.tsv";
                seedDataFolder = Path.Combine(AppSettings.WebRootPath, seedDataFolder);

                #region 用户

                if (!await dataContext.Db.Queryable<User>().AnyAsync())
                {
                    var attr = typeof(User).GetCustomAttribute<SugarTable>();
                    if (attr != null)
                    {
                        await dataContext.GetEntityDb<User>().InsertRangeAsync(
                            JsonConvert.DeserializeObject<List<User>>(
                                FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                setting));
                        ConsoleHelper.WriteLine(
                            $"Entity:{nameof(User)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！",
                            ConsoleColor.Green);
                    }
                }

                #endregion

                #region 角色

                if (!await dataContext.Db.Queryable<Role>().AnyAsync())
                {
                    var attr = typeof(Role).GetCustomAttribute<SugarTable>();
                    if (attr != null)
                    {
                        await dataContext.GetEntityDb<Role>().InsertRangeAsync(
                            JsonConvert.DeserializeObject<List<Role>>(
                                FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                setting));
                        ConsoleHelper.WriteLine(
                            $"Entity:{nameof(Role)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！",
                            ConsoleColor.Green);
                    }
                }

                #endregion

                #region 菜单

                if (!await dataContext.Db.Queryable<Menu>().AnyAsync())
                {
                    var attr = typeof(Menu).GetCustomAttribute<SugarTable>();
                    if (attr != null)
                    {
                        await dataContext.GetEntityDb<Menu>().InsertRangeAsync(
                            JsonConvert.DeserializeObject<List<Menu>>(
                                FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                setting));
                        ConsoleHelper.WriteLine(
                            $"Entity:{nameof(Menu)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！",
                            ConsoleColor.Green);
                    }
                }

                #endregion

                #region 部门

                if (!await dataContext.Db.Queryable<Department>().AnyAsync())
                {
                    var attr = typeof(Department).GetCustomAttribute<SugarTable>();
                    if (attr != null)
                    {
                        await dataContext.GetEntityDb<Department>().InsertRangeAsync(
                            JsonConvert.DeserializeObject<List<Department>>(
                                FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                setting));
                        ConsoleHelper.WriteLine(
                            $"Entity:{nameof(Department)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！",
                            ConsoleColor.Green);
                    }
                }

                #endregion

                #region 岗位

                if (!await dataContext.Db.Queryable<Job>().AnyAsync())
                {
                    var attr = typeof(Job).GetCustomAttribute<SugarTable>();
                    if (attr != null)
                    {
                        await dataContext.GetEntityDb<Job>().InsertRangeAsync(
                            JsonConvert.DeserializeObject<List<Job>>(
                                FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                setting));
                        ConsoleHelper.WriteLine(
                            $"Entity:{nameof(Job)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！",
                            ConsoleColor.Green);
                    }
                }

                #endregion

                #region 系统全局设置

                if (!await dataContext.Db.Queryable<Setting>().AnyAsync())
                {
                    var attr = typeof(Setting).GetCustomAttribute<SugarTable>();
                    if (attr != null)
                    {
                        await dataContext.GetEntityDb<Setting>().InsertRangeAsync(
                            JsonConvert.DeserializeObject<List<Setting>>(
                                FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                setting));
                        ConsoleHelper.WriteLine(
                            $"Entity:{nameof(Setting)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！",
                            ConsoleColor.Green);
                    }
                }

                #endregion

                #region 字典

                if (!await dataContext.Db.Queryable<Dict>().AnyAsync())
                {
                    var attr = typeof(Dict).GetCustomAttribute<SugarTable>();
                    if (attr != null)
                    {
                        await dataContext.GetEntityDb<Dict>().InsertRangeAsync(
                            JsonConvert.DeserializeObject<List<Dict>>(
                                FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                setting));
                        ConsoleHelper.WriteLine(
                            $"Entity:{nameof(Dict)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！",
                            ConsoleColor.Green);
                    }
                }

                #endregion

                #region 字典详情

                if (!await dataContext.Db.Queryable<DictDetail>().AnyAsync())
                {
                    var attr = typeof(DictDetail).GetCustomAttribute<SugarTable>();
                    if (attr != null)
                    {
                        await dataContext.GetEntityDb<DictDetail>().InsertRangeAsync(
                            JsonConvert.DeserializeObject<List<DictDetail>>(
                                FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                setting));
                        ConsoleHelper.WriteLine(
                            $"Entity:{nameof(DictDetail)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！",
                            ConsoleColor.Green);
                    }
                }

                #endregion

                #region 作业调度

                if (!await dataContext.Db.Queryable<QuartzNet>().AnyAsync())
                {
                    var attr = typeof(QuartzNet).GetCustomAttribute<SugarTable>();
                    if (attr != null)
                    {
                        await dataContext.GetEntityDb<QuartzNet>().InsertRangeAsync(
                            JsonConvert.DeserializeObject<List<QuartzNet>>(
                                FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                setting));
                        ConsoleHelper.WriteLine(
                            $"Entity:{nameof(QuartzNet)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！",
                            ConsoleColor.Green);
                    }
                }

                #endregion

                #region 邮箱账户

                if (!await dataContext.Db.Queryable<EmailAccount>().AnyAsync())
                {
                    var attr = typeof(EmailAccount).GetCustomAttribute<SugarTable>();
                    if (attr != null)
                    {
                        await dataContext.GetEntityDb<EmailAccount>().InsertRangeAsync(
                            JsonConvert.DeserializeObject<List<EmailAccount>>(
                                FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                setting));
                        ConsoleHelper.WriteLine(
                            $"Entity:{nameof(EmailAccount)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！",
                            ConsoleColor.Green);
                    }
                }

                #endregion

                #region 邮件模板

                if (!await dataContext.Db.Queryable<EmailMessageTemplate>().AnyAsync())
                {
                    var attr = typeof(EmailMessageTemplate).GetCustomAttribute<SugarTable>();
                    if (attr != null)
                    {
                        await dataContext.GetEntityDb<EmailMessageTemplate>().InsertRangeAsync(
                            JsonConvert.DeserializeObject<List<EmailMessageTemplate>>(
                                FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                setting));
                        ConsoleHelper.WriteLine(
                            $"Entity:{nameof(EmailMessageTemplate)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！",
                            ConsoleColor.Green);
                    }
                }

                #endregion

                #region 用户与角色

                if (!await dataContext.Db.Queryable<UserRole>().AnyAsync())
                {
                    var attr = typeof(UserRole).GetCustomAttribute<SugarTable>();
                    if (attr != null)
                    {
                        await dataContext.GetEntityDb<UserRole>().InsertRangeAsync(
                            JsonConvert.DeserializeObject<List<UserRole>>(
                                FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                setting));
                        ConsoleHelper.WriteLine(
                            $"Entity:{nameof(UserRole)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！",
                            ConsoleColor.Green);
                    }
                }

                #endregion

                #region 用户与岗位

                if (!await dataContext.Db.Queryable<UserJob>().AnyAsync())
                {
                    var attr = typeof(UserJob).GetCustomAttribute<SugarTable>();
                    if (attr != null)
                    {
                        await dataContext.GetEntityDb<UserJob>()
                            .InsertRangeAsync(JsonConvert.DeserializeObject<List<UserJob>>(
                                FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                setting));
                        ConsoleHelper.WriteLine(
                            $"Entity:{nameof(UserJob)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！",
                            ConsoleColor.Green);
                    }
                }

                #endregion

                #region 角色与菜单

                if (!await dataContext.Db.Queryable<RoleMenu>().AnyAsync())
                {
                    var attr = typeof(RoleMenu).GetCustomAttribute<SugarTable>();
                    if (attr != null)
                    {
                        await dataContext.GetEntityDb<RoleMenu>()
                            .InsertRangeAsync(JsonConvert.DeserializeObject<List<RoleMenu>>(
                                FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                setting));
                        ConsoleHelper.WriteLine(
                            $"Entity:{nameof(RoleMenu)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！",
                            ConsoleColor.Green);
                    }
                }

                #endregion

                #region Apis

                if (!await dataContext.Db.Queryable<Apis>().AnyAsync())
                {
                    var attr = typeof(Apis).GetCustomAttribute<SugarTable>();
                    if (attr != null)
                    {
                        await dataContext.GetEntityDb<Apis>().InsertRangeAsync(
                            JsonConvert.DeserializeObject<List<Apis>>(
                                FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                setting));
                        ConsoleHelper.WriteLine(
                            $"Entity:{nameof(Apis)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！",
                            ConsoleColor.Green);
                    }
                }

                #endregion

                #region 角色与Apis

                if (!await dataContext.Db.Queryable<RoleApis>().AnyAsync())
                {
                    var attr = typeof(RoleApis).GetCustomAttribute<SugarTable>();
                    if (attr != null)
                    {
                        await dataContext.GetEntityDb<RoleApis>().InsertRangeAsync(
                            JsonConvert.DeserializeObject<List<RoleApis>>(
                                FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                setting));
                        ConsoleHelper.WriteLine(
                            $"Entity:{nameof(RoleApis)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！",
                            ConsoleColor.Green);
                    }
                }

                #endregion

                ConsoleHelper.WriteLine("初始化数据完成！", ConsoleColor.Green);
            }

            #endregion

            #region 初始化日志库

            if (!dataContext.Db.IsAnyConnection(SqlSugarConfig.LogId))
            {
                throw new ApplicationException("未配置日志数据库，请在appsettings.json中DataConnection节点中配置");
            }

            var logDb = dataContext.Db.GetConnectionScope(SqlSugarConfig.LogId);
            ConsoleHelper.WriteLine($"Log Db Id: {logDb.CurrentConnectionConfig.ConfigId}");
            ConsoleHelper.WriteLine($"Log Db Type: {logDb.CurrentConnectionConfig.DbType}");
            ConsoleHelper.WriteLine($"Log Db ConnectString: {logDb.CurrentConnectionConfig.ConnectionString}");
            ConsoleHelper.WriteLine("初始化日志库成功。", ConsoleColor.Green);
            ConsoleHelper.WriteLine("初始化日志库数据表....");
            logDb.DbMaintenance.CreateDatabase();
            var logEntityList = GlobalData.GetEntityAssembly().GetTypes()
                .Where(x => x.IsClass && x != typeof(SerilogBase) && x.Namespace != null &&
                            x.Namespace.StartsWith("Ape.Volo.Entity.Monitor")).ToList();


            var logTables = logDb.DbMaintenance.GetTableInfoList();

            logEntityList.ForEach(entity =>
            {
                var entityInfo = dataContext.Db.EntityMaintenance.GetEntityInfo(entity);
                // var attr = entity.GetCustomAttribute<SugarTable>();
                // var tableName = attr == null ? entity.Name : attr.TableName;
                if (entityInfo.DbTableName.IsNullOrEmpty())
                {
                    throw new Exception($"类{entityInfo.EntityName}缺少SugarTable表名");
                }

                int lastUnderscoreIndex = entityInfo.DbTableName.LastIndexOf('_');
                var tableName = entityInfo.DbTableName.Substring(0, lastUnderscoreIndex);

                if (!logTables.Any(x => x.Name.Contains(tableName)))
                {
                    if (entity.GetCustomAttribute<SplitTableAttribute>() != null)
                    {
                        logDb.CodeFirst.SplitTables().InitTables(entity);
                        ConsoleHelper.WriteLine(
                            $"Entity:{entity.Name}-->Table:{entityInfo.DbTableName}-->Desc:{entityInfo.TableDescription}-->创建完成！");
                    }
                    else
                    {
                        logDb.CodeFirst.InitTables(entity);
                        ConsoleHelper.WriteLine(
                            $"Entity:{entity.Name}-->Table:{entityInfo.DbTableName}-->Desc:{entityInfo.TableDescription}-->创建完成！");
                    }
                }
            });
            ConsoleHelper.WriteLine("初始化日志库数据表成功！", ConsoleColor.Green);
            ConsoleHelper.WriteLine();

            #endregion

            ConsoleHelper.WriteLine("初始化完成，程序已启动....！", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
