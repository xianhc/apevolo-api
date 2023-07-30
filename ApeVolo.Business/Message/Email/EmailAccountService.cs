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
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Message.Email;
using ApeVolo.IBusiness.Dto.Message.Email;
using ApeVolo.IBusiness.ExportModel.Message.Email.Account;
using ApeVolo.IBusiness.Interface.Message.Email;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.Business.Message.Email;

public class EmailAccountService : BaseServices<EmailAccount>, IEmailAccountService
{
    #region 构造函数

    public EmailAccountService(ApeContext apeContext) : base(apeContext)
    {
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
        if (await TableWhere(x => x.Email == createUpdateEmailAccountDto.Email).AnyAsync())
        {
            throw new BadRequestException($"邮箱账户=>{createUpdateEmailAccountDto.Email}=>已存在!");
        }

        var emailAccount = ApeContext.Mapper.Map<EmailAccount>(createUpdateEmailAccountDto);
        return await AddEntityAsync(emailAccount);
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="createUpdateEmailAccountDto"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(CreateUpdateEmailAccountDto createUpdateEmailAccountDto)
    {
        var oldEmailAccount = await TableWhere(x => x.Id == createUpdateEmailAccountDto.Id).FirstAsync();
        if (oldEmailAccount.IsNull())
        {
            throw new BadRequestException("数据不存在！");
        }

        if (oldEmailAccount.Email != createUpdateEmailAccountDto.Email &&
            await TableWhere(j => j.Email == createUpdateEmailAccountDto.Email).AnyAsync())
        {
            throw new BadRequestException($"邮箱账户=>{createUpdateEmailAccountDto.Email}=>已存在!");
        }

        var emailAccount = ApeContext.Mapper.Map<EmailAccount>(createUpdateEmailAccountDto);
        return await UpdateEntityAsync(emailAccount);
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var emailAccounts = await TableWhere(x => ids.Contains(x.Id)).ToListAsync();
        if (emailAccounts.Count < 1)
            throw new BadRequestException("无可删除数据!");

        return await LogicDelete<EmailAccount>(x => ids.Contains(x.Id)) > 0;
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
        var whereExpression = GetWhereExpression(emailAccountQueryCriteria);
        return ApeContext.Mapper.Map<List<EmailAccountDto>>(
            await SugarRepository.QueryPageListAsync(whereExpression, pagination));
    }

    public async Task<List<ExportBase>> DownloadAsync(EmailAccountQueryCriteria emailAccountQueryCriteria)
    {
        var whereExpression = GetWhereExpression(emailAccountQueryCriteria);
        var emailAccounts = await TableWhere(whereExpression).ToListAsync();
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

    #region 条件表达式

    private static Expression<Func<EmailAccount, bool>> GetWhereExpression(
        EmailAccountQueryCriteria emailAccountQueryCriteria)
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

        return whereExpression;
    }

    #endregion
}
