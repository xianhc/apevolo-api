using System.Linq;
using System.Text;
using ApeVolo.Common.Extention;

namespace ApeVolo.Common.Helper
{
    /// <summary>
    /// 异常处理帮助类
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        /// 获取异常位置
        /// </summary>
        /// <param name="e">异常</param>
        /// <returns></returns>
        private static string GetExceptionAddr(System.Exception e)
        {
            StringBuilder excAddrBuilder = new StringBuilder();
            e?.StackTrace?.Split("\r\n".ToArray())?.ToList()?.ForEach(item =>
            {
                if (item.Contains("行号") || item.Contains("line"))
                    excAddrBuilder.Append($"    {item}\r\n");
            });

            string addr = excAddrBuilder.ToString();

            return addr.IsNullOrEmpty() ? "    无" : addr;
        }

        /// <summary>
        /// 获取异常消息
        /// </summary>
        /// <param name="ex">捕捉的异常</param>
        /// <param name="level">内部异常层级</param>
        /// <returns></returns>
        private static string GetExceptionAllMsg(System.Exception ex, int level)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($@"
{level}层错误:
  消息:
    {ex?.Message}
  位置:
{GetExceptionAddr(ex)}
");
            if (ex.InnerException != null)
            {
                builder.Append(GetExceptionAllMsg(ex.InnerException, level + 1));
            }

            return builder.ToString();
        }

        /// <summary>
        /// 获取异常消息
        /// </summary>
        /// <param name="ex">捕捉的异常</param>
        /// <returns></returns>
        public static string GetExceptionAllMsg(System.Exception ex)
        {
            string msg = GetExceptionAllMsg(ex, 1);
//             try
//             {
//                 msg += $@"
// url:{HttpContextCore.CurrentHttpContext.Request.GetDisplayUrl()}
// body:{HttpContextCore.CurrentHttpContext.Request.Body.ReadToString()}
// ";
//             }
//             catch
//             {
//
//             }
            return msg;
        }
    }
}