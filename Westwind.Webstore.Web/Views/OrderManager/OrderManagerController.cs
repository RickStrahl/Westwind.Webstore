using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Westwind.AspNetCore.Errors;
using Westwind.AspNetCore.Extensions;
using Westwind.AspNetCore.Views;
using Westwind.Globalization.AspnetCore.Utilities;
using Westwind.Utilities;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;
using Westwind.Webstore.Business.Utilities;
using Westwind.Webstore.Web.Controllers;
using Westwind.Webstore.Web.Models;
using Westwind.Webstore.Web.Views;

namespace Westwind.Webstore.Web.Controllers;

public class OrderManagerController : WebStoreBaseController
{
    public BusinessFactory BusinessFactory { get; }


    public OrderManagerController(BusinessFactory businessFactory)
    {
        BusinessFactory = businessFactory;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        base.OnActionExecuting(context);

        if (!UserState.IsAdmin)
        {
            context.HttpContext.Response.Redirect("/account/signin?returnurl=" + Request.Path);
            await context.HttpContext.Response.CompleteAsync();
            return;
        }

        await next();
    }

    [HttpGet]
    [Route("/admin/ordermanager/invoices")]
    [Route("/admin/ordermanager")]
    public IActionResult Index([FromQuery] string s=null)
    {
        var model = CreateViewModel<InvoiceListViewModel>();
        if (s == null)
            s = "recent";
        model.SearchTerm = s;

        if (Request.Query.Keys.Contains("message"))
        {
            model.ErrorDisplay.Message = Request.Query["message"];
            model.ErrorDisplay.Icon = Request.Query["messageicon"].FirstOrDefault() ?? "success";
        }

        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        model.InvoiceList = invoiceBus.GetInvoices(s, 15000);

        model.InvoiceTotal = model.InvoiceList.Sum(inv => inv.InvoiceTotal);
        model.InvoiceCount = model.InvoiceList.Count();

        return View("InvoiceList", model);
    }

    #region Customer Editor
    [HttpGet]
    [Route("/admin/ordermanager/customers")]
    public IActionResult CustomerList([FromQuery] string s=null, [FromQuery] string action = null, [FromQuery] string customerid = null)
    {
        var model = CreateViewModel<CustomerListViewModel>();
        model.SearchTerm = s;
        model.Action = action ?? string.Empty;
        model.CustomerId = customerid;


        var customerBus = BusinessFactory.GetCustomerBusiness();
        if (string.IsNullOrEmpty(model.SearchTerm))
            model.CustomerList = new List<CustomerListItem>();
        else
        {
            model.CustomerList = customerBus.GetCustomerList(model.SearchTerm, 5000);
            model.CustomerCount = model.CustomerList.Count();
        }

        if (model.Action.Equals("addinvoice", StringComparison.OrdinalIgnoreCase))
        {
            return NewInvoiceFromCustomer(model);
        }

        if (model.Action.Equals("deletecustomer"))
        {
            return DeleteCustomer(model);
        }

        return View("CustomerList", model);
    }

    private IActionResult DeleteCustomer(CustomerListViewModel model)
    {
        var customerBus = BusinessFactory.GetCustomerBusiness();
        if (!customerBus.Delete(model.CustomerId, true))
        {
            model.ErrorDisplay.ShowError(customerBus.ErrorMessage ,"Couldn't delete customer.");
        }
        else
        {
            model.ErrorDisplay.ShowSuccess("Customer has been deleted.");
        }

        Response.Headers["Refresh"] = $"1;/admin/OrderManager/Customers?s={model.SearchTerm}";
        return View("CustomerList", model);
    }

    [HttpGet]
    [Route("/admin/ordermanager/customers/{id}")]
    public IActionResult CustomerEditor(string id,[FromQuery] string s = null)
    {
        var model = CreateViewModel<CustomerEditorViewModel>();

        var customerBus = BusinessFactory.GetCustomerBusiness();
        model.Customer = customerBus.Load(id);
        if (model.Customer == null)
            model.Customer = customerBus.Create();

        model.BillingAddress = CustomerBusiness.GetBillingAddress(model.Customer);
        model.SearchTerm = s;

        return View("CustomerEditor",model);
    }

