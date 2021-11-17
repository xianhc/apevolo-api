using ApeVolo.Business.Base;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Dto.Email;
using ApeVolo.IBusiness.EditDto.Email;
using ApeVolo.IBusiness.Interface.Email;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Email;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Entity.Do.Email;

namespace ApeVolo.Business.Impl.Email
{
    /// <summary>
    /// 邮件消息模板实现
    /// </summary>
    public class MessageTemplateService : BaseServices<MessageTemplate>, IEmailMessageTemplateService
    {
        #region 构造函数

        public MessageTemplateService(IMessageTemplateRepository messageTemplateRepository, IMapper mapper)
        {
            _baseDal = messageTemplateRepository;
            _mapper = mapper;
        }

        #endregion

        #region 基础方法

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="createUpdateMessageTemplateDto"></param>
        /// <returns></returns>
        public async Task<bool> CreateAsync(CreateUpdateMessageTemplateDto createUpdateMessageTemplateDto)
        {
            var messageTemplate =
                await QueryFirstAsync(x => x.IsDeleted == false && x.Name == createUpdateMessageTemplateDto.Name);
            if (messageTemplate.IsNotNull())
                throw new BadRequestException($"邮箱模板=>{messageTemplate.Name}=>已存在!");

            return await AddEntityAsync(_mapper.Map<MessageTemplate>(createUpdateMessageTemplateDto));
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="createUpdateMessageTemplateDto"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(CreateUpdateMessageTemplateDto createUpdateMessageTemplateDto)
        {
            if (!await IsExistAsync(x => x.IsDeleted == false && x.Id == createUpdateMessageTemplateDto.Id))
                throw new BadRequestException($"邮箱模板=>{createUpdateMessageTemplateDto.Name}=>不存在！");

            return await UpdateEntityAsync(_mapper.Map<MessageTemplate>(createUpdateMessageTemplateDto));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(HashSet<string> ids)
        {
            var messageTemplateList = await QueryByIdsAsync(ids);
            if (messageTemplateList.Count <= 0)
                throw new BadRequestException($"无可删除数据!");
            return await DeleteEntityListAsync(messageTemplateList);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="messageTemplateQueryCriteria"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public async Task<List<MessageTemplateDto>> QueryAsync(
            MessageTemplateQueryCriteria messageTemplateQueryCriteria, Pagination pagination)
        {
            Expression<Func<MessageTemplate, bool>> whereExpression = x => (x.IsDeleted == false);
            if (!messageTemplateQueryCriteria.Name.IsNullOrEmpty())
            {
                whereExpression = whereExpression.And(x => x.Name.Contains(messageTemplateQueryCriteria.Name));
            }

            if (messageTemplateQueryCriteria.IsActive.IsNotNull())
            {
                whereExpression = whereExpression.And(x => x.IsActive == messageTemplateQueryCriteria.IsActive);
            }

            if (!messageTemplateQueryCriteria.CreateTime.IsNullOrEmpty() &&
                messageTemplateQueryCriteria.CreateTime.Count > 1)
            {
                whereExpression = whereExpression.And(x =>
                    x.CreateTime >= messageTemplateQueryCriteria.CreateTime[0] &&
                    x.CreateTime <= messageTemplateQueryCriteria.CreateTime[1]);
            }

            return _mapper.Map<List<MessageTemplateDto>>(
                await _baseDal.QueryPageListAsync(whereExpression, pagination));
        }

        #endregion
    }
}