using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Westwind.Utilities;

namespace Westwind.Webstore.Business.Utilities;

public class MailgunHttpEmailer
{
    private readonly string _domain;
    private readonly string _from;

    public string ApiBaseUrl { get; set; } = "https://api.mailgun.net";
    public string ApiKey { get; set; }
    public string ErrorMessage { get; set; }

    public MailgunHttpEmailer(string domain = null, string apiKey = null, string from = null)
    {
        var config = wsApp.Configuration.Email;
        _domain = domain ?? config.MailgunDomain;
        ApiKey = apiKey ?? config.MailgunApiKey;
        _from = from ?? config.SenderEmail;
        if (string.IsNullOrWhiteSpace(_from))
        {
            if (!string.IsNullOrEmpty((config.SenderName)))
                _from = $"{config.SenderName} <{config.SenderEmail}>";
            else
                _from = config.SenderEmail;
        }
    }

    public bool SendEmail(
        string to,
        string subject,
        string body,
        EmailModes emailMode = EmailModes.plain,
        bool noCCs = false)
    {
        return SendEmailInternal(
            recipient: to,
            subject: subject,
            messageText: body,
            emailMode: emailMode,
            noCCs: noCCs,
            cc: null,
            bcc: null,
            attachmentFiles: null,
            token: default);
    }

    public bool SendAdminEmail(
        string recipient,
        string subject,
        string messageText,
        EmailModes emailMode = EmailModes.plain,
        bool noCCs = false)
    {
        return SendEmailInternal(
            recipient: recipient,
            subject: subject,
            messageText: messageText,
            emailMode: emailMode,
            noCCs: noCCs,
            cc: null,
            bcc: null,
            attachmentFiles: null,
            token: default,
            isAdminEmail: true);
    }

    private bool SendEmailInternal(
        string recipient,
        string subject,
        string messageText,
        EmailModes emailMode,
        bool noCCs,
        string? cc,
        string? bcc,
        IEnumerable<string>? attachmentFiles,
        CancellationToken token,
        bool isAdminEmail = false)
    {
        try
        {
            token.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(_from) || string.IsNullOrWhiteSpace(_domain) || string.IsNullOrWhiteSpace(ApiKey))
                return true;

            var settings = new HttpClientRequestSettings
            {
                Url = $"{ApiBaseUrl.TrimEnd('/')}/v3/{_domain}/messages",
                HttpVerb = "POST",
                RequestFormPostMode = HttpFormPostMode.MultiPart,
                ThrowExceptions = true
            };

            settings.AddPostKey("from", _from);
            AddRecipients(settings, "to", recipient);
            settings.AddPostKey("subject", subject);

            if (!noCCs)
            {
                if (!string.IsNullOrWhiteSpace(cc))
                    AddRecipients(settings, "cc", cc);

                if (!string.IsNullOrWhiteSpace(bcc))
                    AddRecipients(settings, "bcc", bcc);

                var emailConfig = wsApp.Configuration.Email;
                if (isAdminEmail && !string.IsNullOrWhiteSpace(emailConfig.AdminCcList))
                    AddRecipients(settings, "bcc", emailConfig.AdminCcList);
                else if (!isAdminEmail && !string.IsNullOrWhiteSpace(emailConfig.CcList))
                    AddRecipients(settings, "cc", emailConfig.CcList);
            }

            settings.AddPostKey(emailMode == EmailModes.html ? "html" : "text", messageText);

            if (attachmentFiles != null)
            {
                foreach (var file in attachmentFiles.Where(file => !string.IsNullOrWhiteSpace(file)))
                {
                    if (!settings.AddPostFile("attachment", file, "application/octet-stream", Path.GetFileName(file)))
                        throw new FileNotFoundException($"Attachment file not found: {file}", file);
                }
            }

            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{ApiKey}"));
            settings.Headers["Authorization"] = $"Basic {auth}";

            using var response = HttpClientUtils.DownloadResponseMessage(settings);
            var json = response.Content?.ReadAsStringAsync(token).GetAwaiter().GetResult() ?? string.Empty;

            if (!response.IsSuccessStatusCode)
            {
                SetError($"Mailgun send failed: {(int)response.StatusCode} {response.ReasonPhrase}\n{json}");
                return false;
            }

            SetError();
            return true;
        }
        catch (Exception ex)
        {
            SetError(ex);
            return false;
        }
    }

    private static void AddRecipients(HttpClientRequestSettings settings, string key, string recipientList)
    {
        var recipients = recipientList
            .Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(recipient => recipient.Trim())
            .Where(recipient => !string.IsNullOrWhiteSpace(recipient));

        foreach (var recipient in recipients)
            settings.AddPostKey(key, recipient);
    }

    protected void SetError()
    {
        SetError("CLEAR");
    }

    protected void SetError(string message)
    {
        if (message == null || message == "CLEAR")
        {
            ErrorMessage = string.Empty;
            return;
        }

        ErrorMessage += message;
    }

    protected void SetError(Exception ex, bool checkInner = false)
    {
        if (ex == null)
        {
            ErrorMessage = string.Empty;
            return;
        }

        var error = ex;
        if (checkInner)
            error = error.GetBaseException();

        ErrorMessage = error.Message;
    }
}
