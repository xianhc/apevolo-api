using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.Model;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using Ape.Volo.IBusiness.QueryModel;
using Ape.Volo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ape.Volo.Api.Controllers.Permission;

/// <summary>
/// 用户管理
/// </summary>
[Area("权限管理")]
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
    [Description("创建")]
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
    [Description("编辑")]
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
    [Description("删除")]
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
    [Description("更新个人信息")]
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
    [Description("更新密码")]
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
    [Description("更新邮箱")]
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
    [Description("更新头像")]
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
    [Description("查询")]
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
    [Description("导出")]
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
