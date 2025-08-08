using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Westwind.AspNetCore.Views;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Utilities;
using Westwind.Webstore.Web.Views;

namespace Westwind.Webstore.Web.App
{
    public class AppUtils
    {


        public static JsonSerializerSettings JsonFormatterSettingsForRecursive
        {
            get
            {
                if (jsonSerializerSettings != null)
                    return jsonSerializerSettings;

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
#if DEBUG
                    Formatting = Formatting.Indented
#endif

                };
                settings.Converters.Add(new StringEnumConverter() { NamingStrategy = new CamelCaseNamingStrategy() });

                jsonSerializerSettings = settings;
                return settings;
            }
        }
        private static JsonSerializerSettings jsonSerializerSettings;


        /// <summary>
        /// Sends a user administration email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static bool SendEmail(string email, string title, string body, out string error, bool noHtml = false, bool noCCs = false)
        {
            var emailer = new Emailer();
            var result =emailer.SendEmail(email,
                title,
                body,
                noHtml ? EmailModes.plain : EmailModes.html,
                noCCs);

            error = null;
            if (!result)
            {
                error = emailer.ErrorMessage;
            }

            return result;
        }

        public static async Task<bool> SendOrderConfirmationEmail(OrderFormViewModel model,
            ControllerContext controllerContext)
        {
            var invoice = model.InvoiceModel.Invoice;
            string confirmationHtml = await ViewRenderer.RenderViewToStringAsync("EmailConfirmation", model, controllerContext);


            var emailer = new Emailer();
            return emailer.SendEmail(string.IsNullOrEmpty(invoice.BillingAddress.Email) ? invoice.Customer.Email : invoice.BillingAddress.Email,
                $"{wsApp.Configuration.ApplicationName} Order Confirmation #{invoice.InvoiceNumber}",
                confirmationHtml,
                EmailModes.html);
        }

        #region HTML Utilities

        /// <summary>
        /// Markdown to Html
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static string MarkdownToHtml(string markdown)
        {
            return Westwind.AspNetCore.Markdown.Markdown.Parse(markdown);
        }

        /// <summary>
        /// Self-contained CSS styles that can be embedded into email and other
        /// HTML rendered for embedding into non Web applications.
        /// </summary>
        public static HtmlString EmbeddedCssStyles
        {
            get
            {
                if (_embeddedCssStyles == null)
                {
                    var file = Path.Combine(wsApp.Constants.StartupFolder, "wwwroot", "css", "EmbeddedStyles.css");
                    _embeddedCssStyles = new HtmlString(File.ReadAllText(file));
                }

                return _embeddedCssStyles;
            }
        }
        private static HtmlString _embeddedCssStyles;

        /// <summary>
        ///
        /// </summary>
        /// <param name="htmlBody"></param>
        /// <returns></returns>
        public static string GetEmbeddedHtmlDocument(string htmlBody)
        {
            return $"<html><head><style>\n{AppUtils.EmbeddedCssStyles}\n</style></head><body>\n{htmlBody}\n</body></html>";
        }

        #endregion

    }
}
