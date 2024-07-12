using System;
using System.IO;
using System.Runtime.InteropServices;
using Ape.Volo.Common;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Helper.Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

namespace Ape.Volo.Api.Extensions;

/// <summary>
/// swaggerSetup启动器
/// </summary>
public static class SwaggerSetup
{
    private static readonly ILogger Logger = SerilogManager.GetLogger(typeof(SwaggerSetup));

    public static void AddSwaggerSetup(this IServiceCollection services)
    {
        if (services.IsNull()) throw new ArgumentNullException(nameof(services));

        var basePath = AppContext.BaseDirectory;
        var options = App.GetOptions<SwaggerOptions>();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(options.Name, new OpenApiInfo
            {
                Version = options.Version,
                Title = options.Title + "    " + RuntimeInformation.FrameworkDescription
            });

            try
            {
                var apiXml = Path.Combine(basePath, "Ape.Volo.Api.xml");
                var commonXml = Path.Combine(basePath, "Ape.Volo.Common.xml");
                var iBusinessXml = Path.Combine(basePath, "Ape.Volo.IBusiness.xml");
                c.IncludeXmlComments(apiXml, true);
                c.IncludeXmlComments(commonXml, true);
                c.IncludeXmlComments(iBusinessXml, true);
            }
            catch (Exception ex)
            {
                Logger.Error("swaggerSetup启动失败\n" + ex.Message);
            }

            // 开启加权小锁
            c.OperationFilter<AddResponseHeadersFilter>();
            c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

            // 在header中添加token，传递到后台
            c.OperationFilter<SecurityRequirementsOperationFilter>();


            // 必须是 oauth2
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token} \"",
                Name = "Authorization", //jwt默认的参数名称
                In = ParameterLocation.Header, //jwt默认存放Authorization信息的位置(请求头中)
                Type = SecuritySchemeType.ApiKey
            });
        });
    }
}
