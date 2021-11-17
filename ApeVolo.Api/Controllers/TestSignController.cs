using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Api.ActionExtension.Sign;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.Extention;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TestSignController : BaseApieNoAuthorizeController
    {
        [HttpPost]
        [VerifySignature]
        public async Task<ActionResult<object>> TestSecret()
        {
            var text = new List<string>() {"AAA", "BBB"};
            await Task.CompletedTask;
            return text.ToJson();
        }
    }
}