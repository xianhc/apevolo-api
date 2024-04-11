using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ape.Volo.Entity.Message.Email;
using Ape.Volo.IBusiness.Interface.Message.Email;
using MimeKit;
using MimeKit.Text;

namespace Ape.Volo.Business.Message.Email;

/// <summary>
/// 邮件发送
/// </summary>
public class EmailSender : IEmailSender
{
    #region Fields

    private readonly ISmtpBuilder _smtpBuilder;

    #endregion

    #region Ctor

    public EmailSender(ISmtpBuilder smtpBuilder)
    {
        _smtpBuilder = smtpBuilder;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Create an file attachment for the binary data
    /// </summary>
    /// <param name="attachmentFileName">Attachment file name</param>
    /// <param name="binaryContent">The array of unsigned bytes from which to create the attachment stream.</param>
    /// <param name="cDate">Creation date and time for the specified file or directory</param>
    /// <param name="mDate">Date and time that the specified file or directory was last written to</param>
    /// <param name="rDate">Date and time that the specified file or directory was last access to.</param>
    /// <returns>A leaf-node MIME part that contains an attachment.</returns>
    protected MimePart CreateMimeAttachment(string attachmentFileName, byte[] binaryContent, DateTime cDate,
        DateTime mDate, DateTime rDate)
    {
        if (!ContentType.TryParse(MimeTypes.GetMimeType(attachmentFileName), out var mimeContentType))
            mimeContentType = new ContentType("application", "octet-stream");

        return new MimePart(mimeContentType)
        {
            FileName = attachmentFileName,
            Content = new MimeContent(new MemoryStream(binaryContent)),
            ContentDisposition = new ContentDisposition
            {
                CreationDate = cDate,
                ModificationDate = mDate,
                ReadDate = rDate
            }
        };
    }

    #endregion

    #region Methods

    /// <summary>
    /// Sends an email
    /// </summary>
    /// <param name="emailAccount">Email account to use</param>
    /// <param name="subject">Subject</param>
    /// <param name="body">Body</param>
    /// <param name="fromAddress">From address</param>
    /// <param name="fromName">From display name</param>
    /// <param name="toAddress">To address</param>
    /// <param name="toName">To display name</param>
    /// <param name="replyToAddress">ReplyTo address</param>
    /// <param name="replyToName">ReplyTo display name</param>
    /// <param name="bcc">BCC addresses list</param>
    /// <param name="cc">CC addresses list</param>
    /// <param name="attachmentFilePath">Attachment file path</param>
    /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
    /// <param name="attachedDownloadId">Attachment download ID (another attachment)</param>
    /// <param name="headers">Headers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<bool> SendEmailAsync(EmailAccount emailAccount, string subject, string body,
        string fromAddress, string fromName, string toAddress, string toName,
        string replyToAddress = null, string replyToName = null,
        IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
        string attachmentFilePath = null, string attachmentFileName = null,
        int attachedDownloadId = 0, IDictionary<string, string> headers = null)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(fromName, fromAddress));
        message.To.Add(new MailboxAddress(toName, toAddress));

        if (!string.IsNullOrEmpty(replyToAddress))
        {
            message.ReplyTo.Add(new MailboxAddress(replyToName, replyToAddress));
        }

        //BCC
        if (bcc != null)
        {
            foreach (var address in bcc.Where(bccValue => !string.IsNullOrWhiteSpace(bccValue)))
            {
                message.Bcc.Add(new MailboxAddress("", address.Trim()));
            }
        }

        //CC
        if (cc != null)
        {
            foreach (var address in cc.Where(ccValue => !string.IsNullOrWhiteSpace(ccValue)))
            {
                message.Cc.Add(new MailboxAddress("", address.Trim()));
            }
        }

        //content
        message.Subject = subject;

        //headers
        if (headers != null)
            foreach (var header in headers)
            {
                message.Headers.Add(header.Key, header.Value);
            }

        var multipart = new Multipart("mixed")
        {
            new TextPart(TextFormat.Html) { Text = body }
        };
        message.Body = multipart;

        //send email
        using var smtpClient = await _smtpBuilder.BuildAsync(emailAccount);
        var result = await smtpClient.SendAsync(message);
        await smtpClient.DisconnectAsync(true);
        return result != null && result.Contains("OK", StringComparison.CurrentCultureIgnoreCase);
    }

    #endregion
}
