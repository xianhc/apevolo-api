using System;
using System.ComponentModel;
using System.Threading.Tasks;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Dto.Permission;
using ApeVolo.IBusiness.Interface.Permission;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApeVolo.Api.Controllers.Permission;

/// <summary>
/// 用户管理
/// </summary>
[Area("Permission")]
[Route("/api/user")]
public class UserController : BaseApiController
{
    #region 字段

    private readonly IUserService _userService;

    #endregion

    #region 构造函数

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 新增用户
    /// </summary>
    /// <param name="createUpdateUserDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Description("Add")]
    [Route("create")]
    public async Task<ActionResult<object>> Create([FromBody] CreateUpdateUserDto createUpdateUserDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _userService.CreateAsync(createUpdateUserDto);
        return Create();
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="createUpdateUserDto"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [HttpPut]
    [Description("Edit")]
    [Route("edit")]
    public async Task<ActionResult<object>> Update([FromBody] CreateUpdateUserDto createUpdateUserDto)
    {
        await _userService.UpdateAsync(createUpdateUserDto);
        return NoContent();
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="idCollection"></param>
    /// <returns></returns>
    [HttpDelete]
    [Description("Delete")]
    [Route("delete")]
    public async Task<ActionResult<object>> Delete([FromBody] IdCollection idCollection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _userService.DeleteAsync(idCollection.IdArray);
        return Success();
    }

    [HttpPut]
    [Route("center")]
    [Description("UpdatePersonalInfo")]
    [ApeVoloOnline]
    public async Task<ActionResult<object>> UpdateCenterAsync([FromBody] UpdateUserCenterDto updateUserCenterDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _userService.UpdateCenterAsync(updateUserCenterDto);
        return NoContent();
    }

    [HttpPost]
    [Route("update/password")]
    [Description("UpdatePassword")]
    [ApeVoloOnline]
    public async Task<ActionResult<object>> UpdatePasswordAsync([FromBody] UpdateUserPassDto updateUserPassDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _userService.UpdatePasswordAsync(updateUserPassDto);
        return Success();
    }

    [HttpPost]
    [Route("update/email")]
    [Description("UpdateEmail")]
    [ApeVoloOnline]
    public async Task<ActionResult<object>> UpdateEmail([FromBody] UpdateUserEmailDto updateUserEmailDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _userService.UpdateEmailAsync(updateUserEmailDto);
        return Success();
    }

    [HttpOptions, HttpPost]
    [Route("update/avatar")]
    [Description("UpdateAvatar")]
    [ApeVoloOnline]
    public async Task<ActionResult<object>> UpdateAvatar([FromForm] IFormFile avatar) //多文件使用  IFormFileCollection
    {
        if (avatar == null)
        {
            return Error();
        }

        await _userService.UpdateAvatarAsync(avatar);
        return Success();
    }


    /// <summary>
    /// 查看用户列表
    /// </summary>
    /// <param name="userQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("List")]
    [Route("query")]
    public async Task<ActionResult<object>> Query(UserQueryCriteria userQueryCriteria,
        Pagination pagination)
    {
        var list = await _userService.QueryAsync(userQueryCriteria, pagination);
        return new ActionResultVm<UserDto>
        {
            Content = list,
            TotalElements = pagination.TotalElements
        }.ToJson();
    }

    /// <summary>
    /// 导出用户列表
    /// </summary>
    /// <param name="userQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("Export")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(UserQueryCriteria userQueryCriteria)
    {
        var userExports = await _userService.DownloadAsync(userQueryCriteria);

        // var filepath = ExcelHelper.ExportData(exportRowModels, Localized.Get("User"));
        //
        // FileInfo fileInfo = new FileInfo(filepath);
        // var ext = fileInfo.Extension;
        // new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contently);
        // return File(await global::System.IO.File.ReadAllBytesAsync(filepath), contently ?? "application/octet-stream",
        //     fileInfo.Name);
        var data = new ExcelHelper().GenerateExcel(userExports, out var mimeType);
        return File(data, mimeType);
    }

    #endregion
}
