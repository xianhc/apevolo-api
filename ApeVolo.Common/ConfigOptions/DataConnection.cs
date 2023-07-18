using System.Collections.Generic;

namespace ApeVolo.Common.ConfigOptions;

public class DataConnection
{
    public List<ConnectionItem> ConnectionItem { get; set; }
}

public class ConnectionItem
{
    public string ConnId { get; set; }
    public int HitRate { get; set; }
    public int DbType { get; set; }
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; }
}