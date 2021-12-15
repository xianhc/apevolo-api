using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApeVolo.Api.Authentication;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ApeVolo.Api.Extensions
{
    /// <summary>
    /// 授权启动器
    /// </summary>
    public static class AuthorizationSetup
    {
        public static void AddAuthorizationSetup(this IServiceCollection services)
        {
            if (services.IsNull()) throw new ArgumentNullException(nameof(services));

            //读取配置文件
            var symmetricKeyAsBase64 = AppSettings.GetValue("Audience", "Secret");
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var issuer = AppSettings.GetValue("Audience", "Issuer");
            var audience = AppSettings.GetValue("Audience", "Audience");


            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var permission = new List<PermissionList>();

            // 角色与接口的权限要求参数
            var permissionRequirement = new PermissionRequirement(
                "/api/denied", // 拒绝授权地址
                permission,
                ClaimTypes.Role, //基于角色的授权
                issuer, //发行人
                audience, //听众
                signingCredentials, //签名凭据
                expiration: TimeSpan.FromHours(12) //接口的过期时间
            );


            // 自定义策略授权
            services.AddAuthorization(options =>
            {
                options.AddPolicy(GlobalVar.AuthPolicysName,
                    policy => policy.Requirements.Add(permissionRequirement));
            });


            // 令牌验证参数
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = issuer, //发行人
                ValidateAudience = true,
                ValidAudience = audience, //订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),
                RequireExpirationTime = true,
            };

            //2.1【认证】、core自带官方JWT认证
            // 开启Bearer认证
            services.AddAuthentication(o =>
                {
                    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = nameof(ApiResponseHandler);
                    o.DefaultForbidScheme = nameof(ApiResponseHandler);
                })
                // 添加JwtBearer服务
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = tokenValidationParameters;
                    o.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            // 如果过期，把过期信息添加到头部
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }

                            return Task.CompletedTask;
                        }
                    };
                })
                .AddScheme<AuthenticationSchemeOptions, ApiResponseHandler>(nameof(ApiResponseHandler), _ => { });


            // 注入权限处理器
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddSingleton(permissionRequirement);
        }
    }
}