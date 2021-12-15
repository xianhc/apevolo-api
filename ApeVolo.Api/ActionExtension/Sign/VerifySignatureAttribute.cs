using System;
using System.Threading.Tasks;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.DI;
using ApeVolo.Common.Extention;
using ApeVolo.IBusiness.Interface.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApeVolo.Api.ActionExtension.Sign
{
    /// <summary>
    /// 接口对外签名验证
    /// </summary>
    public class VerifySignatureAttribute : BaseActionFilter
    {
        // private readonly IRedisCacheService _redisCacheService;
        //
        // public VerifySignatureAttribute(IRedisCacheService redisCacheService)
        // {
        //     _redisCacheService = redisCacheService;
        // }

        /// <summary>
        /// Action执行之前执行
        /// </summary>
        /// <param name="filterContext"></param>
        public override async Task OnActionExecuting(ActionExecutingContext filterContext)
        {
            //判断是否需要签名
            if (filterContext.ContainsFilter<IgnoreVerifySignatureAttribute>())
                return;
            var request = filterContext.HttpContext.Request;

            string appId = request.Headers["appId"].ToString();
            if (appId.IsNullOrEmpty())
            {
                ReturnError("缺少header:appId");
                return;
            }

            string time = request.Headers["time"].ToString();
            if (time.IsNullOrEmpty())
            {
                ReturnError("缺少header:time");
                return;
            }

            if (time.ToDateTime() < DateTime.Now.AddMinutes(-5) || time.ToDateTime() > DateTime.Now.AddMinutes(5))
            {
                ReturnError("time过期");
                return;
            }

            string guid = request.Headers["guid"].ToString();
            if (guid.IsNullOrEmpty())
            {
                ReturnError("缺少header:guid");
                return;
            }

            string guidKey = $"ApiGuid_{guid}";
            var redisCacheService = AutofacHelper.GetService<IRedisCacheService>();
            if (!redisCacheService.GetCacheStrAsync(guidKey).IsNullOrEmpty())
            {
                await redisCacheService.SetCacheAsync(guidKey, "1");
            }
            else
            {
                ReturnError("禁止重复调用!");
                return;
            }

            request.EnableBuffering();
            string body = request.Body.ReadToString();

            string sign = request.Headers["sign"].ToString();
            if (sign.IsNullOrEmpty())
            {
                ReturnError("缺少header:sign");
                return;
            }

            var appSecretModel = await AutofacHelper.GetService<IAppSecretService>()
                .QueryFirstAsync(x => x.IsDeleted == false && x.AppId == appId);
            if (appSecretModel.IsNull())
            {
                ReturnError("header:appId无效");
                return;
            }

            string newSign = BuildApiSign(appId, appSecretModel.AppSecretKey, guid, time.ToDateTime(), body);
            if (sign != newSign)
            {
                string log =
                    $@"sign签名错误!
headers:{request.Headers.ToJson()}
body:{body}
正确sign:{newSign}
";
                // ReturnError("header:sign签名错误");
            }

            void ReturnError(string msg)
            {
                filterContext.Result = Error(msg);
            }
        }

        /// <summary>
        /// 生成接口签名sign
        /// 注：md5(appId+time+guid+body+appSecret)
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <param name="appSecret">应用密钥</param>
        /// <param name="guid">唯一GUID</param>
        /// <param name="time">时间</param>
        /// <param name="body">请求体</param>
        /// <returns></returns>
        private string BuildApiSign(string appId, string appSecret, string guid, DateTime time, string body)
        {
            return $"{appId}{time:yyyy-MM-dd HH:mm:ss}{guid}{body}{appSecret}".ToMd5String();
        }
    }
}