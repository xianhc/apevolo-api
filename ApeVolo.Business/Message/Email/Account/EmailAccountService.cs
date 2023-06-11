using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Message.Email;
using ApeVolo.IBusiness.Dto.Message.Email.Account;
using ApeVolo.IBusiness.ExportModel.Message.Email.Account;
using ApeVolo.IBusiness.Interface.Message.Email.Account;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Message.Email.Account;
using AutoMapper;

namespace ApeVolo.Business.Message.Email.Account;

public class EmailAccountService : BaseServices<EmailAccount>, IEmailAccountService
{
    #region 字段

    #endregion

    #region 构造函数

    public EmailAccountService(IEmailAccountRepository emailAccountRepository, IMapper mapper, ICurrentUser currentUser)
    {
        BaseDal = emailAccountRepository;
        Mapper = mapper;
        CurrentUser = currentUser;
    }

    #endregion

    #region 基础方法

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="createUpdateEmailAccountDto"></param>
    /// <returns></returns>
    public async Task<bool> CreateAsync(CreateUpdateEmailAccountDto createUpdateEmailAccountDto)
    {
        if (await IsExistAsync(x => x.Email == createUpdateEmailAccountDto.Email))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("EmailAccount"),
                createUpdateEmailAccountDto.Email));
        }

        var emailAccount = Mapper.Map<EmailAccount>(createUpdateEmailAccountDto);
        return await AddEntityAsync(emailAccount);
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="createUpdateEmailAccountDto"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(CreateUpdateEmailAccountDto createUpdateEmailAccountDto)
    {
        var oldEmailAccount =
            await QueryFirstAsync(x => x.Id == createUpdateEmailAccountDto.Id);
        if (oldEmailAccount.IsNull())
        {
            throw new BadRequestException(Localized.Get("DataNotExist"));
        }

        if (oldEmailAccount.Email != createUpdateEmailAccountDto.Email &&
            await IsExistAsync(j => j.Id == createUpdateEmailAccountDto.Id))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("EmailAccount"),
                createUpdateEmailAccountDto.Email));
        }

        var emailAccount = Mapper.Map<EmailAccount>(createUpdateEmailAccountDto);
        return await UpdateEntityAsync(emailAccount);
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var emailAccounts = await QueryByIdsAsync(ids);
        if (emailAccounts.Count < 1)
            throw new BadRequestException("无可删除数据!");

        return await DeleteEntityListAsync(emailAccounts);
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="emailAccountQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    public async Task<List<EmailAccountDto>> QueryAsync(EmailAccountQueryCriteria emailAccountQueryCriteria,
        Pagination pagination)
    {
        Expression<Func<EmailAccount, bool>> whereExpression = x => true;
        if (!emailAccountQueryCriteria.Username.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(x => x.Username.Contains(emailAccountQueryCriteria.Username));
        }

        if (!emailAccountQueryCriteria.DisplayName.IsNullOrEmpty())
        {
            whereExpression =
                whereExpression.AndAlso(x => x.DisplayName.Contains(emailAccountQueryCriteria.DisplayName));
        }

        if (!emailAccountQueryCriteria.CreateTime.IsNullOrEmpty() && emailAccountQueryCriteria.CreateTime.Count > 1)
        {
            whereExpression = whereExpression.AndAlso(x =>
                x.CreateTime >= emailAccountQueryCriteria.CreateTime[0] &&
                x.CreateTime <= emailAccountQueryCriteria.CreateTime[1]);
        }

        return Mapper.Map<List<EmailAccountDto>>(await BaseDal.QueryPageListAsync(whereExpression, pagination));
    }

    public async Task<List<ExportBase>> DownloadAsync(EmailAccountQueryCriteria emailAccountQueryCriteria)
    {
        var emailAccounts = await QueryAsync(emailAccountQueryCriteria, new Pagination { PageSize = 9999 });
        List<ExportBase> emailAccountExports = new List<ExportBase>();
        emailAccountExports.AddRange(emailAccounts.Select(x => new EmailAccountExport()
        {
            Email = x.Email,
            DisplayName = x.DisplayName,
            Host = x.Host,
            Port = x.Port,
            Username = x.Username,
            EnableSsl = x.EnableSsl ? BoolState.True : BoolState.False,
            UseDefaultCredentials = x.UseDefaultCredentials ? BoolState.True : BoolState.False,
            CreateTime = x.CreateTime
        }));
        return emailAccountExports;
    }

    #endregion
}