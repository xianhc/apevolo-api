using System;
using ApeVolo.Common.Extention;
using Microsoft.AspNetCore.Http;

namespace ApeVolo.Common.Model;

/// <summary>
/// 请求响应结果
/// </summary>
public class ActionResultVm
{
    /// <summary>
    /// 状态码
    /// </summary>
    public int Status { get; set; } = StatusCodes.Status200OK;

    /// <summary>
    /// 错误
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// 返回消息
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Timestamp
    /// </summary>
    public string Timestamp { get; set; } = DateTime.Now.ToUnixTimeStampMillisecond().ToString();

    /// <summary>
    /// 请求路径
    /// </summary>
    public string Path { get; set; }
}