    [HttpPost]
    [Route("/admin/ordermanager/customers/{id}")]
    public IActionResult CustomerEditor(CustomerEditorViewModel model, string id, [FromQuery] string s = null)
    {
        InitializeViewModel(model);
        model.SearchTerm = s;

        var customerBus = BusinessFactory.GetCustomerBusiness();
        model.Customer = customerBus.Load(id);
        if (model.Customer == null)
        {
            model.Customer = customerBus.CreateEmpty();
        }

        var errors = HttpContext.Request.UnbindFormVarsToObject(model.Customer, "Id", "Customer.");
        if (errors.Count > 0)
        {
            foreach(var error in errors)
                customerBus.ValidationErrors.Add(error);
        }

        model.BillingAddress = CustomerBusiness.GetBillingAddress(model.Customer);

        errors = HttpContext.Request.UnbindFormVarsToObject(model.BillingAddress, "Id", "BillingAddress.");
        if (errors.Count > 0)
        {
            foreach(var error in errors)
                customerBus.ValidationErrors.Add(error);
        }

        if (!customerBus.Validate( model.Customer, false))
        {
            model.ErrorDisplay.AddMessages(customerBus.ValidationErrors);
            model.ErrorDisplay.ShowError("Please fix the following errors:");
            return View("CustomerEditor", model);
        }

        ModelState.Clear();
        if (!customerBus.Save())
        {
            model.ErrorDisplay.ShowError(customerBus.ErrorMessage, "Customer could not be saved.");
        }
        else
        {
            model.ErrorDisplay.ShowSuccess($"Customer {model.Customer.Fullname} has been saved.");
        }

        return View("CustomerEditor",model);
    }


    #endregion

    private IActionResult NewInvoiceFromCustomer(CustomerListViewModel model)
    {
        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        var customerBus = BusinessFactory.GetCustomerBusiness(invoiceBus.Context);

        var invoice = invoiceBus.Create();
        if (!string.IsNullOrEmpty(model.CustomerId))
        {
            var customer = customerBus.Load(model.CustomerId);
            if (customer == null)
            {
                ErrorDisplay.ShowError("Invalid customer selected.");
                return View("CustomerList", model);
            }

            invoice.CustomerId = model.CustomerId;
            invoice.BillingAddress = CustomerBusiness.GetBillingAddress(customer);
        }
        else
        {
            var customer  = customerBus.CreateEmpty(); // non-validating
            invoice.Customer = customer;
            invoice.CustomerId = customer.Id;
        }

        if (!invoiceBus.Save())
        {
            ErrorDisplay.ShowError("Invoice not created: " + invoiceBus.ErrorMessage);
            return View("CustomerList", model);
        }

        return Redirect($"/admin/ordermanager/{invoice.InvoiceNumber}");
    }

    [HttpGet]
    [Route("/admin/ordermanager/{invoiceNumber}")]
    public IActionResult InvoiceEditor(string invoiceNumber, [FromQuery] string listSearchTerm)
    {
        var model = CreateViewModel<InvoiceEditorViewModel>();
        model.ListSearchTerm = listSearchTerm;

        var message = Request.Query["message"];
        var messageIcon = Request.Query["messageicon"];
        if (!string.IsNullOrEmpty(message))
        {
            model.ErrorDisplay.Message = message;
            if (!string.IsNullOrEmpty(messageIcon))
            {
                model.ErrorDisplay.Icon = messageIcon;
                model.ErrorDisplay.AlertClass = messageIcon;
            }

            model.ErrorDisplay.Timeout = 10000;
        }

        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        model.Invoice = invoiceBus.LoadByInvNo(invoiceNumber) ?? invoiceBus.Load(invoiceNumber);
        if (model.Invoice == null)
        {
            model.Invoice = invoiceBus.Create();
            ErrorDisplay.ShowInfo("Creating a new invoice.");
            return View(model);
        }

        return View(model);
    }

