using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Braintree;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Westwind.AspNetCore;
using Westwind.AspNetCore.Extensions;
using Westwind.AspNetCore.Markdown;
using Westwind.Globalization.AspnetCore.Utilities;
using Westwind.Utilities;
using Westwind.Webstore.Business;
using Westwind.Webstore.Web.App;
using Westwind.Webstore.Web.Models;

using Westwind.Webstore.Web.Views;

namespace Westwind.Webstore.Web.Controllers
{
    public class HomeController : WebStoreBaseController
    {
        public BusinessFactory BusinessFactory { get; }
        public IHttpContextAccessor ContextAccessor { get; }


        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,  BusinessFactory businessFactory, IHttpContextAccessor contextAccessor)
        {
            BusinessFactory = businessFactory;
            ContextAccessor = contextAccessor;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = CreateViewModel<ProductListViewModel>();

            var bus = BusinessFactory.GetProductBusiness();
            var productList = bus.GetItems(new InventoryItemsFilter { ListOrder = InventoryListOrder.Specials});

            model.ItemList = productList;

            return View("Index", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            var exceptionHandlerPath=  HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exceptionHandler =   HttpContext.Features.Get<IExceptionHandlerFeature>();

            var mainException = exceptionHandler?.Error;
            var pathException = exceptionHandlerPath?.Error;

            var model = new ErrorViewModel {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Error = pathException,
                StatusCode = (int) HttpStatusCode.InternalServerError
            };

            var context = ContextAccessor.HttpContext;

            var header = StringUtils.GetLines(mainException.Message).FirstOrDefault();

            if (pathException is HttpRequestException httpEx)
            {
                model.StatusCode = (int) httpEx.StatusCode.Value;
                Response.StatusCode = (int) model.StatusCode;

            }
            else
            {
                if (context?.Request != null)
                {
                    model.HttpVerb = context?.Request?.Method?.ToString();

                    if (context != null &&
                        model.HttpVerb.Equals("post", StringComparison.InvariantCultureIgnoreCase) ||
                        model.HttpVerb.Equals("put", StringComparison.InvariantCultureIgnoreCase))
                    {
                        try
                        {
                            model.PostData = await context.Request.GetRawBodyStringAsync();
                        }
                        catch
                        {
                        }

                        if (model.PostData is { Length: > 1000 })
                        {
                            model.PostData = model.PostData.GetMaxCharacters(1000);
                        }
                    }
                }

                var msg = $@"
<style>
body {{ font-family: sans-serif }}
</style>
# {header}

**({(int) model.StatusCode}) {model.HttpVerb} {exceptionHandlerPath?.Path}**

```
{mainException.StackTrace}*
```
{model.PostData}
---
";

                var textMsg = $@"{header}
({(int) model.StatusCode}) {model.HttpVerb} {exceptionHandlerPath?.Path}

{mainException.StackTrace}

{model.PostData}
";
                _logger.LogCritical(pathException,textMsg);

                if (wsApp.Configuration.Email.SendAdminEmails)
                {
                    AppUtils.SendEmail(wsApp.Configuration.Email.AdminSenderEmail,
                        $"Web Store Error: {model.StatusCode} " + exceptionHandlerPath?.Path,
                        Markdown.Parse(msg),
                         out string error, noHtml: false);
                }
            }


            InitializeViewModel(model);

            return View(model);
        }

        [Route("/mvpperks")]
        [Route("/discountpricing")]
        public IActionResult MvpPerks(MvpPerksViewModel model)
        {
            var viewName = "MvpPerks";
            if (Request.Path.Value.Contains("/discountpricing", StringComparison.OrdinalIgnoreCase))
            {
                model.IsDiscountRequest = true;
                viewName = "DiscountPricing";
            }

            bool isPostback = false;
            if (!Request.Method.Equals("POST", StringComparison.InvariantCultureIgnoreCase))
            {
                model = CreateViewModel<MvpPerksViewModel>();
            }
            else
            {
                isPostback = true;
                InitializeViewModel(model);
            }

            if (!isPostback)
                return View(viewName,model);

            var validationErrors = new ValidationErrorCollection();

            var customerBus = BusinessFactory.GetCustomerBusiness();
            if (!customerBus.IsValidEmailAddress(model.Email))
            {
                validationErrors.Add("Invalid or missing email address.", "Email");
            }

            if (string.IsNullOrEmpty(model.Lastname) || string.IsNullOrEmpty(model.Firstname))
            {
                validationErrors.Add("First and lastname cannot be empty.", "Firstname");
            }

            if (string.IsNullOrEmpty(model.City))
            {
                validationErrors.Add("Place provide a city");
            }

            if (string.IsNullOrEmpty(model.CountryCode) || model.CountryCode.StartsWith("***"))
            {
                validationErrors.Add("Place provide a country...");
            }

            if (validationErrors.Count > 0)
            {
                model.ErrorDisplay.AddMessages(validationErrors, "");
                model.ErrorDisplay.ShowError("Please fix the following errors");
                return View(viewName,model);
            }

            StringBuilder sb = new StringBuilder();

            if (model.IsDiscountRequest)
                sb.AppendLine("*** West Wind Product Discount Request\r\n\r\n");
            else
                sb.AppendLine("*** West Wind MVP Product Request\r\n\r\n");

            sb.AppendLine("Name: " + model.Firstname + " " + model.Lastname);
            sb.AppendLine("Email: " + model.Email);
            sb.AppendLine("City: " + model.City);
            sb.AppendLine("Country: " + model.CountryCode);

            if (!model.IsDiscountRequest)
                sb.AppendLine("MVP Link: " + model.MvpLink);

            if (!string.IsNullOrEmpty(model.CustomerNotes))
            {
                sb.AppendLine("Notes:");
                sb.AppendLine(model.CustomerNotes);
                sb.AppendLine("---");
            }

            if (model.ReceiveMarkdownMonster)
                sb.AppendLine("Requesting: Markdown Monster");
            if (model.ReceiveWebSurge)
                sb.AppendLine("Requesting: West Wind Web Surge");

            if (model.IsDiscountRequest)
            {
                AppUtils.SendEmail(model.Email, "Westwind Technologies Product Discount Request", sb.ToString(),
                    out string error, noHtml: true);

                model.ErrorDisplay.MessageAsRawHtml = true;
                model.ErrorDisplay.Message = @"<p>Thank you for your request.</p>
<p>We'll review your request and will send you a confirmation email with a discount code if the request is approved.</p>

<p>The West Wind Team</p>";
                model.ErrorDisplay.Header = "License Discount Request Submitted";
            }
            else
            {
                AppUtils.SendEmail(model.Email, "Westwind Technologies MVP License Request", sb.ToString(),
                    out string error, noHtml: true);
                model.ErrorDisplay.MessageAsRawHtml = true;
                model.ErrorDisplay.Header = "License Request Submitted";
                model.ErrorDisplay.Message = @"<p>Thank you for your community support.</p>
<p>We've sent your license request, and we'll get back to you after verifying your contact and award info.</p>

<p>The West Wind Team</p>";

                // create a new order
                var customer = customerBus.GetCustomerByEmailAddress(model.Email);
                if (customer == null)
                    customer = customerBus.Create();

                customer.Email = model.Email;
                customer.Firstname = model.Firstname;
                customer.Lastname = model.Lastname;

                var billing = CustomerBusiness.GetBillingAddress(customer);
                billing.City = model.City;
                billing.CountryCode = model.CountryCode;

                var invoiceBus = BusinessFactory.GetInvoiceBusiness(customerBus.Context);
                var invoice = invoiceBus.Create();
                invoice.IsTemporary = false;
                invoiceBus.UpdateCustomerReferences(customer);

                invoice.Notes = model.MvpLink + "\n\n" + model.CustomerNotes;

                if (model.ReceiveMarkdownMonster)
                    invoiceBus.AddLineItem("markdown_monster_2_mvp");
                if (model.ReceiveWebSurge)
                    invoiceBus.AddLineItem("websurge_2_mvp");
                invoice.PromoCode = "MVP_REQUEST";

                invoiceBus.Save();
            }

            Response.Headers.Add("Refresh","3,url=/");
            return View(model);
        }
    }
}
