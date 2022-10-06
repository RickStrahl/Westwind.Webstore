using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Westwind.AspNetCore.Extensions;
using Westwind.Utilities;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;
using Westwind.Webstore.Web.Models;
using Westwind.Webstore.Web.App;
using Westwind.Webstore.Web.Utilities;
using Westwind.Webstore.Web.Views;

namespace Westwind.Webstore.Web.Controllers
{
    public class ShoppingCartController : WebStoreBaseController
    {
        public BusinessFactory BusinessFactory { get; }


        public ShoppingCartController(BusinessFactory businessFactory)
        {
            BusinessFactory = businessFactory;
        }

        [Route("shoppingcart")]
        public IActionResult Index()
        {
            var model = CreateViewModel<ShoppingCartViewModel>();

            var busInvoice = BusinessFactory.GetInvoiceBusiness();
            Invoice invoice;

            if (string.IsNullOrEmpty(AppUserState.InvoiceId))
            {
                invoice = busInvoice.Create();
            }
            else
            {
                invoice = busInvoice.Load(AppUserState.InvoiceId);
                if (invoice == null)
                {
                    invoice = busInvoice.Create();
                    invoice.IsTemporary = true;
                    if (string.IsNullOrEmpty(AppUserState.InvoiceId))
                        invoice.Id = null;
                    invoice.Id = AppUserState.InvoiceId;
                }

                if (!invoice.IsTemporary)
                {
                    ClearInvoiceAppUserState();
                    return RedirectToAction("Index");
                }
            }


            // always update the customer references from actual profile record
            // in case it was changed so we display tax/shipping correctly
            var busCustomer = BusinessFactory.GetCustomerBusiness(busInvoice.Context);
            var customer = busCustomer.Load(AppUserState.UserId);
            if (customer != null)
            {
                // update invoice's customer and billing address refs
                busInvoice.UpdateCustomerReferences(customer);
                invoice.IsTemporary = true;
                busInvoice.Save();
            }

            model.Invoice = new InvoiceViewModel(invoice)
            {
                CanDeleteLineItems = true,
                CanEditLineItemQuantity = true,
                CanEditPromoCode = true
            };

            model.InvoiceId = invoice.Id;
            model.AppUserState.CartItemCount = invoice.LineItems.Count;

            // form inputs
            if (HttpContext.Request.Method == HttpMethods.Post)
            {
                model.Invoice.PromoCode = Request.Form["invoice.PromoCode"];
                invoice.PromoCode = model.Invoice.PromoCode;
                busInvoice.ApplyPromoCodes(invoice.PromoCode);
            }

            if (HttpContext.Request.IsFormVar("btnRecalculate"))
            {
                Recalculate(busInvoice);
            }

            return View("ShoppingCart",model);
        }

        /// <summary>
        /// Manually parse the line items and
        /// </summary>
        /// <param name="invoice">invoice to recalculate</param>
        /// <param name="noSave">don't save the invoice</param>
        /// <returns></returns>
        private bool Recalculate(InvoiceBusiness invoice, bool noSave = false)
        {
            var inv = invoice.Entity;

            var req = HttpContext.Request;
            inv.PromoCode = req.Form["invoice.PromoCode"].FirstOrDefault();

            foreach (var itemVar in req.Form)
            {
                if (!itemVar.Key.StartsWith("item_")) continue;

                var sku = itemVar.Key.Replace("item_", "");
                var lineItem = inv.LineItems.FirstOrDefault(li => li.Sku == sku);
                if (lineItem == null) continue;

                decimal qty = StringUtils.ParseDecimal(itemVar.Value.FirstOrDefault(), 0);

                qty = invoice.FixLineItemQuantityForFractionalItem(sku, qty);

                if (qty < 0.01M)
                {
                    inv.LineItems.Remove(lineItem);
                    continue;
                }

                lineItem.Quantity = qty;
            }

            invoice.CalculateTotals();

            if (!noSave)
                return invoice.Save();

            return true;
        }


