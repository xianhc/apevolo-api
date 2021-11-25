using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper;
using ApeVolo.Entity.Do.Core;
using ApeVolo.Entity.Do.Dictionary;
using ApeVolo.Entity.Do.Email;
using ApeVolo.Entity.Do.Tasks;
using Newtonsoft.Json;
using SqlSugar;

namespace ApeVolo.Entity.Seed
{
    public class SeedData
    {
        /// <summary>
        /// 异步添加种子数据
        /// </summary>
        /// <param name="myContext"></param>
        /// <param name="webRootPath"></param>
        /// <returns></returns>
        public static async Task InitSystemDataAsync(MyContext myContext, string webRootPath)
        {
            try
            {
                if (string.IsNullOrEmpty(webRootPath))
                {
                    throw new Exception("获取wwwroot路径时，异常！");
                }

                Console.WriteLine($"程序启动==>webRootPath:{webRootPath}");
                Console.WriteLine($"DB Type: {MyContext.DbType}");
                Console.WriteLine($"DB ConnectString: {MyContext.ConnectionString}");
                Console.WriteLine("初始化数据库....");
                myContext.Db.DbMaintenance.CreateDatabase();
                ConsoleHelper.WriteLine("初始化数据库成功。", ConsoleColor.Green);
                Console.WriteLine();

                Console.WriteLine("初始化数据表....");
                var entityTypes = GlobalData.EntityTypes.Where(x => x.GetCustomAttribute<InitTableAttribute>() != null)
                    .Select(x => x.GetCustomAttribute<InitTableAttribute>()?.SourceType);

                entityTypes.ForEach(entity =>
                {
                    var attr = entity.GetCustomAttribute<SugarTable>();
                    if (attr == null)
                    {
                        Console.WriteLine($"Entity:{entity.Name}-->缺少SugarTable特性");
                    }
                    else
                    {
                        if (!myContext.Db.DbMaintenance.IsAnyTable(attr.TableName))
                        {
                            ConsoleHelper.WriteLine(
                                $"Entity:{entity.Name}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->创建完成！");
                            myContext.Db.CodeFirst.InitTables(entity);
                        }
                    }
                });
                ConsoleHelper.WriteLine($"初始化数据表成功！", ConsoleColor.Green);
                Console.WriteLine();
                //添加初始数据

                #region 添加初始数据

                if (AppSettings.GetValue("InitSeedData").ToBool())
                {
                    Console.WriteLine("初始化种子数据....");
                    JsonSerializerSettings setting = new JsonSerializerSettings();
                    JsonConvert.DefaultSettings = () =>
                    {
                        setting.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                        setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                        setting.NullValueHandling = NullValueHandling.Ignore;
                        return setting;
                    };
                    string seedDataFolder = "resources/db/{0}.tsv";
                    seedDataFolder = Path.Combine(webRootPath, seedDataFolder);

                    #region 用户

                    if (!await myContext.Db.Queryable<User>().AnyAsync())
                    {
                        var attr = typeof(User).GetCustomAttribute<SugarTable>();
                        if (attr != null)
                        {
                            await myContext.GetEntityDb<User>().InsertRangeAsync(
                                JsonConvert.DeserializeObject<List<User>>(
                                    FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                    setting));
                            ConsoleHelper.WriteLine(
                                $"Entity:{nameof(User)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！");
                        }
                    }

                    #endregion

                    #region 角色

                    if (!await myContext.Db.Queryable<Role>().AnyAsync())
                    {
                        var attr = typeof(Role).GetCustomAttribute<SugarTable>();
                        if (attr != null)
                        {
                            await myContext.GetEntityDb<Role>().InsertRangeAsync(
                                JsonConvert.DeserializeObject<List<Role>>(
                                    FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                    setting));
                            ConsoleHelper.WriteLine(
                                $"Entity:{nameof(Role)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！");
                        }
                    }

                    #endregion

                    #region 菜单

                    if (!await myContext.Db.Queryable<Menu>().AnyAsync())
                    {
                        var attr = typeof(Menu).GetCustomAttribute<SugarTable>();
                        if (attr != null)
                        {
                            await myContext.GetEntityDb<Menu>().InsertRangeAsync(
                                JsonConvert.DeserializeObject<List<Menu>>(
                                    FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                    setting));
                            ConsoleHelper.WriteLine(
                                $"Entity:{nameof(Menu)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！");
                        }
                    }

                    #endregion

                    #region 部门

                    if (!await myContext.Db.Queryable<Department>().AnyAsync())
                    {
                        var attr = typeof(Department).GetCustomAttribute<SugarTable>();
                        if (attr != null)
                        {
                            await myContext.GetEntityDb<Department>().InsertRangeAsync(
                                JsonConvert.DeserializeObject<List<Department>>(
                                    FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                    setting));
                            ConsoleHelper.WriteLine(
                                $"Entity:{nameof(Department)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！");
                        }
                    }

                    #endregion

                    #region 岗位

                    if (!await myContext.Db.Queryable<Job>().AnyAsync())
                    {
                        var attr = typeof(Job).GetCustomAttribute<SugarTable>();
                        if (attr != null)
                        {
                            await myContext.GetEntityDb<Job>().InsertRangeAsync(
                                JsonConvert.DeserializeObject<List<Job>>(
                                    FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                    setting));
                            ConsoleHelper.WriteLine(
                                $"Entity:{nameof(Job)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！");
                        }
                    }

                    #endregion

                    #region 系统全局设置

                    if (!await myContext.Db.Queryable<Setting>().AnyAsync())
                    {
                        var attr = typeof(Setting).GetCustomAttribute<SugarTable>();
                        if (attr != null)
                        {
                            await myContext.GetEntityDb<Setting>().InsertRangeAsync(
                                JsonConvert.DeserializeObject<List<Setting>>(
                                    FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                    setting));
                            ConsoleHelper.WriteLine(
                                $"Entity:{nameof(Setting)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！");
                        }
                    }

                    #endregion
                    
                    #region 字典

                    if (!await myContext.Db.Queryable<Dict>().AnyAsync())
                    {
                        var attr = typeof(Dict).GetCustomAttribute<SugarTable>();
                        if (attr != null)
                        {
                            await myContext.GetEntityDb<Dict>().InsertRangeAsync(
                                JsonConvert.DeserializeObject<List<Dict>>(
                                    FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                    setting));
                            ConsoleHelper.WriteLine(
                                $"Entity:{nameof(Dict)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！");
                        }
                    }

                    #endregion

                    #region 字典详情

                    if (!await myContext.Db.Queryable<DictDetail>().AnyAsync())
                    {
                        var attr = typeof(DictDetail).GetCustomAttribute<SugarTable>();
                        if (attr != null)
                        {
                            await myContext.GetEntityDb<DictDetail>().InsertRangeAsync(
                                JsonConvert.DeserializeObject<List<DictDetail>>(
                                    FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                    setting));
                            ConsoleHelper.WriteLine(
                                $"Entity:{nameof(DictDetail)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！");
                        }
                    }

                    #endregion

                    #region 作业调度

                    if (!await myContext.Db.Queryable<QuartzNet>().AnyAsync())
                    {
                        var attr = typeof(QuartzNet).GetCustomAttribute<SugarTable>();
                        if (attr != null)
                        {
                            await myContext.GetEntityDb<QuartzNet>().InsertRangeAsync(
                                JsonConvert.DeserializeObject<List<QuartzNet>>(
                                    FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                    setting));
                            ConsoleHelper.WriteLine(
                                $"Entity:{nameof(QuartzNet)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！");
                        }
                    }

                    #endregion

                    #region 邮箱账户

                    if (!await myContext.Db.Queryable<EmailAccount>().AnyAsync())
                    {
                        var attr = typeof(EmailAccount).GetCustomAttribute<SugarTable>();
                        if (attr != null)
                        {
                            await myContext.GetEntityDb<EmailAccount>().InsertRangeAsync(
                                JsonConvert.DeserializeObject<List<EmailAccount>>(
                                    FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                    setting));
                            ConsoleHelper.WriteLine(
                                $"Entity:{nameof(EmailAccount)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！");
                        }
                    }

                    #endregion

                    #region 邮件模板

                    if (!await myContext.Db.Queryable<MessageTemplate>().AnyAsync())
                    {
                        var attr = typeof(MessageTemplate).GetCustomAttribute<SugarTable>();
                        if (attr != null)
                        {
                            await myContext.GetEntityDb<MessageTemplate>().InsertRangeAsync(
                                JsonConvert.DeserializeObject<List<MessageTemplate>>(
                                    FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                    setting));
                            ConsoleHelper.WriteLine(
                                $"Entity:{nameof(MessageTemplate)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！");
                        }
                    }

                    #endregion

                    #region 用户与角色

                    if (!await myContext.Db.Queryable<UserRoles>().AnyAsync())
                    {
                        var attr = typeof(UserRoles).GetCustomAttribute<SugarTable>();
                        if (attr != null)
                        {
                            await myContext.GetEntityDb<UserRoles>().InsertRangeAsync(
                                JsonConvert.DeserializeObject<List<UserRoles>>(
                                    FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                    setting));
                            ConsoleHelper.WriteLine(
                                $"Entity:{nameof(UserRoles)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！");
                        }
                    }

                    #endregion

                    #region 用户与岗位

                    if (!await myContext.Db.Queryable<UserJobs>().AnyAsync())
                    {
                        var attr = typeof(UserJobs).GetCustomAttribute<SugarTable>();
                        if (attr != null)
                        {
                            await myContext.GetEntityDb<UserJobs>()
                                .InsertRangeAsync(JsonConvert.DeserializeObject<List<UserJobs>>(
                                    FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                    setting));
                            ConsoleHelper.WriteLine(
                                $"Entity:{nameof(UserJobs)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！");
                        }
                    }

                    #endregion

                    #region 角色与菜单

                    if (!await myContext.Db.Queryable<RoleMenu>().AnyAsync())
                    {
                        var attr = typeof(RoleMenu).GetCustomAttribute<SugarTable>();
                        if (attr != null)
                        {
                            await myContext.GetEntityDb<RoleMenu>()
                                .InsertRangeAsync(JsonConvert.DeserializeObject<List<RoleMenu>>(
                                    FileHelper.ReadFile(string.Format(seedDataFolder, attr.TableName), Encoding.UTF8),
                                    setting));
                            ConsoleHelper.WriteLine(
                                $"Entity:{nameof(RoleMenu)}-->Table:{attr.TableName}-->Desc:{attr.TableDescription}-->初始数据成功！");
                        }
                    }

                    #endregion

                    ConsoleHelper.WriteLine($"初始化数据完成！", ConsoleColor.Green);
                }

                #endregion

                ConsoleHelper.WriteLine($"程序已启动！", ConsoleColor.Green);
                // return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}