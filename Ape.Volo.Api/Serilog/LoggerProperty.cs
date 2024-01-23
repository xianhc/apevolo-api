namespace Ape.Volo.Api.Serilog;

public class LoggerProperty
{
    public static readonly string SugarActionType = "SugarActionType";
    public static readonly string RecordSqlLog = "RecordSqlLog";
    public static readonly string ToConsole = "ToConsole";
    public static readonly string ToFile = "OutToFile";
    public static readonly string ToDb = "ToDb";
    public static readonly string ToElasticsearch = "ToElasticsearch";

    public static readonly string MessageTemplate =
        "{NewLine}时间:{Timestamp:yyyy-MM-dd HH:mm:ss.fff}{NewLine}所在类:{SourceContext}{NewLine}等级:{Level}{NewLine}信息:{Message}{NewLine}{Exception}";
}
