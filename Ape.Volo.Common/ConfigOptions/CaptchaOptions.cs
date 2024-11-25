using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

/// <summary>
/// 验证码配置
/// </summary>
[OptionsSettings]
public class CaptchaOptions
{
    /// <summary>
    /// 长度
    /// </summary>
    public int KeyLength { get; set; }

    /// <summary>
    /// 图像宽度
    /// </summary>
    public int ImgWidth { get; set; }

    /// <summary>
    /// 图像高度
    /// </summary>
    public int ImgHeight { get; set; }

    /// <summary>
    /// 字体大小
    /// </summary>
    public int FontSize { get; set; }

    /// <summary>
    /// 触发验证码的失败次数阈值(超出则显示验证码)
    /// </summary>
    public int Threshold { get; set; }

    /// <summary>
    /// 验证码阈值超时时间 单位：s(秒)
    /// </summary>
    public int TimeOut { get; set; }
}