    [HttpPost]
    [Route("/admin/ordermanager/{invoiceNumber}")]
    public async Task<IActionResult> InvoiceEditor(InvoiceEditorViewModel model)
    {
        InitializeViewModel(model);

        if (Request.Query.Keys.Contains("message"))
        {
            model.ErrorDisplay.Message = Request.Query["message"];
            model.ErrorDisplay.Icon = Request.Query["messageicon"];
        }

        if (Request.IsFormVar("btnProcessPayment"))
            return await ProcessPayment(model);
        if (Request.IsFormVar("btnEmailOrderConfirmation"))
            return await OrderEmailConfirmation(model);
        if (Request.IsFormVar("btnEmailProductConfirmations"))
            return EmailProductOrderConfirmations(model);
        if (Request.IsFormVar("btnDeleteInvoice"))
            return DeleteInvoice(model);
        if (Request.IsFormVar("btnAddLineItem"))
            return AddLineItem(model);


        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        var invoice = invoiceBus.Load(model.Invoice.Id);
        if (invoice == null)
        {
            invoice = invoiceBus.Create();
            var customerBus = BusinessFactory.GetCustomerBusiness();
            invoice.Customer = customerBus.Create();
        }

        if (!Request.IsFormVar("btnRecalculate"))
            invoiceBus.DontApplyPromoCodes = true;


        var minvoice = model.Invoice;
        invoice.InvoiceDate = minvoice.InvoiceDate;
        invoice.Completed = minvoice.Completed;
        invoice.PoNumber = minvoice.PoNumber;
        invoice.CreditCardResult.ProcessingResult = minvoice.CreditCardResult.ProcessingResult;
        invoice.PromoCode = minvoice.PromoCode;

        invoice.Customer.Firstname = minvoice.Customer.Firstname;
        invoice.Customer.Lastname = minvoice.Customer.Lastname;
        invoice.Customer.Company = minvoice.Customer.Company;
        invoice.Customer.Email = minvoice.Customer.Email;

        invoice.BillingAddress.StreetAddress = minvoice.BillingAddress.StreetAddress;
        invoice.BillingAddress.City = minvoice.BillingAddress.City;
        invoice.BillingAddress.PostalCode = minvoice.BillingAddress.PostalCode;
        invoice.BillingAddress.CountryCode = minvoice.BillingAddress.CountryCode;
        invoice.BillingAddress.Telephone = minvoice.BillingAddress.Telephone;

        if (!invoiceBus.Save())
        {
            ErrorDisplay.ShowError($"Invoice was not saved: {invoiceBus.ErrorMessage}");
        }
        else
        {
            ErrorDisplay.ShowSuccess("Invoice Saved");
        }

        model.Invoice = invoice;

        return View(model);
    }

    private IActionResult AddLineItem(InvoiceEditorViewModel model)
    {
        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        var invoice = invoiceBus.Load(model.Invoice.Id);
        if (invoice == null)
        {
            model.ErrorDisplay.ShowError($"Couldn't load invoice: {invoiceBus.ErrorMessage}");
            return View("InvoiceEditor", model);
        }

        var lineItem = invoiceBus.AddLineItem(string.Empty,createEmptyLineItem: true);
        invoiceBus.Save();

        return Redirect("/admin/ordermanager/" + invoice.Id + "/" + lineItem.Id);
    }

    private IActionResult DeleteInvoice(InvoiceEditorViewModel model)
    {
        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        var invoice = invoiceBus.Load(model.Invoice.Id);

        if (!invoiceBus.Delete(model.Invoice.Id, true))
        {
            model.ErrorDisplay.ShowError("Couldn't delete invoice: " + invoiceBus.ErrorMessage);
            return View("InvoiceEditor",model);
        }

        var message = $"Invoice {invoice.InvoiceNumber} was deleted.";
        return Redirect($"/admin/OrderManager?message={WebUtility.UrlEncode(message)}");
    }

    private async Task<IActionResult> ProcessPayment(InvoiceEditorViewModel model)
    {
        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        var invoice = invoiceBus.Load(model.Invoice.Id);
        if (invoice == null)
            return Redirect("/admin/ordermanager");

        var result = invoiceBus.ProcessCreditCard();

        var message = $"Credit Card Processed: {invoice.CreditCardResult.ToString()}.";
        var icon = "success";
        if (!result)
        {
            message = $"Credit Card Processing failed: {invoiceBus.ErrorMessage}.";
            icon = "warning";
        }
        else
        {
            result = invoiceBus.UpdateLineItemsLicenses();
            if (!result)
                message = $"Failed to create licenses: {invoiceBus.ErrorMessage}";
        }

        if (!result)
        {
            // do nothing
        }
        else if (!invoiceBus.Save())
        {
                message = $"Couldn't save invoice: {invoiceBus.ErrorMessage}.";
                icon = "warning";
        }
        else
        {
            result = invoiceBus.SendEmailItemConfirmations();

            if (!result)
            {
                message = $"Failed to send email confirmations: {invoiceBus.ErrorMessage}.";
                icon = "warning";
            }
            else
            {
                if (invoice.Completed == null || invoice.Completed <= wsApp.Constants.EmptyDate)
                    invoice.Completed = DateTime.Now;

                message += " and product confirmation sent.";

                result = await invoiceBus.SaveAsync();
            }
        }

        await Task.Yield(); // force async

        return Redirect($"/admin/ordermanager/{invoice.Id}?message={message}&messageicon={icon}");
    }

