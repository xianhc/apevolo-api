using System.IO;
using Ape.Volo.Common.Global;
using SqlSugar;

namespace Ape.Volo.Common.Helper;

public static class TenantHelper
{
    /// <summary>
    /// 获取租户数据库连接
    /// </summary>
    /// <param name="configId">唯一标识</param>
    /// <param name="dbType">数据库类型</param>
    /// <param name="connection">连接符</param>
    /// <returns></returns>
    public static ConnectionConfig GetConnectionConfig(string configId, DbType dbType, string connection)
    {
        if (dbType == DbType.Sqlite)
        {
            connection = "DataSource=" + Path.Combine(AppSettings.ContentRootPath,
                connection ?? string.Empty);
        }

        return new ConnectionConfig()
        {
            ConfigId = configId,
            DbType = dbType,
            ConnectionString = connection,
            IsAutoCloseConnection = true,
            LanguageType = LanguageType.Chinese,
            MoreSettings = new ConnMoreSettings()
            {
                IsAutoRemoveDataCache = true,
                SqlServerCodeFirstNvarchar = true,
            },
        };
    }
}
