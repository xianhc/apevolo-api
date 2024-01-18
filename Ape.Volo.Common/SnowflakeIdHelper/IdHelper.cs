namespace Ape.Volo.Common.SnowflakeIdHelper;

/// <summary>
/// 雪花ID生成工具
/// </summary>
public static class IdHelper
{
    internal static SnowflakeIdWorker IdWorker { get; set; }

    internal static IdHelperBootstrapper IdHelperBootstrapper { get; set; }


    //public static long WorkerId { get => IdWorker.workerId; }

    /// <summary>
    /// 获取String型雪花Id
    /// </summary>
    /// <returns></returns>
    public static string GetId()
    {
        return GetLongId().ToString();
    }

    /// <summary>
    /// 获取long型雪花Id
    /// </summary>
    /// <returns></returns>
    public static long GetLongId()
    {
        return IdWorker.NextId();
    }
}
