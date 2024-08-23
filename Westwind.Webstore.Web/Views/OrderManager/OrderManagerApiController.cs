using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Westwind.AspNetCore.Errors;
using Westwind.AspNetCore.Messages;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;

namespace Westwind.Webstore.Web.Controllers;

[ApiExceptionFilter(false)]
public class OrderManagerApiController : WebStoreBaseController
{
    public BusinessFactory BusinessFactory { get; }


    public OrderManagerApiController(BusinessFactory businessFactory)
    {
        BusinessFactory = businessFactory;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        base.OnActionExecuting(context);

        if (!UserState.IsAdmin)
        {
            throw new ApiException("Unauthorized", 401);
        }

        await next();
    }


    [HttpGet]
    [Route("/admin/ordermanager/api/invoice/{invoiceNumber}")]
    public Invoice GetInvoice(string invoiceNumber)
    {
        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        var invoice = invoiceBus.LoadByInvNo(invoiceNumber);
        if (invoice == null)
            throw new ApiException("Invalid Invoice Number");

        return invoice;
    }

    [HttpGet]
    [Route("admin/ordermanager/api/lineitem/{lineItemNumber}")]
    public LineItem GetLineItem(string lineItemNumber)
    {
        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        var item = invoiceBus.GetLineItem(lineItemNumber);
        if (item == null)
        {
            throw new ApiException("Invalid line item: " + invoiceBus.ErrorMessage);
        }

        return item;
    }

    [HttpDelete]
    [Route("/admin/ordermanager/api/lineitem/{lineItemNumber}")]
    public ApiResponse DeleteLineItem(string lineItemNumber)
    {
        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        var result  = invoiceBus.Db.ExecuteNonQuery("delete from LineItems where Id = @0", lineItemNumber);
        if (result < 0)
        {
            throw new ApiException("Failed to delete line item.", 404);
        }

        return new ApiResponse() { Message = "Lineitem deleted." };
    }

    [HttpPost]
    [Route("admin/ordermanager/api/lineitem/recalculate")]
    public LineItem RecalculateLineItem([FromBody] LineItem model)
    {

        if (model == null || string.IsNullOrEmpty(model.Id) )
        {
            throw new ApiException("Invalid line item.");
        }
        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        var item = invoiceBus.GetLineItem(model.Id);
        if (item == null)
        {
            throw new ApiException("Invalid line item: " + invoiceBus.ErrorMessage);
        }

        item.Sku = model.Sku;
        item.Description = model.Description;
        item.Price = model.Price;
        item.Quantity = model.Quantity;
        item.DiscountPercent = model.DiscountPercent;
        return item;
    }

    [HttpPost]
    [Route("admin/ordermanager/api/lineitem")]
    public LineItem UpdateLineItem([FromBody] LineItem model)
    {
        if (model == null || string.IsNullOrEmpty(model.Id) )
        {
            throw new ApiException("Invalid line item.");
        }

        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        invoiceBus.DontApplyPromoCodes = true;  // let us override the PromoCode here

        var invoice = invoiceBus.Load(model.InvoiceId);
        if (invoice == null)
        {
            throw new ApiException("Invalid invoice number.");
        }

        var lineItem = invoice.LineItems.FirstOrDefault(li => li.Id == model.Id);
        if (lineItem == null)
        {
            lineItem = new LineItem();
            invoiceBus.Context.Add<LineItem>(lineItem);
            invoice.LineItems.Add(lineItem);
            lineItem.InvoiceId = invoice.Id;
        }

        lineItem.Sku = model.Sku;
        lineItem.Description = model.Description;
        lineItem.Price = model.Price;
        lineItem.Quantity = model.Quantity;
        lineItem.DiscountPercent = model.DiscountPercent;
        lineItem.LicenseSerial = model.LicenseSerial;

        lineItem.AutoRegister = model.AutoRegister;
        lineItem.UseLicensing = model.UseLicensing;
        lineItem.ItemImage = model.ItemImage;

        lineItem.CalculateItemTotal();

        if (!invoiceBus.Save())
        {
            throw new ApiException("Lineitem save failed: " + invoiceBus.ErrorMessage);
        }

        return lineItem;
    }

