using System;
using Serilog;

namespace ApeVolo.Common.Helper.Serilog;

public class SerilogManager
{
    public static ILogger GetLogger(Type type)
    {
        return Log.ForContext("SourceContext", type.FullName);
    }
}