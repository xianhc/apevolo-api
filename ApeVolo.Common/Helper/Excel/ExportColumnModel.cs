namespace ApeVolo.Common.Helper.Excel;

/// <summary>
/// 导出数据
/// </summary>
public class ExportColumnModel
{
    /// <summary>
    /// 列
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// 列位置
    /// </summary>
    public int Point { get; set; }
}