    [HttpGet]
    [Route("/admin/ordermanager/api/product/{sku}")]
    public Product GetProduct(string sku)
    {
        var productBus = BusinessFactory.GetProductBusiness();
        var product  = productBus.LoadBySku(sku);
        if (product == null)
        {
            throw new ApiException("Product not found: " + productBus.ErrorMessage, 404);
        }
        return product;
    }

    [HttpDelete]
    [Route("/admin/ordermanager/api/invoice/{invoiceNumber}")]
    public object DeleteInvoice(string invoiceNumber)
    {
        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        var invoice = invoiceBus.LoadByInvNo(invoiceNumber);
        if (invoice == null)
            throw new ApiException("Invalid Invoice Number");

        var id = invoice.Id;

        return new { isSuccess = invoiceBus.Delete(id, true), message = invoiceBus.ErrorMessage };
    }

    #region Licensing

    [HttpGet]
    [Route("/admin/ordermanager/api/licensing/create/{invoiceId}/{lineItemId}")]
    public object CreateLineItemLicense( string invoiceId, string lineItemId)
     {
        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        var invoice = invoiceBus.Load(invoiceId);
        if (invoice == null)
        {
            invoice = invoiceBus.LoadByInvNo(invoiceId);
            if (invoice == null)
                throw new ApiException("Invalid invoice.",404);
        }

        var lineItem = invoice.LineItems.FirstOrDefault(li => li.Id == lineItemId);
        if (lineItem == null)
        {
            throw new ApiException("Invalid line item - make sure the line item has been saved.",404);
        }

        if (!invoiceBus.CreateLineItemLicense(lineItem, force: true))
        {
            throw new ApiException($"Couldn't create license: {invoiceBus.ErrorMessage}");
        }

        if (!invoiceBus.Save())
        {
            throw new ApiException($"Couldn't save line item with updated license: {invoiceBus.ErrorMessage}");
        }

        return new  ApiResponse
        {
            Message = "License has been created.",
            Data = lineItem
        };
    }

    [HttpGet]
    [Route("/admin/ordermanager/api/licensing/revoke/{invoiceId}/{lineItemId}")]
    public object RevokeLicense(string invoiceId,string  lineItemId)
    {
        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        var invoice = invoiceBus.Load(invoiceId);
        if (invoice == null)
        {
            invoice = invoiceBus.LoadByInvNo(invoiceId);
            if (invoice == null)
                throw new ApiException("Invalid invoice.",404);
        }

        var lineItem = invoice.LineItems.FirstOrDefault(li => li.Id == lineItemId);
        if (lineItem == null)
        {
            throw new ApiException("Invalid line item - make sure the line item has been saved.",404);
        }

        // Toggle license revocation status
        var licensing = new InvoiceLicensing(invoiceBus);
        if (!licensing.RevokeLicense(lineItem))
        {
            throw new ApiException($"Revoking of the license failed: {licensing.ErrorMessage}");
        }

        return new ApiResponse
        {
            Message = "License has been revoked.",
            Data = lineItem
        };
    }

    [HttpGet]
    [Route("/admin/ordermanager/api/licensing/reissue/{invoiceId}/{lineItemId}")]
    public object ReissueLicense(string invoiceId,string  lineItemId)
    {
        var invoiceBus = BusinessFactory.GetInvoiceBusiness();
        var invoice = invoiceBus.Load(invoiceId);
        if (invoice == null)
        {
            invoice = invoiceBus.LoadByInvNo(invoiceId);
            if (invoice == null)
                throw new ApiException("Invalid invoice.",404);
        }

        var lineItem = invoice.LineItems.FirstOrDefault(li => li.Id == lineItemId);
        if (lineItem == null)
        {
            throw new ApiException("Invalid line item - make sure the line item has been saved.",404);
        }

        // Toggle license revocation status
        var licensing = new InvoiceLicensing(invoiceBus);
        if (!licensing.ReissueLicense(lineItem))
        {
            throw new ApiException($"Reissuing of the license failed: {licensing.ErrorMessage}");
        }

        return new ApiResponse
        {
            Message = "License has been revoked.",
            Data = lineItem
        };
    }
    #endregion

}

