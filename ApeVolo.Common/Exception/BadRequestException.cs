using ApeVolo.Common.Extention;
using Microsoft.AspNetCore.Http;

namespace ApeVolo.Common.Exception
{
    /// <summary>
    /// 请求错误
    /// </summary>
    public class BadRequestException : System.Exception
    {
        public int statusCode { get; set; } = StatusCodes.Status400BadRequest;

        public BadRequestException(string message) : base(message)
        {
        }
    }
}