using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ApeVolo.Common.Exception;

/// <summary>
/// 请求错误
/// </summary>
public class BadRequestException : System.Exception
{
    public int statusCode { get; set; } = StatusCodes.Status400BadRequest;

    public Dictionary<string, string> Errors { get; set; }

    public BadRequestException(string message, Dictionary<string, string> errors = null) : base(message)
    {
    }
}