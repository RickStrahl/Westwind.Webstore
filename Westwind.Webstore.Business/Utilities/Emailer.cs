using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public bool SendEmail(string recipient, string subject, string messageText, string contentType = "text/plain")
        {
            // Set up the Email object - we'll use this for several emails
            var smtp = new SmtpClientNative();

            smtp.MailServer = wsApp.Configuration.Email.MailServer;
            smtp.UseSsl = wsApp.Configuration.Email.UseSsl;
            smtp.SenderEmail = wsApp.Configuration.Email.SenderEmail;
            smtp.SenderName = wsApp.Configuration.Email.SenderName;
            smtp.Username = wsApp.Configuration.Email.MailServerUsername;
            smtp.Password = wsApp.Configuration.Email.MailServerPassword;
            smtp.Recipient = recipient;

            // Also forward to admin (TEMPORARY)
            smtp.BCC = wsApp.Configuration.Email.CcList;
            smtp.ContentType = contentType;

            smtp.Subject = subject;
            smtp.Message = messageText;
            if (!smtp.SendMail())
            {
                SetError(WebStoreBusinessResources.EmailFailure + ": " + smtp.ErrorMessage);
                return false;
            }

            return true;
        }


        /// <summary>
        /// Send admin notifications for errors and other problems
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="subject"></param>
        /// <param name="messageText"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public bool SendAdminEmail(string recipient, string subject, string messageText, string contentType = "text/plain")
        {
            // Set up the Email object - we'll use this for several emails
            var smtp = new SmtpClientNative();

            smtp.MailServer = wsApp.Configuration.Email.MailServer;
            smtp.UseSsl = wsApp.Configuration.Email.UseSsl;
            smtp.SenderEmail = wsApp.Configuration.Email.SenderEmail;
            smtp.SenderName = wsApp.Configuration.Email.SenderName;
            smtp.Username = wsApp.Configuration.Email.MailServerUsername;
            smtp.Password = wsApp.Configuration.Email.MailServerPassword;
            smtp.Recipient = recipient;

            // Also forward to admin (TEMPORARY)
            smtp.BCC = wsApp.Configuration.Email.AdminCcList;
            smtp.ContentType = contentType;

            smtp.Subject = subject;
            smtp.Message = messageText;
            if (!smtp.SendMail())
            {
                SetError(WebStoreBusinessResources.EmailFailure + ": " + smtp.ErrorMessage);
                return false;
            }

            return true;
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
}
