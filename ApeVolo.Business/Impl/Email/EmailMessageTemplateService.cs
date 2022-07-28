using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Email;
using ApeVolo.IBusiness.Dto.Email;
using ApeVolo.IBusiness.EditDto.Email;
using ApeVolo.IBusiness.Interface.Email;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Email;
using AutoMapper;

namespace ApeVolo.Business.Impl.Email;

/// <summary>
/// 邮件消息模板实现
/// </summary>
public class EmailMessageTemplateService : BaseServices<EmailMessageTemplate>, IEmailMessageTemplateService
{
    #region 构造函数

    public EmailMessageTemplateService(IEmailMessageTemplateRepository messageTemplateRepository, IMapper mapper)
    {
        BaseDal = messageTemplateRepository;
        Mapper = mapper;
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
        var messageTemplate =
            await QueryFirstAsync(x => x.Name == createUpdateEmailMessageTemplateDto.Name);
        if (messageTemplate.IsNotNull())
            throw new BadRequestException($"邮箱模板=>{messageTemplate.Name}=>已存在!");

        return await AddEntityAsync(Mapper.Map<EmailMessageTemplate>(createUpdateEmailMessageTemplateDto));
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="createUpdateEmailMessageTemplateDto"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(CreateUpdateEmailMessageTemplateDto createUpdateEmailMessageTemplateDto)
    {
        if (!await IsExistAsync(x => x.Id == createUpdateEmailMessageTemplateDto.Id))
            throw new BadRequestException($"邮箱模板=>{createUpdateEmailMessageTemplateDto.Name}=>不存在！");

        return await UpdateEntityAsync(Mapper.Map<EmailMessageTemplate>(createUpdateEmailMessageTemplateDto));
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var messageTemplateList = await QueryByIdsAsync(ids);
        if (messageTemplateList.Count <= 0)
            throw new BadRequestException("无可删除数据!");
        return await DeleteEntityListAsync(messageTemplateList);
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

        return Mapper.Map<List<EmailMessageTemplateDto>>(
            await BaseDal.QueryPageListAsync(whereExpression, pagination));
    }

    #endregion
}