using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using ApeVolo.Api.ActionExtension.Json;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ApeVolo.Api.Controllers;

/// <summary>
/// 用户管理
/// </summary>
[Area("User")]
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
    [Description("新增用户")]
    [Route("create")]
    public async Task<ActionResult<object>> Create([FromBody] CreateUpdateUserDto createUpdateUserDto)
    {
        RequiredHelper.IsValid(createUpdateUserDto);
        await _userService.CreateAsync(createUpdateUserDto);
        return Create("添加成功，默认密码123456");
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="createUpdateUserDto"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [HttpPut]
    [Description("更新用户")]
    [Route("edit")]
    public async Task<ActionResult<object>> Update([FromBody] CreateUpdateUserDto createUpdateUserDto)
    {
        await _userService.UpdateAsync(createUpdateUserDto);
        return NoContent();
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    [Description("删除用户")]
    [NoJsonParamter]
    public async Task<ActionResult<object>> Delete([FromBody] HashSet<long> ids)
    {
        if (ids == null || ids.Count < 1)
        {
            return Error("ids is null");
        }

        await _userService.DeleteAsync(ids);
        return Success();
    }

    [HttpPut]
    [Route("center")]
    [Description("更新个人信息")]
    [ApeVoloOnline]
    public async Task<ActionResult<object>> UpdateCenterAsync(UpdateUserCenterDto updateUserCenterDto)
    {
        RequiredHelper.IsValid(updateUserCenterDto);
        await _userService.UpdateCenterAsync(updateUserCenterDto);
        return NoContent();
    }

    [HttpPost]
    [Route("update/password")]
    [Description("更新密码")]
    public async Task<ActionResult<object>> UpdatePasswordAsync(UpdateUserPassDto updateUserPassDto)
    {
        RequiredHelper.IsValid(updateUserPassDto);
        await _userService.UpdatePasswordAsync(updateUserPassDto);
        return Success();
    }

    [HttpPost]
    [Route("update/email")]
    [Description("更新邮箱")]
    [ApeVoloOnline]
    public async Task<ActionResult<object>> UpdateEmail(UpdateUserEmailDto updateUserEmailDto)
    {
        RequiredHelper.IsValid(updateUserEmailDto);
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
    [Description("查看用户列表")]
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
    [Description("导出用户列表")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(UserQueryCriteria userQueryCriteria)
    {
        var exportRowModels = await _userService.DownloadAsync(userQueryCriteria);

        var filepath = ExcelHelper.ExportData(exportRowModels, "用户列表");

        var provider = new FileExtensionContentTypeProvider();
        FileInfo fileInfo = new FileInfo(filepath);
        var ext = fileInfo.Extension;
        new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contently);
        return File(await System.IO.File.ReadAllBytesAsync(filepath), contently ?? "application/octet-stream",
            fileInfo.Name);
    }

    #endregion
}