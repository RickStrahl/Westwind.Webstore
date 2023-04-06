using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Westwind.Utilities;
using Westwind.Utilities.InternetTools;
using Westwind.Webstore.Business.Properties;

namespace Westwind.Webstore.Business.Utilities
{
    public class Emailer
    {
        /// <summary>
        /// Send customer facing emails for confirmations, password validation and recovery
        /// etc.
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="subject"></param>
        /// <param name="messageText"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public bool SendEmail(string recipient, string subject, string messageText, EmailModes emailMode = EmailModes.plain, bool noCCs = false)
        {
            var emailConfig = wsApp.Configuration.Email;

            var message = new MimeMessage();
            message.From.Add(CreateMailboxAddress(emailConfig.SenderName,
                emailConfig.SenderEmail));

            message.To.Add(CreateMailboxAddress(recipient));
            if (!noCCs)
            {
                message.Bcc.Add(CreateMailboxAddress(emailConfig.CcList ));
            }

            message.Subject = subject;
            message.Body = new TextPart ( emailMode.ToString() ) {
                Text = messageText
            };

            try
            {
                using (var client = new SmtpClient())
                {
                    // Server and Port (ie. smtp.server.com:587)
                    var serverTokens = emailConfig.MailServer.Split(':');
                    var mailServer = serverTokens[0];
                    var mailServerPort = 25;
                    if (serverTokens.Length > 1)
                        mailServerPort = Westwind.Utilities.StringUtils.ParseInt(serverTokens[1], 25);

                    client.Connect(mailServer, mailServerPort, emailConfig.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                    // Note: only needed if the SMTP server requires authentication
                    if (!string.IsNullOrEmpty(emailConfig.MailServerUsername))
                    {
                        client.Authenticate(emailConfig.MailServerUsername,
                            emailConfig.MailServerPassword);
                    }

                    client.Send(message);
                    client.Disconnect(true);

                    return true;
                }
            }
            catch (Exception ex)
            {
                SetError(ex);
                return false;
            }

        }


        /// <summary>
        /// Send admin notifications for errors and other problems
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="subject"></param>
        /// <param name="messageText"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public bool SendAdminEmail(string recipient, string subject, string messageText, EmailModes emailMode = EmailModes.plain, bool noCCs = false)
        {
            var emailConfig = wsApp.Configuration.Email;

            var message = new MimeMessage();
            message.From.Add(CreateMailboxAddress(emailConfig.SenderName,
                emailConfig.SenderEmail));

            message.To.Add(CreateMailboxAddress(recipient));
            if (!noCCs)
            {
                message.Bcc.Add(CreateMailboxAddress(emailConfig.AdminCcList ));
            }

            message.Subject = subject;
            message.Body = new TextPart ( emailMode.ToString() ) {
                Text = messageText
            };

            try
            {
                using (var client = new SmtpClient())
                {
                    // Server and Port (ie. smtp.server.com:587)
                    var serverTokens = emailConfig.MailServer.Split(':');
                    var mailServer = serverTokens[0];
                    var mailServerPort = 25;
                    if (serverTokens.Length > 1)
                        mailServerPort = Westwind.Utilities.StringUtils.ParseInt(serverTokens[1], 25);

                    client.Connect(mailServer, mailServerPort, emailConfig.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                    if (!string.IsNullOrEmpty(emailConfig.MailServerUsername))
                    {
                        // Note: only needed if the SMTP server requires authentication
                        client.Authenticate(emailConfig.MailServerUsername,
                            emailConfig.MailServerPassword);
                    }

                    client.Send(message);
                    client.Disconnect(true);

                    return true;
                }
            }
            catch (Exception ex)
            {
                SetError(ex);
                return false;
            }
        }


        /// <summary>
        /// Splits an email from "John Doe <doe@email.com>" into
        /// name and email parts.
        /// If only an email address is proved "doe@email.com"
        /// both values are returned as the email address.
        /// </summary>
        /// <param name="emailAddress">Input email address either as Name <email> or just an email address</param>
        /// <returns>Name and Address or only the Address or empty strings if null or empty passed in</returns>
        public MailboxAddress CreateMailboxAddress(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                return new MailboxAddress("","");

            string origEmail = emailAddress?.Trim();
            string email = origEmail;
            string name = email;

            if (origEmail.Contains("<"))
            {
                var tokens = origEmail.Split(" <");
                name = tokens[0]?.Trim();
                email = tokens[1]?.TrimEnd(  ' ', '>');
            }

            return new MailboxAddress(name, email);
        }

        /// <summary>
        /// Creates a new Mail Box address
        /// </summary>
        /// <param name="name"></param>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public MailboxAddress CreateMailboxAddress(string name, string emailAddress)
        {
            return new MailboxAddress(name, emailAddress);
        }


        #region Errors

        public string ErrorMessage { get; set; }

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
            }
            else
            {
                Exception e = ex;
                if (checkInner)
                    e = e.GetBaseException();

                ErrorMessage = e.Message;
            }
        }

        #endregion
    }

    public enum EmailModes
    {
        plain,
        html
    }
}
