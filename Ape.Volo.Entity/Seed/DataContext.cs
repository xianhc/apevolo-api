using System;
using System.Linq;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.Common.Extention;
using Microsoft.Extensions.Options;
using SqlSugar;

namespace Ape.Volo.Entity.Seed;

/// <summary>
/// 
/// </summary>
public class DataContext
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sqlSugarClient"></param>
    /// <param name="configs"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public DataContext(ISqlSugarClient sqlSugarClient, IOptionsMonitor<Configs> configs = null)
    {
        _configs = configs?.CurrentValue ?? new Configs();
        if (string.IsNullOrEmpty(ConnectionString))
            throw new ArgumentNullException(nameof(sqlSugarClient), "数据库连接字符串为空");
        _db = sqlSugarClient as SqlSugarScope;
    }

    /// <summary>
    /// 
    /// </summary>
    public ConnectionItem ConnectObject => GetCurrentDataBase();

    /// <summary>
    /// 连接字符串 
    /// </summary>
    public string ConnectionString => ConnectObject.ConnectionString;

    /// <summary>
    /// 数据库类型 
    /// </summary>
    public DbType DbType => (DbType)ConnectObject.DbType;


    /// <summary>
    /// 数据连接对象 
    /// </summary>
    private SqlSugarScope _db;

    public SqlSugarScope Db
    {
        get => _db;
        private set => _db = value;
    }

    /// <summary>
    /// 数据连接对象 
    /// </summary>
    private Configs _configs;

    public Configs Configs
    {
        get => _configs;
    }

    /// <summary>
    /// 当前数据库
    /// </summary>
    private ConnectionItem GetCurrentDataBase()
    {
        var defaultConnectionItem =
            Configs.DataConnection.ConnectionItem.FirstOrDefault(x => x.ConnId == Configs.DefaultDataBase);
        if (defaultConnectionItem.IsNull())
        {
            throw new Exception("数据库配置出错，请检查！");
        }

        return defaultConnectionItem;
    }

    #region 实例方法

    /// <summary>
    /// 获取数据库处理对象
    /// </summary>
    /// <returns>返回值</returns>
    public SimpleClient<T> GetEntityDb<T>() where T : class, new()
    {
        return new SimpleClient<T>(_db);
    }

    /// <summary>
    /// 获取数据库处理对象
    /// </summary>
    /// <param name="db">db</param>
    /// <returns>返回值</returns>
    public SimpleClient<T> GetEntityDb<T>(SqlSugarClient db) where T : class, new()
    {
        return new SimpleClient<T>(db);
    }

    #endregion


    #region 根据实体类生成数据库表

    /// <summary>
    /// 根据实体类生成数据库表
    /// </summary>
    /// <param name="blnBackupTable">是否备份表</param>
    /// <param name="entityList">指定的实体</param>
    public void CreateTableByEntity<T>(bool blnBackupTable, params T[] entityList) where T : class, new()
    {
        Type[] lstTypes = null;
        if (entityList != null)
        {
            lstTypes = new Type[entityList.Length];
            for (int i = 0; i < entityList.Length; i++)
            {
                lstTypes[i] = typeof(T);
            }
        }

        CreateTableByEntity(blnBackupTable, lstTypes);
    }

    /// <summary>
    /// 根据实体类生成数据库表
    /// </summary>
    /// <param name="blnBackupTable">是否备份表</param>
    /// <param name="entityList">指定的实体</param>
    private void CreateTableByEntity(bool blnBackupTable, params Type[] entityList)
    {
        if (blnBackupTable)
        {
            Db.CodeFirst.BackupTable().InitTables(entityList); //change entity backupTable            
        }
        else
        {
            Db.CodeFirst.InitTables(entityList);
        }
    }

    #endregion
}
