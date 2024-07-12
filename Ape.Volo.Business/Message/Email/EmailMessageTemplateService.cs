using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
using Ape.Volo.Common;
using Ape.Volo.Common.Exception;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Model;
using Ape.Volo.Entity.Message.Email;
using Ape.Volo.IBusiness.Dto.Message.Email;
using Ape.Volo.IBusiness.Interface.Message.Email;
using Ape.Volo.IBusiness.QueryModel;

namespace Ape.Volo.Business.Message.Email;

/// <summary>
/// 邮件消息模板实现
/// </summary>
public class EmailMessageTemplateService : BaseServices<EmailMessageTemplate>, IEmailMessageTemplateService
{
    #region 构造函数

    public EmailMessageTemplateService()
    {
    }

    #endregion

    #region 基础方法

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="createUpdateEmailMessageTemplateDto"></param>
    /// <returns></returns>
    public async Task<bool> CreateAsync(CreateUpdateEmailMessageTemplateDto createUpdateEmailMessageTemplateDto)
    {
        var messageTemplate = await TableWhere(x => x.Name == createUpdateEmailMessageTemplateDto.Name).FirstAsync();
        if (messageTemplate.IsNotNull())
            throw new BadRequestException($"模板名称=>{createUpdateEmailMessageTemplateDto.Name}=>已存在!");

        return await AddEntityAsync(App.Mapper.MapTo<EmailMessageTemplate>(createUpdateEmailMessageTemplateDto));
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="createUpdateEmailMessageTemplateDto"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(CreateUpdateEmailMessageTemplateDto createUpdateEmailMessageTemplateDto)
    {
        var emailMessageTemplate = await TableWhere(x => x.Id == createUpdateEmailMessageTemplateDto.Id).FirstAsync();
        if (emailMessageTemplate.IsNull())
        {
            throw new BadRequestException("数据不存在！");
        }

        if (emailMessageTemplate.Name != createUpdateEmailMessageTemplateDto.Name &&
            await TableWhere(j => j.Name == emailMessageTemplate.Name).AnyAsync())
        {
            throw new BadRequestException($"模板名称=>{createUpdateEmailMessageTemplateDto.Name}=>已存在!");
        }

        return await UpdateEntityAsync(
            App.Mapper.MapTo<EmailMessageTemplate>(createUpdateEmailMessageTemplateDto));
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var messageTemplateList = await TableWhere(x => ids.Contains(x.Id)).ToListAsync();
        if (messageTemplateList.Count <= 0)
            throw new BadRequestException("数据不存在！");
        return await LogicDelete<EmailMessageTemplate>(x => ids.Contains(x.Id)) > 0;
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="messageTemplateQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    public async Task<List<EmailMessageTemplateDto>> QueryAsync(
        EmailMessageTemplateQueryCriteria messageTemplateQueryCriteria, Pagination pagination)
    {
        var whereExpression = GetWhereExpression(messageTemplateQueryCriteria);
        var queryOptions = new QueryOptions<EmailMessageTemplate>
        {
            Pagination = pagination,
            WhereLambda = whereExpression,
        };
        return App.Mapper.MapTo<List<EmailMessageTemplateDto>>(
            await SugarRepository.QueryPageListAsync(queryOptions));
    }

    #endregion

    #region 条件表达式

    private static Expression<Func<EmailMessageTemplate, bool>> GetWhereExpression(
        EmailMessageTemplateQueryCriteria messageTemplateQueryCriteria)
    {
        Expression<Func<EmailMessageTemplate, bool>> whereExpression = x => true;
        if (!messageTemplateQueryCriteria.Name.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(x => x.Name.Contains(messageTemplateQueryCriteria.Name));
        }

        if (messageTemplateQueryCriteria.IsActive.IsNotNull())
        {
            whereExpression = whereExpression.AndAlso(x => x.IsActive == messageTemplateQueryCriteria.IsActive);
        }

        if (!messageTemplateQueryCriteria.CreateTime.IsNullOrEmpty() &&
            messageTemplateQueryCriteria.CreateTime.Count > 1)
        {
            whereExpression = whereExpression.AndAlso(x =>
                x.CreateTime >= messageTemplateQueryCriteria.CreateTime[0] &&
                x.CreateTime <= messageTemplateQueryCriteria.CreateTime[1]);
        }

        return whereExpression;
    }

    #endregion
}
