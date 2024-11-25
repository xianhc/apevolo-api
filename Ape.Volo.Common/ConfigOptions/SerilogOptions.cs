using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

/// <summary>
/// Serilog日志配置
/// </summary>
[OptionsSettings]
public class SerilogOptions
{
    /// <summary>
    /// 是否记录SQL日志
    /// </summary>
    public bool RecordSqlEnabled { get; set; }

    /// <summary>
    /// 输出到数据库
    /// </summary>
    public ToDb ToDb { get; set; }

    /// <summary>
    /// 输出到文件
    /// </summary>
    public ToFile ToFile { get; set; }

    /// <summary>
    /// 输出到控制台
    /// </summary>
    public ToConsole ToConsole { get; set; }

    /// <summary>
    /// 输出到Elasticsearch
    /// </summary>
    public ToElasticsearch ToElasticsearch { get; set; }
}

/// <summary>
/// 输出到数据库
/// </summary>
public class ToDb
{
    public bool Enabled { get; set; }
}

/// <summary>
/// 输出到文件
/// </summary>
public class ToFile
{
    public bool Enabled { get; set; }
}

/// <summary>
/// 输出到控制台
/// </summary>
public class ToConsole
{
    public bool Enabled { get; set; }
}

/// <summary>
/// 输出到Elasticsearch
/// </summary>
public class ToElasticsearch
{
    public bool Enabled { get; set; }
}
