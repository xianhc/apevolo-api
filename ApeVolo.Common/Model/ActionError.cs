using System.Collections.Generic;
using System.Linq;

namespace ApeVolo.Common.Model;

public class ActionError
{
    public Dictionary<string, string> Errors { get; set; }

    public string GetFirstError()
    {
        return Errors != null && Errors.Any()
            ? Errors.First().Value
            : "";
    }
}