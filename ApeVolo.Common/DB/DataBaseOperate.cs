using ApeVolo.Common.Global;

namespace ApeVolo.Common.DB
{
    /// <summary>
    /// 数据库操作
    /// </summary>
    public class DataBaseOperate
    {
        public string ConnId { get; set; }
        public string Conn { get; set; }
        public DataBaseType DbType { get; set; }
    }
}