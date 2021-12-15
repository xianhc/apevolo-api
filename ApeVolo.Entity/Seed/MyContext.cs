using System;
using ApeVolo.Common.DB;
using SqlSugar;

namespace ApeVolo.Entity.Seed
{
    public class MyContext
    {
        public MyContext()
        {
            if (string.IsNullOrEmpty(ConnectionString))
                throw new ArgumentNullException("数据库连接字符串为空");
            Db = new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = ConnectionString,
                DbType = DbType,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute, //mark
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    //DataInfoCacheService = new HttpRuntimeCache()
                },
                MoreSettings = new ConnMoreSettings
                {
                    //IsWithNoLockQuery = true,
                    IsAutoRemoveDataCache = true
                }
            });
        }

        private static DataBaseOperate ConnectObject => GetCurrentConnectionDb();

        /// <summary>
        /// 连接字符串 
        /// </summary>
        public static string ConnectionString { get; set; } = ConnectObject.ConnectionString;

        /// <summary>
        /// 数据库类型 
        /// </summary>
        public static DbType DbType { get; set; } = (DbType) ConnectObject.DbType;

        /// <summary>
        /// 数据连接对象 
        /// </summary>
        public SqlSugarClient Db { get; private set; }

        /// <summary>
        /// 数据库上下文实例（自动关闭连接）
        /// </summary>
        public static MyContext Context => new MyContext();

        /// <summary>
        /// 当前数据库连接字符串 
        /// </summary>
        private static DataBaseOperate GetCurrentConnectionDb()
        {
            return BaseDbConfig.GetDataBaseOperate.MasterDb;
        }

        #region 实例方法

        /// <summary>
        /// 获取数据库处理对象
        /// </summary>
        /// <returns>返回值</returns>
        public SimpleClient<T> GetEntityDb<T>() where T : class, new()
        {
            return new SimpleClient<T>(Db);
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
}