        [HttpGet, HttpPost]
        [Route("shoppingcart/add/{sku}/{quantity}"), Route("shoppingcart/add/{sku}"), Route("shoppingcart/add")]
        public async Task<IActionResult> AddItem(string sku, decimal quantity)
        {
            var model = CreateViewModel<ShoppingCartViewModel>();

            var busInvoice = BusinessFactory.GetInvoiceBusiness();
            Invoice invoice;

            if (string.IsNullOrEmpty(AppUserState.InvoiceId))
            {
                invoice = busInvoice.Create();
            }
            else
            {
                invoice = await busInvoice.LoadAsync(AppUserState.InvoiceId);
                if (invoice == null)
                {
                    invoice = busInvoice.Create();
                    if (string.IsNullOrEmpty(AppUserState.InvoiceId))
                        invoice.Id = null;
                    else
                        invoice.Id = AppUserState.InvoiceId;
                }
            }


            invoice.IsTemporary = true;

            model.Invoice = new InvoiceViewModel(invoice);
            model.InvoiceId = invoice.Id;

            busInvoice.AddLineItem(sku, quantity);
            busInvoice.CalculateTotals();

            if (invoice.LineItems.Count > 0)
            {
                // We need to force the id and count stored in cookie to be updated with the new invoice id
                AppUserState.SetInvoiceSettings(invoice);

                if (!await busInvoice.SaveAsync())
                {
                    TempData["DisplayMessage"] = "Couldn't save invoice after adding items into shopping cart: " + busInvoice.ErrorMessage;
                    return Redirect("~/");
                }
            }

            return Redirect("~/shoppingcart");
        }

        [HttpGet, HttpPost]
        [Route("shoppingcart/remove/{lineItemId}/{quantity?}")]
        public async Task<ActionResult> RemoveItem(string lineItemId, decimal quantity = 0)
        {
            if (string.IsNullOrEmpty(AppUserState.InvoiceId))
                return Redirect("/");

            var model = CreateViewModel<ShoppingCartViewModel>();
            var busInvoice = BusinessFactory.GetInvoiceBusiness();

            Invoice invoice;
            invoice = await busInvoice.LoadAsync(AppUserState.InvoiceId);
            if (invoice == null)
            {
                return Redirect("/");
            }

            busInvoice.RemoveLineItem(lineItemId, invoice);

            AppUserState.SetInvoiceSettings(invoice);

            if (!await busInvoice.SaveAsync())
            {
                model.ErrorDisplay.ShowError("Couldn't save invoice after removing items.");
                return View("ShoppingCart");
            }

            // we want to explicitly
            return Redirect("~/shoppingcart");
        }

        #region Long Form Order Form

        [HttpGet]
        [Route("shoppingcart/orderForm")]
        public ActionResult OrderForm()
        {
            var model = CreateViewModel<OrderFormViewModel>();
            model.dtto = OrderValidation.EncodeCurrentDate();

            var busCustomer = BusinessFactory.GetCustomerBusiness();
            var customer = busCustomer.Load(AppUserState.UserId);
            if (customer == null)
                return Redirect("~/");

            var busInvoice = BusinessFactory.GetInvoiceBusiness();
            var invoice = busInvoice.Load(AppUserState.InvoiceId);
            if (invoice == null)
                return Redirect("~/");

            // Update InvoiceModel From Customer
            model.InvoiceModel = new InvoiceViewModel(invoice);
            model.InvoiceModel.CanEditPromoCode = true;

            // ensure customer and address are linked
            busInvoice.UpdateCustomerReferences(customer);
            busInvoice.Save(); // save address and id

            return View(model);
        }

