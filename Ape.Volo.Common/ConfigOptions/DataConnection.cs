using System.Collections.Generic;
using SqlSugar;

namespace Ape.Volo.Common.ConfigOptions;

public class DataConnection
{
    public List<ConnectionItem> ConnectionItem { get; set; }
}

public class ConnectionItem
{
    public string ConnId { get; set; }
    public int HitRate { get; set; }
    public DbType DbType { get; set; }
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; }
}
