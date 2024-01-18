using System;
using Serilog;

namespace Ape.Volo.Common.Helper.Serilog;

public class SerilogManager
{
    public static ILogger GetLogger(Type type)
    {
        return Log.ForContext("SourceContext", type.FullName);
    }
}