        [HttpPost]
        [Route("shoppingcart/orderForm")]
        public ActionResult OrderForm(OrderFormViewModel model)
        {
            InitializeViewModel(model);

            bool isRecalculate = Request.IsFormVar("btnRecalculate");

            var custId = AppUserState.UserId;
            var invoiceId = AppUserState.InvoiceId;

            if (string.IsNullOrEmpty(custId) || string.IsNullOrEmpty(invoiceId))
            {
                return Redirect("~/");
            }

            var customerBusiness = BusinessFactory.GetCustomerBusiness();
            var customer = customerBusiness.Load(custId);
            if (customer == null)
            {
                return Redirect("~/");
            }

            var invoiceBusiness = BusinessFactory.GetInvoiceBusiness(customerBusiness.Context);
            var invoice = invoiceBusiness.Load(invoiceId);
            if (invoice == null || invoice.LineItems.Count < 1)
            {
                return Redirect("~/");
            }

            // We just re-display data from stored invoice
            invoice.Notes = model.InvoiceModel.Notes;
            invoice.PoNumber = model.InvoiceModel.PoNumber;

            // ensure customer and address are linked
            invoiceBusiness.UpdateCustomerReferences(customer);
            //var entry = invoiceBusiness.Context.Entry(customer);
            //entry.State = EntityState.Modified;

            var invoiceModel = model.InvoiceModel;
            model.InvoiceModel.CanEditPromoCode = true;

            invoiceModel.Invoice = invoice;  // assign actual invoice

            invoiceBusiness.CalculateTotals();

            if (isRecalculate)
            {
                Recalculate(invoiceBusiness);
                if (!invoiceBusiness.Save(invoice))
                {
                    model.ErrorDisplay.AddMessage(invoiceBusiness.ErrorMessage);
                }

                return View(model);
            }

            // Validate Invoice/Customer/Security and set error display. if False redisplay view
            if (!ValidateOrderForm(model, invoiceBusiness, customerBusiness))
                return View(model);

            invoiceBusiness.CalculateTotals();

            if (!ProcessCreditCard(model, invoiceBusiness))
            {
                ModelState.Clear();
                model.ErrorDisplay.AddMessage(invoiceBusiness.ErrorMessage,"dropin-container");
                model.ErrorDisplay.ShowError("Please fix the following");
                return View(model);
            }

            invoice.IsTemporary = false;
            invoice.InvoiceDate = DateTime.Now;
            invoice.BillingAddress = CustomerBusiness.GetBillingAddress(customer);

            if (!invoiceBusiness.Save())
            {
                model.ErrorDisplay.ShowError(invoiceBusiness.ErrorMessage,
                    AppResources.ShoppingCart.InvoiceCouldNotBeSaved);
                return View(model);
            }

            invoiceBusiness.UpdateLineItemsLicenses(saveInvoice: true);

            invoiceBusiness.DeleteExpiredTemporaryInvoices();

            ClearInvoiceAppUserState();

            return Redirect("/shoppingcart/orderconfirmation/" + invoice.InvoiceNumber);
        }



        [HttpGet]
        [Route("/product/order/{sku}")]
        public ActionResult OrderFormFast(string sku)
        {
            var model = CreateViewModel<OrderFormFastViewModel>();

            var busProduct = BusinessFactory.GetProductBusiness();
            model.Product = busProduct.LoadBySku(sku);
            if (model.Product == null)
                return Redirect("/");

            var busCustomer = BusinessFactory.GetCustomerBusiness(busProduct.Context);
            var customer = busCustomer.Load(AppUserState.UserId);
            if (customer == null)
                customer = busCustomer.Create();

            model.Customer = customer;

            model.Firstname = customer.Firstname;
            model.Lastname = customer.Lastname;
            model.Company = customer.Company;
            model.Email = customer.Email;
            model.Password = null; // ensure

            var address = CustomerBusiness.GetBillingAddress(customer);
            model.StreetAddress = address?.StreetAddress.GetLines().FirstOrDefault();
            model.PostalCode = address?.PostalCode;
            model.CountryCode = address?.CountryCode;

            return View("OrderFormFast", model);
        }


