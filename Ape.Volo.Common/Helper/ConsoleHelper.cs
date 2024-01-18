using System;

namespace Ape.Volo.Common.Helper;

public static class ConsoleHelper
{
    public static void WriteLine() => Console.Out.WriteLine();

    /// <summary>
    /// 打印控制台信息
    /// </summary>
    /// <param name="str">待打印的字符串</param>
    /// <param name="color">想要打印的颜色</param>
    public static void WriteLine(string str, ConsoleColor color = ConsoleColor.White)
    {
        ConsoleColor currentForeColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(str);
        Console.ForegroundColor = currentForeColor;
    }
}
