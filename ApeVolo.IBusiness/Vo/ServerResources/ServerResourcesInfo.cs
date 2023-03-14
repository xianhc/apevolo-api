namespace ApeVolo.IBusiness.Vo.ServerResources;

public class ServerResourcesInfo
{
    /// <summary>
    /// 运行时间
    /// </summary>
    public string Time { get; set; }

    /// <summary>
    /// 系统信息
    /// </summary>
    public Sys Sys { get; set; }

    /// <summary>
    /// Cpu信息
    /// </summary>
    public Cpu Cpu { get; set; }

    /// <summary>
    /// 内存信息
    /// </summary>
    public Memory Memory { get; set; }

    /// <summary>
    /// 交换区信息
    /// </summary>
    public Swap Swap { get; set; }

    /// <summary>
    /// 磁盘信息
    /// </summary>
    public Disk Disk { get; set; }
}