        [HttpPost]
        [Route("/product/order/{sku}")]
        public ActionResult OrderFormFast(string sku, OrderFormFastViewModel model)
        {
            InitializeViewModel(model);

            if (Request.Form.ContainsKey("btnSignIn"))
            {
                return Redirect("/account/signin/?returnurl=" + Request.Path.ToString());
            }

            var productBusiness = BusinessFactory.GetProductBusiness();
            model.Product = productBusiness.LoadBySku(sku);
            if (model.Product == null)
                return Redirect("/");

            var customerBusiness = BusinessFactory.GetCustomerBusiness(productBusiness.Context);
            var customer = customerBusiness.Load(AppUserState.UserId);
            if (customer == null)
                customer = customerBusiness.Create();

            model.Customer = customer;

            string oldEmail = customer.Email;

            customer.Firstname = model.Firstname;
            customer.Lastname = model.Lastname;
            customer.Company = model.Company;
            customer.Email = model.Email;

            if (customer.IsNew)
                customer.Password = model.Password;

            var address = CustomerBusiness.GetBillingAddress(customer);
            if (customer.IsNew)
                customer.Addresses.Add(address);

            address.StreetAddress = model.StreetAddress;
            address.PostalCode = model.PostalCode;
            address.CountryCode = model.CountryCode;



            if (customer.IsNew)
            {
                if (string.IsNullOrEmpty(model.Password))
                {
                    customer.Password = null;
                    customerBusiness.ValidationErrors.Add(AppResources.Account.PasswordMissingOrdontMatch, "password");
                }

                customerBusiness.ValidatePassword(model.Password);
            }

            // email has changed - validate it
            if (wsApp.Configuration.Security.ValidateEmailAddresses &&
                (string.IsNullOrEmpty(oldEmail) ||
                 (oldEmail != customer.Email)))
            {
                // check email validation code
                var validator = new EmailAddressValidator();
                if (validator.IsCodeValidated(model.Evc, customer.Email, false))
                {
                    customer.ValidationKey = null;
                    customer.IsActive = true;
                }
                else
                {
                    ErrorDisplay.AddMessage("Your email address has not been validated.", "Email");
                }
            }

            customerBusiness.Validate(customer, true);

            if (customerBusiness.HasValidationErrors)
            {
                model.ErrorDisplay.AddMessages(customerBusiness.ValidationErrors, null);
                model.ErrorDisplay.ShowError("Please fix the following errors:");
                return View("OrderFormFast",model);
            }

            var invoiceBusiness = BusinessFactory.GetInvoiceBusiness(customerBusiness.Context);

            // create a new invoice
            var invoice = invoiceBusiness.Create();
            invoice.IsTemporary = true;

            // ensure customer and address are linked
            invoiceBusiness.UpdateCustomerReferences(customer);
            invoiceBusiness.AddLineItem(sku);
            invoiceBusiness.CalculateTotals();

            if (!invoiceBusiness.Save())
            {
                model.ErrorDisplay.ShowError(invoiceBusiness.ErrorMessage,
                    AppResources.ShoppingCart.InvoiceCouldNotBeSaved);
                return View(model);
            }

            // log in the user (if new and update if changed)
            SetAppUserFromCustomer(customer);
            AppUserState.InvoiceId = invoice.Id;

            return Redirect("/shoppingcart/orderform");
        }



        /// <summary>
        /// Validates the current order to be processed:
        ///
        /// * Invoice data entered
        /// * Customer data entered
        /// * Captcha
        /// * Timeout
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <param name="invoiceBusiness"></param>
        /// <param name="customerBusiness"></param>
        /// <returns></returns>
        private bool ValidateOrderForm(OrderFormViewModel model, InvoiceBusiness invoiceBusiness, CustomerBusiness customerBusiness)
        {
            var invoice = invoiceBusiness.Entity;
            var customer = customerBusiness.Entity;

            var hasValidationErrors = !invoiceBusiness.Validate(invoice);

            if (wsApp.Configuration.Security.UseOrderFormTimeout)
            {
                var errorResult = OrderValidation.IsTimeEncodingValid(model.dtto,
                    wsApp.Configuration.Security.OrderFormMinimumSecondsTimeout,
                    wsApp.Configuration.Security.OrderFormMaximumMinutesTimeout);

                if (errorResult.IsError)
                {
                    invoiceBusiness.ValidationErrors.Add(errorResult.Message);
                }
                else
                {
                    // assign a new date
                    model.dtto = OrderValidation.EncodeCurrentDate();
                }
            }

            if (wsApp.Configuration.Security.UseOrderFormRecaptcha &&
                !OrderValidation.VerifyRecaptcha(model.ReCaptchaResult, invoice.CreditCard.IpAddress))
            {
                invoiceBusiness.ValidationErrors.Add("reCAPTCHA Failed. Please try validating again.", "grc");
            }

            if (hasValidationErrors || invoiceBusiness.ValidationErrors.Count > 0)
            {
                model.ErrorDisplay.AddMessages(invoiceBusiness.ValidationErrors);
                model.ErrorDisplay.ShowError(AppResources.Resources.PleaseFixTheFollowing);
                return false;
            }

            return true;
        }


