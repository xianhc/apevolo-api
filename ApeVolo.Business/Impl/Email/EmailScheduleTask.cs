using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.DI;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Interface.Email;
using ApeVolo.IBusiness.QueryModel;
using log4net;

namespace ApeVolo.Business.Impl.Email
{
    /// <summary>
    /// 电子邮件任务
    /// </summary>
    public class EmailScheduleTask : IEmailScheduleTask, IDependencyService
    {
        #region Fields

        private readonly ILog _log = LogManager.GetLogger(typeof(EmailScheduleTask));
        private readonly IEmailAccountService _emailAccountService;
        private readonly IEmailSender _emailSender;
        private readonly IQueuedEmailService _queuedEmailService;

        #endregion

        #region Ctor

        public EmailScheduleTask(IEmailAccountService emailAccountService,
            IEmailSender emailSender,
            IQueuedEmailService queuedEmailService)
        {
            _emailAccountService = emailAccountService;
            _emailSender = emailSender;
            _queuedEmailService = queuedEmailService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public virtual async Task ExecuteAsync(long emailId = 0)
        {
            var maxTries = 3;
            QueuedEmailQueryCriteria queuedEmailQueryCriteria = new QueuedEmailQueryCriteria();
            queuedEmailQueryCriteria.MaxTries = maxTries;
            queuedEmailQueryCriteria.IsSend = false;
            if (emailId > 0)
            {
                queuedEmailQueryCriteria.Id = emailId;
            }

            var queuedEmails = await _queuedEmailService.QueryAsync(
                queuedEmailQueryCriteria,
                new Pagination
                {
                    PageIndex = 1, PageSize = 100,
                    SortFields = new List<string> { "priority asc", "create_time asc" }
                });
            foreach (var queuedEmail in queuedEmails)
            {
                var bcc = string.IsNullOrWhiteSpace(queuedEmail.Bcc)
                    ? null
                    : queuedEmail.Bcc.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var cc = string.IsNullOrWhiteSpace(queuedEmail.CC)
                    ? null
                    : queuedEmail.CC.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    await _emailSender.SendEmailAsync(
                        await _emailAccountService.QueryFirstAsync(x =>
                            x.IsDeleted == false && x.Id == queuedEmail.EmailAccountId),
                        queuedEmail.Subject,
                        queuedEmail.Body,
                        queuedEmail.From,
                        queuedEmail.FromName,
                        queuedEmail.To,
                        queuedEmail.ToName,
                        queuedEmail.ReplyTo,
                        queuedEmail.ReplyToName,
                        bcc,
                        cc);

                    queuedEmail.SendTime = DateTime.Now;
                }
                catch (Exception exc)
                {
                    _log.Error($"Error sending e-mail. {exc.Message}");
                }
                finally
                {
                    queuedEmail.SentTries += 1;
                    queuedEmail.UpdateBy = "EmailScheduleTask";
                    queuedEmail.UpdateTime = DateTime.Now;
                    await _queuedEmailService.UpdateTriesAsync(queuedEmail);
                }
            }
        }

        #endregion
    }
}