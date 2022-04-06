using System;
using System.IO;
using System.Runtime.InteropServices;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace ApeVolo.Api.Extensions;

/// <summary>
/// swaggerSetup启动器
/// </summary>
public static class SwaggerSetup
{
    private static readonly ILog Log =
        LogManager.GetLogger(typeof(SwaggerSetup));

    public static void AddSwaggerSetup(this IServiceCollection services)
    {
        if (services.IsNull()) throw new ArgumentNullException(nameof(services));

        var basePath = AppContext.BaseDirectory;

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(AppSettings.GetValue("Swagger", "Name"), new OpenApiInfo
            {
                Version = AppSettings.GetValue("Swagger", "Version"),
                Title = AppSettings.GetValue("Swagger", "Title") + "    " + RuntimeInformation.FrameworkDescription
            });

            try
            {
                var xmlPath = Path.Combine(basePath, "ApeVolo.Api.xml");
                c.IncludeXmlComments(xmlPath, true);
            }
            catch (Exception ex)
            {
                Log.Error("swaggerSetup启动失败\n" + ex.Message);
            }

            // 开启加权小锁
            c.OperationFilter<AddResponseHeadersFilter>();
            c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

            // 在header中添加token，传递到后台
            c.OperationFilter<SecurityRequirementsOperationFilter>();


            // 必须是 oauth2
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                Name = "Authorization", //jwt默认的参数名称
                In = ParameterLocation.Header, //jwt默认存放Authorization信息的位置(请求头中)
                Type = SecuritySchemeType.ApiKey
            });
        });
    }
}