    private async Task<IActionResult> OrderEmailConfirmation(InvoiceEditorViewModel model)
    {
        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        var invoice = invoiceBus.Load(model.Invoice.Id);
        if (invoice == null)
            return Redirect("/admin/ordermanager");

        var orderFormModel = new OrderFormViewModel();
        orderFormModel.InvoiceModel = new InvoiceViewModel(invoice);

        var confirmation = await ViewRenderer.RenderViewToStringAsync(
            "../ShoppingCart/EmailConfirmation",
            orderFormModel, this.ControllerContext);

        var emailer = new Emailer();
        var result = emailer.SendEmail(invoice.Customer.Email,
            wsApp.Configuration.ApplicationName + " Order Confirmation #" + invoice.InvoiceNumber,
            confirmation,
            "text/html");

        var message = "Email confirmation sent.";
        var icon = "success";
        if (!result)
        {
            message = "Email sending failed: " + emailer.ErrorMessage;
            icon = "error";
        }

        return Redirect($"/admin/ordermanager/{invoice.Id}?message={WebUtility.UrlEncode(message)}&messageicon={icon}");
    }

    private IActionResult EmailProductOrderConfirmations(InvoiceEditorViewModel model)
    {
        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        var invoice = invoiceBus.Load(model.Invoice.Id);
        if (invoice == null)
            return Redirect("/admin/ordermanager");

       var result =  invoiceBus.SendEmailItemConfirmations();

       var message = "Product Email confirmation sent.";
       var icon = "success";
       if (!result)
       {
           message = $"Email sending failed: {invoiceBus.ErrorMessage}";
           icon = "error";
       }
       else
       {
           if (invoice.Completed == null || invoice.Completed <= wsApp.Constants.EmptyDate)
               invoice.Completed = DateTime.Now;
           invoiceBus.Save();
       }


       var url = $"/admin/ordermanager/{invoice.Id}?message={WebUtility.UrlEncode(message.Trim())}&messageicon={icon}";
       return Redirect(url);
    }


    [HttpGet]
    [Route("/admin/ordermanager/{invoiceNumber}/{lineItemNumber}")]
    public IActionResult LineItemEditor(string invoiceNumber, string lineItemNumber)
    {
        var model = this.CreateViewModel<LineItemEditorViewModel>();

        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        model.Invoice = invoiceBus.LoadByInvNo(invoiceNumber);
        if (model.Invoice == null)
        {
            model.Invoice = invoiceBus.Load(invoiceNumber);  // try by invoice Id
        }
        model.LineItem = model.Invoice.LineItems.FirstOrDefault(li => li.Id == lineItemNumber);
        return View(model);
    }


}

public class InvoiceListViewModel : WebStoreBaseViewModel
{
    public IQueryable<InvoiceListItem> InvoiceList { get; set; }

    public string SearchTerm { get; set; } = "recent";

    public object SearchCustomer { get; set; }

    public int InvoiceCount { get; set; }

    public decimal InvoiceTotal { get; set; }
}

public class InvoiceEditorViewModel : WebStoreBaseViewModel
{
    public Invoice Invoice { get; set; } = new Invoice();
    public string ListSearchTerm { get; set; }
}

public class LineItemEditorViewModel : WebStoreBaseViewModel
{
    public LineItem LineItem { get; set; } = new LineItem();
    public Invoice Invoice { get; set; } = new Invoice();

    public string SearchTerm { get; set; }
}

public class CustomerListViewModel : WebStoreBaseViewModel
{
    public List<CustomerListItem> CustomerList { get; set; }

    public string Action { get; set; }

    public string CustomerId { get; set; }
    public string SearchTerm { get; set; }
    public int CustomerCount { get; set; }
}


