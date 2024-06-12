using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Model;
using Ape.Volo.Common.SnowflakeIdHelper;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using Ape.Volo.IBusiness.QueryModel;
using Ape.Volo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Ape.Volo.Api.Controllers.Permission;

/// <summary>
/// Apis管理
/// </summary>
[Area("Apis管理")]
[Route("/api/apis", Order = 20)]
public class ApisController : BaseApiController
{
    #region 字段

    private readonly IApisService _apisService;

    #endregion

    #region 构造函数

    public ApisController(IApisService apisService)
    {
        _apisService = apisService;
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 新增Api
    /// </summary>
    /// <param name="createUpdateApisDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("创建")]
    public async Task<ActionResult<object>> Create(
        [FromBody] CreateUpdateApisDto createUpdateApisDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _apisService.CreateAsync(createUpdateApisDto);
        return Create();
    }

    /// <summary>
    /// 更新Api
    /// </summary>
    /// <param name="createUpdateApisDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("编辑")]
    public async Task<ActionResult<object>> Update(
        [FromBody] CreateUpdateApisDto createUpdateApisDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _apisService.UpdateAsync(createUpdateApisDto);
        return NoContent();
    }

    /// <summary>
    /// 删除Api
    /// </summary>
    /// <param name="idCollection"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    [Description("删除")]
    public async Task<ActionResult<object>> Delete([FromBody] IdCollection idCollection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _apisService.DeleteAsync(idCollection.IdArray);
        return Success();
    }

    /// <summary>
    /// 查看Apis列表
    /// </summary>
    /// <param name="apisQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("查询")]
    public async Task<ActionResult<object>> Query(ApisQueryCriteria apisQueryCriteria, Pagination pagination)
    {
        var apisList = await _apisService.QueryAsync(apisQueryCriteria, pagination);
        return JsonContent(new ActionResultVm<Apis>
        {
            Content = apisList,
            TotalElements = pagination.TotalElements
        });
    }


    #region 刷新api列表

    /// <summary>
    /// 刷新Api列表 只实现了新增的api添加
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("refresh")]
    [Description("刷新")]
    public async Task<ActionResult<object>> RefreshApis()
    {
        List<Apis> apis = new List<Apis>();
        var assembly = GlobalData.GetApiAssembly();
        var allApis = await _apisService.QueryAllAsync();
        var types = assembly.ExportedTypes.Where(x =>
                x.IsClass && typeof(Controller).IsAssignableFrom(x) && x.Name != "TestController" &&
                x.Namespace != "Ape.Volo.Api.Controllers.Base")
            .OrderBy(x => x.GetCustomAttributes<RouteAttribute>().FirstOrDefault()?.Order).ToList();
        foreach (var type in types)
        {
            var areaAttr = type.GetCustomAttributes(typeof(AreaAttribute), true)
                .OfType<AreaAttribute>()
                .FirstOrDefault();
            var routeAttr = type.GetCustomAttributes(typeof(RouteAttribute), true)
                .OfType<RouteAttribute>()
                .FirstOrDefault();
            var methods = type.GetMethods().Where(m =>
                m.DeclaringType == type && !Attribute.IsDefined(m, typeof(NonActionAttribute)));

            foreach (var methodInfo in methods)
            {
                var methodAttr = methodInfo.GetCustomAttributes(typeof(HttpMethodAttribute), true)
                    .OfType<HttpMethodAttribute>()
                    .FirstOrDefault();
                var methodRouteAttr = methodInfo.GetCustomAttributes(typeof(RouteAttribute), true)
                    .OfType<RouteAttribute>()
                    .FirstOrDefault();
                var url = ($"{routeAttr?.Template}/{methodRouteAttr?.Template}").ToLower();
                var method = methodAttr?.HttpMethods.FirstOrDefault()?.Trim();
                if (!allApis.Any(
                        x => x.Url.Equals(url, StringComparison.CurrentCultureIgnoreCase) && x.Method == method))
                {
                    apis.Add(new Apis()
                    {
                        Id = IdHelper.GetLongId(),
                        Group = areaAttr != null ? areaAttr.RouteValue : type.Name,
                        Url = url,
                        Description = methodInfo.GetCustomAttributes(typeof(DescriptionAttribute), true)
                            .OfType<DescriptionAttribute>()
                            .FirstOrDefault()?.Description,
                        Method = method
                    });
                }
            }
        }

        if (apis.Count == 0) return Success("无新增Api数据！");

        if (await _apisService.CreateAsync(apis))
        {
            return Success("刷新Api成功！");
        }

        return Error("刷新Api失败！");
    }

    #endregion

    #endregion
}
