using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ape.Volo.Business.Base;
using Ape.Volo.Common.Caches.Redis.MessageQueue;
using Ape.Volo.Common.Exception;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.Model;
using Ape.Volo.Common.WebApp;
using Ape.Volo.IBusiness.Dto.Queued;
using Ape.Volo.IBusiness.Interface.Message.Email;
using Ape.Volo.IBusiness.Interface.Queued;
using Ape.Volo.IBusiness.QueryModel;
using ApeVolo.Entity.Queued;

namespace Ape.Volo.Business.Queued;

/// <summary>
/// 邮件队列接口实现
/// </summary>
public class QueuedEmailService : BaseServices<QueuedEmail>, IQueuedEmailService
{
    #region 字段

    private readonly IEmailMessageTemplateService _emailMessageTemplateService;
    private readonly IEmailAccountService _emailAccountService;

    #endregion

    #region 构造函数

    public QueuedEmailService(IEmailMessageTemplateService emailMessageTemplateService,
        IEmailAccountService emailAccountService, ApeContext apeContext) : base(apeContext)
    {
        _emailMessageTemplateService = emailMessageTemplateService;
        _emailAccountService = emailAccountService;
    }

    #endregion

    #region 基础方法

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="createUpdateQueuedEmailDto"></param>
    /// <returns></returns>
    public async Task<bool> CreateAsync(CreateUpdateQueuedEmailDto createUpdateQueuedEmailDto)
    {
        var queuedEmail = ApeContext.Mapper.Map<QueuedEmail>(createUpdateQueuedEmailDto);
        return await AddEntityAsync(queuedEmail);
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="queuedEmailDto"></param>
    /// <returns></returns>
    public async Task<bool> UpdateTriesAsync(QueuedEmailDto queuedEmailDto)
    {
        var queuedEmail = ApeContext.Mapper.Map<QueuedEmail>(queuedEmailDto);
        return await SugarRepository.UpdateAsync(queuedEmail) > 0;
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="createUpdateQueuedEmailDto"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(CreateUpdateQueuedEmailDto createUpdateQueuedEmailDto)
    {
        if (!await TableWhere(x => x.Id == createUpdateQueuedEmailDto.Id).AnyAsync())
        {
            throw new BadRequestException("数据不存在！");
        }

        var queuedEmail = ApeContext.Mapper.Map<QueuedEmail>(createUpdateQueuedEmailDto);
        return await UpdateEntityAsync(queuedEmail);
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

        return await LogicDelete<QueuedEmail>(x => ids.Contains(x.Id)) > 0;
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="queuedEmailQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    public async Task<List<QueuedEmailDto>> QueryAsync(QueuedEmailQueryCriteria queuedEmailQueryCriteria,
        Pagination pagination)
    {
        var whereExpression = GetWhereExpression(queuedEmailQueryCriteria);
        return ApeContext.Mapper.Map<List<QueuedEmailDto>>(
            await SugarRepository.QueryPageListAsync(whereExpression, pagination));
    }

    #endregion

    #region 扩展方法

    /// <summary>
    /// 变更邮箱验证码
    /// </summary>
    /// <param name="emailAddres"></param>
    /// <param name="messageTemplateName"></param>
    /// <returns></returns>
    public async Task<bool> ResetEmail(string emailAddres, string messageTemplateName)
    {
        var emailMessageTemplate =
            await _emailMessageTemplateService.TableWhere(x => x.Name == messageTemplateName).FirstAsync();
        if (emailMessageTemplate.IsNull())
            throw new BadRequestException($"{messageTemplateName} 获取邮件模板失败！");
        var emailAccount = await _emailAccountService.TableWhere(x => x.Id == emailMessageTemplate.EmailAccountId)
            .SingleAsync();

        //生成6位随机码
        var captcha = SixLaborsImageHelper.BuilEmailCaptcha(6);

        QueuedEmail queuedEmail = new QueuedEmail();
        queuedEmail.From = emailAccount.Email;
        queuedEmail.FromName = emailAccount.DisplayName;
        queuedEmail.To = emailAddres;
        queuedEmail.Priority = (int)QueuedEmailPriority.High;
        queuedEmail.Bcc = emailMessageTemplate.BccEmailAddresses;
        queuedEmail.Subject = emailMessageTemplate.Subject;
        queuedEmail.Body = emailMessageTemplate.Body.Replace("%captcha%", captcha);
        queuedEmail.SentTries = 0;
        queuedEmail.EmailAccountId = emailAccount.Id;

        bool isTrue = await SugarRepository.AddReturnBoolAsync(queuedEmail);
        if (isTrue)
        {
            await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.EmailCaptchaKey +
                                               queuedEmail.To.ToMd5String());
            await ApeContext.Cache.SetAsync(
                GlobalConstants.CacheKey.EmailCaptchaKey + queuedEmail.To.ToMd5String(), captcha,
                TimeSpan.FromMinutes(5), null);
            //进redis队列执行发送
            await ApeContext.Cache.GetDatabase()
                .ListLeftPushAsync(MqTopicNameKey.MailboxQueue, queuedEmail.Id.ToString());
        }

        return isTrue;
    }

    #endregion

    #region 条件表达式

    private static Expression<Func<QueuedEmail, bool>> GetWhereExpression(
        QueuedEmailQueryCriteria queuedEmailQueryCriteria)
    {
        Expression<Func<QueuedEmail, bool>> whereExpression = x => true;
        if (!queuedEmailQueryCriteria.Id.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(x => x.Id == queuedEmailQueryCriteria.Id);
        }

        if (queuedEmailQueryCriteria.MaxTries > 0)
        {
            whereExpression = whereExpression.AndAlso(x => x.SentTries < queuedEmailQueryCriteria.MaxTries);
        }

        if (!queuedEmailQueryCriteria.Form.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(x =>
                x.From.Contains(queuedEmailQueryCriteria.Form) ||
                x.FromName.Contains(queuedEmailQueryCriteria.Form));
        }

        if (!queuedEmailQueryCriteria.To.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(x =>
                x.To.Contains(queuedEmailQueryCriteria.To) || x.ToName.Contains(queuedEmailQueryCriteria.To));
        }

        if (queuedEmailQueryCriteria.IsSend.IsNotNull())
        {
            whereExpression = queuedEmailQueryCriteria.IsSend.ToBool()
                ? whereExpression.AndAlso(x => x.SendTime != null)
                : whereExpression.AndAlso(x => x.SendTime == null);
        }

        if (!queuedEmailQueryCriteria.CreateTime.IsNullOrEmpty() && queuedEmailQueryCriteria.CreateTime.Count > 1)
        {
            whereExpression = whereExpression.AndAlso(x =>
                x.CreateTime >= queuedEmailQueryCriteria.CreateTime[0] &&
                x.CreateTime <= queuedEmailQueryCriteria.CreateTime[1]);
        }

        return whereExpression;
    }

    #endregion
}
