using System;
using System.Linq;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper;
using Microsoft.AspNetCore.Builder;

namespace Ape.Volo.Api.Extensions;

public static class ApplicationNotifierSetup
{
    public static void ApplicationStartedNotifier(this WebApplication app)
    {
        if (app.IsNull())
            throw new ArgumentNullException(nameof(app));
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var address = "8002";
            if (app.Configuration["urls"] != null)
            {
                address = app.Configuration["urls"].Split(':').Last();
            }

            ConsoleHelper.WriteLine($"应用程序启动成功! {{端口号 : {address}}}\n" +
                                    "欢迎使用《apevolo-api》中后台权限管理系统\n" +
                                    "加群方式:微信号：apevolo<备注'加群'>   QQ群：839263566\n" +
                                    "项目在线文档:http://doc.apevolo.com/\n" +
                                    $"接口文档地址:http://localhost:{address}/swagger/api/index.html\n" +
                                    "前端运行地址:http://localhost:8001\n" +
                                    "如果项目让您获得了收益，希望您能请作者喝杯咖啡:http://doc.apevolo.com/donate",
                ConsoleColor.Green);
        });
    }
}
