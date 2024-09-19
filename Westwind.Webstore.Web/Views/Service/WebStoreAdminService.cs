using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Westwind.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Westwind.AspNetCore.Errors;
using Westwind.Utilities.Data.Security;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;

namespace Westwind.Webstore.Web.Service
{

    public class WebStoreAdminService : BaseApiController
    {

        public UserTokenManager TokenManager { get; set; }


        public WebStoreAdminService()
        {
            TokenManager = new UserTokenManager(wsApp.Configuration.ConnectionString);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var script = System.IO.Path.GetFileName(Request.Path.Value)?.ToLower();
            if (script == null)
                throw new UnauthorizedAccessException();

            if (script != "authenticate") // exclusion list
            {
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
                if (!string.IsNullOrEmpty(token))
                {
                    if (!TokenManager.IsTokenValid(token))
                        token = null;
                }

                if (string.IsNullOrEmpty(token))
                {
                    throw new ApiException("Not Authorized", 401);
                }
            }
        }

        [HttpPost]
        [Route("api/account/authenticate")]
        public object AuthenticateWithToken([FromBody] AuthenticateRequest authenticateRequest)
        {
            if (authenticateRequest == null)
                throw new ApiException("Invalid Sign in data.",401);

            var customerBus = BusinessFactory.Current.GetCustomerBusiness();
            var customer = customerBus.AuthenticateAndRetrieveUser(authenticateRequest.Username, authenticateRequest.Password);

            if (customer == null)
                throw new ApiException("Invalid credentials passed.", 401);
            if (!customer.IsAdminUser)
                throw new ApiException("Access denied.", 401);

            var tokenManager = new UserTokenManager(wsApp.Configuration.ConnectionString);
            var token = tokenManager.CreateNewToken(customer.Id);

            return new
            {
                Token = token
            };
        }

        [HttpGet]
        [Route("adminservice/products")]
        public IEnumerable<Product> GetProducts(string searchTerm)
        {
            var productBus = BusinessFactory.Current.GetProductBusiness();
            var items = productBus.GetItems(new InventoryItemsFilter { SearchTerm = searchTerm });
            return items;
        }

        [HttpGet]
        [Route("adminservice/customers")]
        public IEnumerable<CustomerListResult> GetCustomers()
        {
            var customerBus = BusinessFactory.Current.GetCustomerBusiness();
            var custList = customerBus.GetCustomerList(new Business.CustomerListFilter
            {
                ActiveOnly = true,
                SortOrder = CustomerListSortOrder.Entered,
                MaxItems = 5000
            });


            return custList;
        }



#region Invoices
        [HttpGet]
        [Route("adminservice/invoices")]
        public IQueryable<object> GetInvoiceList( [FromQuery] string q = null) // searchTerm
        {
            var invoiceBus = BusinessFactory.Current.GetInvoiceBusiness();
            var invoice = invoiceBus.GetInvoices(q);
            if (invoice == null)
                throw new ApiException(invoiceBus.ErrorMessage,404);

            return invoice;
        }

        [HttpGet]
        [Route("adminservice/invoice/{invoiceNumber}")]
        public Invoice GetInvoice(string invoiceNumber)
        {
            Invoice invoice;
            using (var invoiceBus = BusinessFactory.Current.GetInvoiceBusiness())
            {
                invoice = invoiceBus.Context.Invoices
                    .Include("LineItems")
                    .Include("Customer")
                    .Include(i=> i.Customer.Addresses)
                    .FirstOrDefault(i => i.InvoiceNumber == invoiceNumber);

                if (invoice == null)
                    throw new ApiException( "Invalid Invoice", 404);

                return invoice;
            }
        }




        [HttpGet]
        [Route("adminservice/customers/{customerId}/invoices")]
        public IEnumerable<Invoice> GetCustomerInvoices(string customerId)
        {
            using (var invoiceBus = BusinessFactory.Current.GetInvoiceBusiness())
            {
                return invoiceBus.Context.Invoices
                    .Include("LineItems")
                    .Include("Customer")
                    .Include(i=> i.Customer.Addresses)
                    .Where(i => i.CustomerId == customerId && !i.IsTemporary)
                    .OrderByDescending(i => i.InvoiceDate).ToList();
            }
        }

        #endregion


        [Route("adminservice/systeminfo")]
        public object SystemInfo([FromServices] IServer server)
        {            
            var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses;

            return new
            {
                Version = wsApp.Version,
                HostAddresses = addresses,
                Platform = $"{RuntimeInformation.FrameworkDescription}- {wsApp.EnvironmentName}",
                Os = RuntimeInformation.OSDescription + " (" + RuntimeInformation.OSArchitecture + ")"
            };
        }
    }

    public class AuthenticateRequest
    {
        public string Username { get; set; }
        public string Password { get; set;  }
    }
}
