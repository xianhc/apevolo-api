using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using ApeVolo.Common.Extention;

namespace ApeVolo.Api.Extensions
{
    /// <summary>
    /// swaggerSetup启动器
    /// </summary>
    public static class SwaggerSetup
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(typeof(SwaggerSetup));

        public static void AddSwaggerSetup(this IServiceCollection services)
        {
            if (services.IsNull()) throw new ArgumentNullException(nameof(services));

            var basePath = AppContext.BaseDirectory;

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1.0.0",
                    Title = "Apevolo Api 接口文档"
                });

                try
                {
                    var xmlPath = Path.Combine(basePath, "ApeVolo.Api.xml");
                    c.IncludeXmlComments(xmlPath, true); //默认的第二个参数是false，这个是controller的注释，记得修改
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
}