        private bool ProcessCreditCard(OrderFormViewModel model, InvoiceBusiness invoiceBusiness)
        {
            // don't process cards but it's a success
            if (!wsApp.Configuration.Payment.ProcessCardsOnline)
                return true;

            var invoiceModel = model.InvoiceModel;
            var invoice = invoiceModel.Invoice;


            // capture Credit Card Props
            invoice.CreditCard.Nonce = invoiceModel.Nonce;
            invoice.CreditCard.Descriptor = invoiceModel.Descriptor;
            invoice.CreditCard.IpAddress = HttpContext.Connection.RemoteIpAddress.ToString();

            if (invoice.CreditCard.IpAddress == "::1")
                invoice.CreditCard.IpAddress = "127.0.0.1";

            if (string.IsNullOrEmpty(invoice.CreditCard.Nonce))
            {
                invoiceBusiness.ValidationErrors.Add("Missing payment nonce. Please fill out the payment section.");
                return false;
            }

            // sets error info on business object validation errors
            if (!invoiceBusiness.ProcessCreditCard())
            {
                var inv = invoice;
                var cust = inv.Customer;

                var result = invoice.CreditCardResult;
                var msg = $@"Credit Card Processing Failed
Result:     {result.ProcessingResult}
Resultx:    {result.RawProcessingResult}
Error:      {result.ProcessingError}

CustId:     {inv.CustomerId}
Name:       {cust.Firstname + " " + cust.Lastname}
Zip:        {inv.BillingAddress.PostalCode}
Avs:        {result.AvsCode}
Card:       {inv.CreditCard.LastFour}
Nonce:      {inv.CreditCard.Nonce}";

                model.InvoiceModel.Nonce = null;
                model.InvoiceModel.Descriptor = null;
                model.Descriptor = null;
                model.Nonce = null;

                //LogManager.Current.LogError(new InvalidOperationException(msg), true);
                return false;
            }

            return true;
        }

        [HttpGet]
        [Route("invoice/{invoiceNo}")]
        [Route("shoppingcart/orderconfirmation/{invoiceNo}")]
        public async Task<ActionResult> OrderConfirmation(string invoiceNo)
        {
            var model = CreateViewModel<OrderFormViewModel>();
            var userId = AppUserState.UserId;

            bool isPrintInvoice = Request.Path.Value?.Contains("/invoice/", StringComparison.OrdinalIgnoreCase) ?? false;

            var invoiceBusiness = BusinessFactory.GetInvoiceBusiness();
            var invoice = invoiceBusiness.LoadByInvNo(invoiceNo);

            if(invoice == null)return RedirectToAction("Index");

            if (invoice.IsTemporary)
            {
                return Redirect("/shoppingcart");
            }

            // Only allow access for user who created this invoice - or admin
            if (!AppUserState.IsAdmin && invoice.CustomerId != userId)
            {
                if (isPrintInvoice)
                {
                    return Redirect("~/account/signin?ReturnUrl=" + Request.Path.Value);
                }
                return RedirectToAction("Index");
            }

            model.InvoiceModel = new InvoiceViewModel(invoice);

            if (!isPrintInvoice)
            {
               await AppUtils.SendOrderConfirmationEmail(model, ControllerContext);

                if (invoice.CreditCardResult.IsApproved())
                {
                    invoice.Completed = DateTime.Now;
                    invoiceBusiness.SendEmailItemConfirmations();
                }

                invoiceBusiness.Save();
            }

            if (Request.Path.Value.Contains("/invoice/"))
            {
                return View("Invoice", model);
            }

            return View(model);
        }

        #endregion

        #region Short Form Order Form

        #endregion



        #region Support Functions

        private void ClearInvoiceAppUserState()
        {
            AppUserState.InvoiceId = null;
            AppUserState.CartItemCount = 0;
        }

        #endregion
    }



    public class ShoppingCartViewModel : WebStoreBaseViewModel
    {
        public string CustomerId { get; set;  }

        public InvoiceViewModel Invoice { get; set;  }

        public string Sku { get; set; }

        public decimal Quantity { get; set; } = 1.0M;


        public string PromoCode {get; set; }

        public decimal OrderTotal { get; set;  }

        public decimal Tax { get; set;  }

        public decimal SubTotal { get; set;  }
        public string InvoiceId { get; internal set; }
    }

}
