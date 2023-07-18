using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ApeVolo.Common.DI;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper;
using ApeVolo.Entity.Base;
using ApeVolo.Entity.Message.Email;
using ApeVolo.Entity.Permission;
using ApeVolo.Entity.System;
using ApeVolo.Entity.Test;
using Newtonsoft.Json;
using SqlSugar;

namespace ApeVolo.Entity.Seed;

public class DataSeeder
{
    /// <summary>
    /// 异步添加种子数据
    /// </summary>
    /// <param name="dataContext"></param>
    /// <param name="isInitData"></param>
    /// <returns></returns>
    public static async Task InitSystemDataAsync(DataContext dataContext, bool isInitData, bool isQuickDebug)
    {
        try
        {
            ConsoleHelper.WriteLine($"程序正在启动....", ConsoleColor.Green);
            ConsoleHelper.WriteLine($"是否开发环境: {isQuickDebug}");
            ConsoleHelper.WriteLine($"ContentRootPath: {AppSettings.ContentRootPath}");
            ConsoleHelper.WriteLine($"WebRootPath: {AppSettings.WebRootPath}");
            ConsoleHelper.WriteLine($"DB Type: {dataContext.DbType}");
            ConsoleHelper.WriteLine($"DB ConnectString: {dataContext.ConnectionString}");
            ConsoleHelper.WriteLine("初始化数据库....");
            dataContext.Db.DbMaintenance.CreateDatabase();
            ConsoleHelper.WriteLine("初始化数据库成功。", ConsoleColor.Green);
            ConsoleHelper.WriteLine();

            ConsoleHelper.WriteLine("初始化数据表....");

            //继承自BaseEntity或者RootKey<>的类型
            //一些没有继承的需手动维护添加
            //例如，用户与岗位(UserJobs)
            var entityList = GlobalData.GetEntityAssembly().GetTypes()
                .Where(x => (x.BaseType == typeof(BaseEntity) || x.BaseType == typeof(RootKey<long>)) &&
                            x != typeof(BaseEntity)).ToList();
            entityList.Add(typeof(UserRoles));
            entityList.Add(typeof(UserJobs));
            entityList.Add(typeof(RoleMenu));
            entityList.Add(typeof(RolesDepartments));


            entityList.ForEach(entity =>
            {
                var attr = entity.GetCustomAttribute<SugarTable>();

                if (!dataContext.Db.DbMaintenance.IsAnyTable(attr == null ? entity.Name : attr.TableName))
                {
                    ConsoleHelper.WriteLine(
                        attr == null
                            ? $"Entity:{entity.Name}-->缺少SugarTable特性"
                            : $"Entity:{entity.Name}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->创建完成！");
                    dataContext.Db.CodeFirst.InitTables(entity);
                }
            });

            ConsoleHelper.WriteLine("初始化数据表成功！", ConsoleColor.Green);
            ConsoleHelper.WriteLine();
            //添加初始数据

            #region 添加初始数据

            if (isInitData)
            {
                ConsoleHelper.WriteLine("初始化种子数据....");
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

                if (!await dataContext.Db.Queryable<UserRoles>().AnyAsync())
                {
                    var attr = typeof(UserRoles).GetCustomAttribute<SugarTable>();
                    if (attr != null)
                    {
                        await dataContext.GetEntityDb<UserRoles>().InsertRangeAsync(
                            JsonConvert.DeserializeObject<List<UserRoles>>(
                                FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                setting));
                        ConsoleHelper.WriteLine(
                            $"Entity:{nameof(UserRoles)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！",
                            ConsoleColor.Green);
                    }
                }

                #endregion

                #region 用户与岗位

                if (!await dataContext.Db.Queryable<UserJobs>().AnyAsync())
                {
                    var attr = typeof(UserJobs).GetCustomAttribute<SugarTable>();
                    if (attr != null)
                    {
                        await dataContext.GetEntityDb<UserJobs>()
                            .InsertRangeAsync(JsonConvert.DeserializeObject<List<UserJobs>>(
                                FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                setting));
                        ConsoleHelper.WriteLine(
                            $"Entity:{nameof(UserJobs)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！",
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

                ConsoleHelper.WriteLine("初始化数据完成！", ConsoleColor.Green);
            }

            #endregion

            ConsoleHelper.WriteLine("程序已启动！", ConsoleColor.Green);
            // return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
