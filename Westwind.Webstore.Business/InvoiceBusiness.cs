
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Westwind.CreditCardProcessing;
using Westwind.Utilities;
using Westwind.Utilities.InternetTools;
using Westwind.Webstore.Business.Entities;
using Westwind.Webstore.Business.Properties;
using Westwind.Webstore.Business.Utilities;

namespace Westwind.Webstore.Business
{

    public class InvoiceBusiness : WebStoreBusinessObject<Invoice>
    {
        /// <summary>
        /// Override that can be used when explicitly saving line items
        /// where the Discount can be manually applied.
        /// (such as the line item order editor)
        /// Used from OrderManagerApiManager
        /// </summary>
        public bool DontApplyPromoCodes { get; set; }

        public InvoiceBusiness(WebStoreContext context) : base(context)
        {
        }

        /// <summary>
        /// Loads an invoice by Invoice Number rather than by Pk
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        public Invoice LoadByInvNo(string invoiceNumber)
        {
            return LoadBase(i => i.InvoiceNumber == invoiceNumber);
        }

        #region Operational

        /// <summary>
        /// Updates an invoice with a customers base information
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="invoice"></param>
        public void UpdateCustomerReferences(Customer customer, Invoice invoice = null)
        {
            if (invoice == null)
                invoice = Entity;

            invoice.Customer = customer;
            invoice.CustomerId = customer.Id;
            invoice.BillingAddress = CustomerBusiness.GetBillingAddress(customer);
        }


        /// <summary>
        /// Deletes expired
        /// </summary>
        /// <param name="saveInvoices"></param>
        public void DeleteExpiredTemporaryInvoices(bool saveInvoices = true)
        {
            var invPks = Context.Invoices
                        .Where(inv => inv.IsTemporary && inv.InvoiceDate < DateTime.Now.AddDays(1))
                        .Select(inv => inv.Id);

            foreach (var id in invPks)
            {
                Db.ExecuteNonQuery("delete from LineItems where invoiceId = @0;\n" +
                                   "delete from invoices where id = @0", id );
            }

            // delete orphaned lineitems
            Db.ExecuteNonQuery("delete from LineItems where invoiceId not in (select id from Invoices)");
        }
        #endregion

        #region Queries



        public IQueryable<InvoiceListItem> GetInvoices(string search = null, int maxCount = 2000)
        {
            string lsearch = search?.ToLower() ?? string.Empty;

            var invBase = Context.Invoices
                .AsNoTracking()
                .Include("Customer")
                .Include("LineItems");

            if (!string.IsNullOrEmpty(search))
            {

                if (lsearch == "this year")
                {
                    var dt = new DateTime(DateTime.Now.Year, 1, 1);
                    invBase = invBase.Where(inv => inv.InvoiceDate >= dt);
                } else if (lsearch == "last year")
                {
                    var dt = new DateTime(DateTime.Now.Year -1, 1, 1);
                    var dt2 =new DateTime(DateTime.Now.Year, 1, 1);
                    invBase = invBase.Where(inv => inv.InvoiceDate >= dt && inv.InvoiceDate < dt2);
                }
                else if (lsearch == "this month")
                {
                    var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    invBase = invBase.Where(inv => inv.InvoiceDate >= dt);
                }
                else if (lsearch == "last month")
                {
                    var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month-1, 1);
                    var dt2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    invBase = invBase.Where(inv => inv.InvoiceDate >= dt && inv.InvoiceDate < dt2);
                }
                else if (lsearch == "90 days" || lsearch == "90")
                {
                    var dt = DateTime.Now.AddDays(-90);
                    invBase = invBase.Where(inv => inv.InvoiceDate >= dt);
                }
                else if (lsearch == "60 days" || lsearch == "60")
                {
                    var dt = DateTime.Now.AddDays(-60);
                    invBase = invBase.Where(inv => inv.InvoiceDate >= dt);
                }
                else if (lsearch == "30 days" || lsearch == "30")
                {
                    var dt = DateTime.Now.AddDays(-30);
                    invBase = invBase.Where(inv => inv.InvoiceDate >= dt);
                }
                else if (lsearch == "unapproved" || lsearch == "unpaid")
                {
                    invBase = invBase.Where(inv => inv.CreditCardResult.ProcessingResult != "APPROVED" &&
                                                         inv.CreditCardResult.ProcessingResult != "PAID IN FULL" &&
                                                         inv.InvoiceDate > DateTime.Now.AddMonths(-18)) ;
                }
                else if (lsearch == "authorized")
                {
                    invBase = invBase.Where(inv => inv.CreditCardResult.ProcessingResult == "AUTHORIZED") ;
                }
                // By License Key
                else if (StringUtils.Occurs(lsearch, '-') == 3 && lsearch.Length == 23)
                {
                    invBase = invBase.Where(inv => inv.LineItems.Any(li => li.LicenseSerial == search));
                }
                else if (lsearch == "all" || lsearch == "recent")
                {
                    // do nothing - not recommended
                }
                else
                {
                    invBase = invBase.Where(i => i.InvoiceNumber.Contains(search) ||
                                                 i.CustomerId == search ||
                                                 i.Customer.Lastname.Contains(search) ||
                                                 i.Customer.Company.Contains(search) ||
                                                 i.Customer.Email.Contains(search) ||
                                                 i.PoNumber.Contains(search));
                }
            }

            IQueryable<Invoice> tlist = invBase
                .AsNoTracking()
                .Where(inv => !inv.IsTemporary)
                .OrderByDescending(inv => inv.InvoiceDate);

            if (lsearch == "recent")
            {
                tlist = tlist.Take(10);
            }

            return tlist
                .Select(inv => new InvoiceListItem
                    {
                        InvoiceNumber = inv.InvoiceNumber,
                        Name = inv.Customer.Fullname,
                        Company = inv.Customer.Company,
                        Email = inv.Customer.Email,
                        InvoiceDate = inv.InvoiceDate,
                        InvoiceTotal = inv.InvoiceTotal,
                        Status = inv.CreditCardResult.ProcessingResult,
                        Id = inv.Id,
                        CustId = inv.CustomerId,
                        LineItems = inv.LineItems
                    })
                .Take(maxCount);
        }

        public async Task<List<Invoice>> GetRecentInvoices(string customerId, int maxCount = 5)
        {
            return await Context.Invoices
                .AsNoTracking()
                .Include("Customer")
                .Include("LineItems")
                .Where(inv => inv.CustomerId == customerId && !inv.IsTemporary)
                .OrderByDescending(inv => inv.InvoiceDate)
                .Take(maxCount)
                .ToListAsync();
        }

        public async Task<List<Invoice>> GetOpenInvoices(string customerId)
        {
            return await Context.Invoices
                .AsNoTracking()
                .Include("Customer")
                .Where(inv => inv.CustomerId == customerId && inv.Completed == null && !inv.IsTemporary)
                .OrderByDescending(inv => inv.InvoiceDate)
                .ToListAsync();
        }

        #endregion


        #region LineItems

        /// <summary>
        /// Calculates invoice totals and assigns them to the passed invoice
        /// or the active Entity instance.
        /// </summary>
        /// <param name="invoice">Invoice entity or the current active Entity</param>
        /// <returns></returns>
        public virtual decimal CalculateTotals(Invoice invoice = null)
        {
            if (invoice == null)
                invoice = Entity;

            if (invoice == null)
                return 0M;

            return invoice.CalculateTotals();
        }

        /// <summary>
        /// Calculates the tax for an invoice
        ///
        /// Automatically called from CalculateTotals
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public decimal CalculateTax(Invoice invoice = null)
        {
            if (invoice == null)
                invoice = Entity;

            if (invoice == null)
                return 0M;

            return invoice.CalculateTax();
        }

        /// <summary>
        /// Adds an item to the active entity.
        ///
        /// Does not save the invoice, but recalculates totals
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="qty"></param>
        /// <param name="discount"></param>
        public LineItem AddLineItem(string sku, decimal qty = 1M, decimal discount = 0.0M, bool createEmptyLineItem = false)
        {
            LineItem lineItem = null;

            if (!createEmptyLineItem && string.IsNullOrEmpty(sku))
                return null;

            if (qty < 0.01M)
                return null;

            if (createEmptyLineItem)
            {
                lineItem = new LineItem();
                Entity.LineItems.Add(lineItem);
                return lineItem;
            }

            var productBusiness = BusinessFactory.Current.GetProductBusiness(Context);
            var product = productBusiness.LoadBySku(sku);
            if (product == null)
            {
                SetError(productBusiness.ErrorMessage);
                return null;
            }

            if (!product.IsFractional && qty % 1 != 0)
            {
                var rounded = Math.Round(qty, 0);
                if (rounded < 0.01M)
                    return null;
                qty = rounded;
            }

            var existing = Entity.LineItems.FirstOrDefault(li => li.Sku == sku);
            if (existing != null && discount == existing.DiscountPercent)
            {
                existing.Quantity += qty;
            }
            else
            {
                lineItem = new LineItem
                {
                    Sku = product.Sku,
                    Description = product.Description,
                    Price = product.Price,
                    Quantity = qty,
                    DiscountPercent = discount,
                    ItemImage = product.ItemImage,
                    AutoRegister = product.AutoRegister,
                    UseLicensing = product.UseLicensing
                };
                Entity.LineItems.Add(lineItem);
            }

            CalculateTotals(Entity);

            return lineItem;
        }

        public LineItem GetLineItem(string lineItemNumber)
        {
            return Context.LineItems.FirstOrDefault(li => li.Id == lineItemNumber);
        }


        /// <summary>
        /// Removes an item from the lineitems collection
        /// </summary>
        /// <param name="lineItemId">Id of the item to delete</param>
        /// <param name="invoice">instance of the invoice on which to delete.</param>
        public void RemoveLineItem(string lineItemId, Invoice invoice = null)
        {
            if (string.IsNullOrEmpty(lineItemId))
                return;

            if (invoice == null)
                invoice = Entity;

            if (invoice == null)
                return;

            var item = invoice.LineItems.FirstOrDefault(li => li.Id == lineItemId);
            if (item != null)
                invoice.LineItems.Remove(item);

            CalculateTotals();
        }

        /// <summary>
        /// Fixes up a quantity to be added, to ensure it's not fractional
        /// when the SKU doesn't support it - it goes to the database to
        /// retrieve the item by sku.
        ///
        /// If Sku is not fractional, qty is rounded.
        /// </summary>
        /// <param name="sku">Sku - looked up in Product db</param>
        /// <param name="qty">quantity to fix up</param>
        /// <returns></returns>
        public decimal FixLineItemQuantityForFractionalItem(string sku, decimal qty)
        {
            if (qty % 1 == 0)
                return qty; // not fractional

            // check if Product is fractional
            var isFractional = Db.ExecuteScalar("select IsFractional from Products where sku=@0", sku);
            if (isFractional == null)
                return 0;

            if (!(bool)isFractional)
            {
                qty = Math.Round(qty, 0);
            }

            return qty;
        }

        #endregion

        #region Promo Codes

        /// <summary>
        /// Applies one or more promo codes to an order
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public virtual bool ApplyPromoCodes(string codes)
        {
            string[] promos = new string[] { string.Empty };

            if (!string.IsNullOrEmpty(codes))
            {
                promos = codes.Split(',', ';');
                if (promos.Length == 0)
                    // we have to apply the empty code in case it changed
                    promos = new string[] { string.Empty };
            }

            foreach (string promo in promos)
            {
                ApplyPromoCode(promo);
            }

            return true;
        }


        /// <summary>
        /// Applies a promo code to an order.
        ///
        /// Updates existing lineitems but does not save.
        /// </summary>
        /// <param name="promoCode"></param>
        /// <returns></returns>
        protected virtual bool ApplyPromoCode(string promoCode)
        {
            // no need to hit db if no promo codes are set
            var busLookups = BusinessFactory.Current.GetLookupBusiness();
            decimal percentage = busLookups.GetPromoCodePercentage(promoCode);

            // nothing to do here
            if (Entity?.LineItems == null)
                return true;

            foreach (var lineItem in Entity.LineItems)
            {
                lineItem.DiscountPercent = percentage;
                lineItem.CalculateItemTotal();
            }

            return true;
        }

        #endregion

        #region CreditCard Processing




        public bool ProcessCreditCard()
        {
            var invoice = Entity;
            var payConfig = wsApp.Configuration.Payment;

            if (!OnBeforeProcessCreditCard())
                return false;

            // Special case TEST and AVS failure promo code allows order to be 'deferred'
            // without online processing - gets orders placed but not processed at all yet
            if ((Entity.PromoCode == "TEST" || Entity.PromoCode == "AVS"))
                return true;

            if (Entity.CreditCardResult.ProcessingResult == "APPROVED")
            {
                SetError(WebStoreBusinessResources.OrderAlreadyProcessed);
                return false;
            }

            // Check to see if this order can be processed without
            // manual cchecks (such as upgrades etc.)
            bool CanAutoConfirm = this.CanAutoConfirm();

            // Assume we are doing a sale immediately
            ccProcessTypes cardProcessType = wsApp.Configuration.Payment.DefaultCardProcessType;

            if (!CanAutoConfirm)
                cardProcessType = ccProcessTypes.PreAuth;


            // If we have authorized orders they now need to be captured (2 step process)
            if (Entity.CreditCardResult.ProcessingResult == "AUTHORIZED")
                cardProcessType = ccProcessTypes.AuthCapture;

            CreditCardProcessorBase processor = null;

            bool isError = false;
            try
            {
                processor = CreditCardProcessorBase.CreateProcessor(ccProcessors.Braintree);
                if (processor == null)
                {
                    SetError(WebStoreBusinessResources.InvalidCreditCardProcessor);
                    return false;
                }

                if (invoice.InvoiceTotal == 0)
                {
                    SetError(WebStoreBusinessResources.OrderAmountCantBeZero);
                    return false;
                }

                if (payConfig.TestMode)
                    processor.Configuration.UseTestTransaction = true;

                processor.Merchant.MerchantId = payConfig.MerchantId;
                processor.Merchant.PrivateKey = payConfig.PrivateKey;
                processor.Merchant.PublicKey = payConfig.PublicKey;
                processor.Merchant.MerchantPassword = payConfig.MerchantPassword;

                // Tell whether we do SALE or Pre-Auth
                processor.Order.ProcessType = cardProcessType;

                // Disable this for testing to get provider response
                processor.Configuration.UseLocalMod10Check = false; //true;

                processor.Configuration.Timeout = payConfig.ProcessConnectionTimeoutSeconds; // In Seconds

                processor.Configuration.LogFile = payConfig.LogFile;
                processor.Configuration.ReferringUrl = payConfig.ReferingOrderUrl;


                // Name can be provided as a single string or as firstname and lastname
                //CC.Name = Cust.Firstname.TrimEnd() + " " + Cust.Lastname.TrimEnd();
                processor.BillingInfo.FirstName = invoice.Customer.Firstname;
                processor.BillingInfo.LastName = invoice.Customer.Lastname;

                processor.BillingInfo.Company = invoice.Customer.Company;
                processor.BillingInfo.Address = invoice.BillingAddress.StreetAddress.GetLines(1).FirstOrDefault();

                processor.BillingInfo.State = invoice.BillingAddress.State;
                processor.BillingInfo.City = invoice.BillingAddress.City;
                processor.BillingInfo.PostalCode = invoice.BillingAddress.PostalCode;
                processor.BillingInfo.CountryCode = invoice.BillingAddress.CountryCode; // 2 Character Country ID
                processor.BillingInfo.Phone = invoice.Customer.Telephone;
                processor.BillingInfo.Email = invoice.Customer.Email;
                processor.BillingInfo.IpAddress = invoice.CreditCard.IpAddress;

                processor.Order.OrderAmount = invoice.InvoiceTotal;
                processor.Order.TaxAmount = invoice.Tax; // Optional

                processor.Order.CardNumber = invoice.CreditCard.CardNumber;
                processor.Order.CardExpiration = invoice.CreditCard.Expiration;
                processor.Order.SecurityCode = invoice.CreditCard.SecurityCode;

                processor.Order.ClientNonce = invoice.CreditCard.Nonce;
                processor.Order.ClientDescriptor = invoice.CreditCard.Descriptor;
                if (invoice.CreditCard.Descriptor == "PayPalAccount") // BrainTree PayPal Processing
                {
                    invoice.CreditCard.Type = "PayPal";
                }
                else
                    invoice.CreditCard.Type = "CC";

                // Transaction id for auth code
                processor.Order.TransactionId = invoice.CreditCardResult.TransactionId;

                // Make this Order Unique
                processor.Order.OrderId = invoice.InvoiceNumber + "_" + DateTime.Now.Millisecond;

                // Credit Card formatting is very specific:
                // 22 characters total, all upper case. Company 12 characters * 9 characters for invoice number (or whatever)
                processor.Order.Comment =
                    $"{wsApp.Configuration.Company.CompanyName} #{invoice.InvoiceNumber.ToUpper()}";

                var result = processor.Process(invoice.CreditCardResult.TransactionId);

                if (string.IsNullOrEmpty(invoice.BillingAddress.PostalCode))
                    invoice.BillingAddress.PostalCode = processor.BillingInfo?.PostalCode;
                if (string.IsNullOrEmpty(invoice.BillingAddress.CountryCode))
                {
                    invoice.BillingAddress.PostalCode = processor.BillingInfo?.CountryCode;
                    invoice.BillingAddress.Country = processor.BillingInfo?.Country;
                }

                if (result == null)
                {
                    ErrorMessage = WebStoreBusinessResources.CardProcessingFailed;
                    invoice.CreditCardResult.ProcessingError = ErrorMessage;

                    // preserve existing setting from pre-auth
                    if (string.IsNullOrEmpty(invoice.CreditCardResult.ProcessingResult))
                        invoice.CreditCardResult.ProcessingResult = "FAILED";

                    return false;
                }

                isError = !result.IsSuccess;

                invoice.CreditCardResult.ProcessingResult = result.ValidatedResult.ToString().ToUpper();
                ErrorMessage = result.Message;
                invoice.CreditCardResult.ProcessingError = ErrorMessage;

                // Always write out the raw response
                invoice.CreditCardResult.RawProcessingResult = result.RawProcessorResult;

                invoice.CreditCardResult.AuthCode = result.AuthorizationCode;
                invoice.CreditCardResult.TransactionId = result.TransactionId;
                invoice.CreditCardResult.AvsCode = result.AvsResultCode;

                // Clear out credit card info as soon as card is approved
                if (payConfig.ClearCardInfoAfterApproval &&
                    invoice.CreditCardResult.ProcessingResult == "APPROVED" ||
                    invoice.CreditCardResult.ProcessingResult == "AUTHORIZED")
                {
                    invoice.CreditCard.CardNumber = null;
                    invoice.CreditCard.Expiration = null;
                    invoice.CreditCard.SecurityCode = null;
                    invoice.CreditCard.Nonce = null;
                    invoice.CreditCard.Descriptor = null;

                    if (invoice.CreditCardResult.ProcessingResult == "APPROVED")
                        invoice.CreditCardResult.RawProcessingResult = ""; // don't keep if we've processed
                }

                if (invoice.CreditCardResult.ProcessingResult == "DECLINED")
                {
                    invoice.CreditCard.CardNumber = null;
                    invoice.CreditCard.Expiration = null;
                    invoice.CreditCard.SecurityCode = null;
                    invoice.CreditCard.Nonce = null;
                    invoice.CreditCard.Descriptor = null;
                }
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
                invoice.CreditCardResult.ProcessingResult = "FAILED";
                invoice.CreditCardResult.RawProcessingResult =
                    WebStoreBusinessResources.ProcessingError + ": " + ex.Message;
                invoice.CreditCardResult.ProcessingError =
                    WebStoreBusinessResources.ProcessingError + ": " + ex.Message;

                isError = true;
            }

            // final auth capture failed - stay authorized but not captured
            if (isError && !string.IsNullOrEmpty(invoice.CreditCardResult.AuthCode))
                invoice.CreditCardResult.ProcessingResult = "AUTHORIZED";

            return !isError;
        }

        protected virtual bool OnBeforeProcessCreditCard()
        {




            return true;
        }


        /// <summary>
        /// This method checks to see if the current invoice is capable of
        /// being confirmed electronically by checking each lineitem and
        /// its regauto flag.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanAutoConfirm(bool checkForLicensePresent = false)
        {
            if (!string.IsNullOrEmpty(Entity.PromoCode))
                return false;

            // If there is any item that can't autoconfirm
            // don't allow the order to be processed as online as a SALE - use Authorize
            foreach (var lineItem in Entity.LineItems)
            {
                // no automatic registration
                if (!lineItem.AutoRegister)
                    return false;

                string countryCode = Entity.BillingAddress.CountryCode;
                string[] whiteList = { "US", "CA", "DE", "GB", "AU", "NZ", "AT", "CH", "FR", "NL", "BE", "IT", "DK", "SE", "NO", "GR", "ES","IE", "SI", "CZ", "IS", "IL", "CL", "BR", "AR" };
                if (!whiteList.Any(w => w == countryCode))
                    return false;

                if (checkForLicensePresent)
                {
                    // uses a license but license has not been applied
                    if (lineItem.UseLicensing && string.IsNullOrEmpty(lineItem.LicenseSerial))
                        return false;
                }
            }

            return true;
        }


        #endregion

        #region Licensing

        /// <summary>
        /// Updates each line item by retrieving license information
        /// and storing it in the lineitems.
        ///
        /// Does not save only updates the fields.
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="force">if true forces data to be updated. Otherwise if already filled license is not reloaded</param>
        /// <param name="saveInvoice">if true the invoice is saved to disk</param>
        /// <returns></returns>
        public bool UpdateLineItemsLicenses(bool force = false, bool saveInvoice = false)
        {
            // Check if we should apply licensing at all
            if (!wsApp.Configuration.Licensing.IsLicensingEnabled ||
                !Entity.CreditCardResult.IsApproved())
                return true;

            bool result = true;

            // *** Retrieve the 'List' table
            var lineItems = Entity.LineItems;

            var productBus = new ProductBusiness(Context);

            // Licensing Service client
            var lic = new InvoiceLicensing(this);

            // *** Loop through all lineitems and see which have changed
            foreach (var lineItem in lineItems)
            {
                if (!string.IsNullOrEmpty(lineItem.LicenseSerial))
                    continue;

                CreateLineItemLicense(lineItem, productBus, lic);
            }

            if (result && saveInvoice)
                Save();

            return result;
        }

        public bool CreateLineItemLicense(LineItem lineItem,
            ProductBusiness productBus = null,
            InvoiceLicensing lic = null,
            bool force = false)
        {
            bool result = true;

            if (productBus == null)
                productBus = new ProductBusiness(Context);
            if (lic == null)
                lic = new InvoiceLicensing(this);

            if (!force && !string.IsNullOrEmpty(lineItem.LicenseSerial))
                return result;

            var item = productBus.LoadBySku(lineItem.Sku);
            if (item == null)
                return result; // nothing to do - OK

            if (item.UseLicensing)
            {
                // update the item instance
                if (!lic.CreateLicenseForLineItem(lineItem, item))
                {
                    result = false;
                    ErrorMessage += "License creation failed for: " + lineItem.Sku + "\n";
                }
            }
            else
                lineItem.LicenseSerial = item?.RegistrationPassword;

            return result;
        }

        #endregion


        #region Crud Overrides


        protected override void OnAfterLoaded(Invoice entity)
        {
            if (entity == null) return;

            base.OnAfterLoaded(entity);

            if (!string.IsNullOrEmpty(entity.ShippingAddressJson))
                entity.ShippingAddress = JsonSerializationUtils.Deserialize<Address>(entity.ShippingAddressJson);
            else
                entity.ShippingAddress = new Address();

            if (!string.IsNullOrEmpty(entity.BillingAddressJson))
                entity.BillingAddress = JsonSerializationUtils.Deserialize<Address>(entity.BillingAddressJson);
            else
                entity.BillingAddress = new Address();
        }

        protected override bool OnBeforeSave(Invoice entity)
        {
            if (!base.OnBeforeSave(entity))
                return false;

            if (entity.ShippingAddress != null && !entity.ShippingAddress.IsEmpty())
                entity.ShippingAddressJson = JsonSerializationUtils.Serialize(entity.ShippingAddress);
            else
                entity.ShippingAddressJson = null;

            if (entity.BillingAddress != null && !entity.BillingAddress.IsEmpty())
                entity.BillingAddressJson = JsonSerializationUtils.Serialize(entity.BillingAddress);
            else
                entity.BillingAddressJson = null;

            if (entity._extraProperties != null)
                entity._extraPropertiesStorage = JsonSerializationUtils.Serialize(entity._extraProperties);
            else
                entity._extraPropertiesStorage = null;

            if (!DontApplyPromoCodes)
                ApplyPromoCodes(entity.PromoCode);

            CalculateTotals();

            // make sure we force customer password to be encoded since it doesn't go through
            // it's own save cycle
            if (entity.Customer != null && entity.Customer.IsNew)
            {
                entity.Customer.Password = CustomerBusiness.HashPassword(entity.Customer.Password, entity.Customer.Id);
            }

            return true;
        }

        protected override bool OnAfterSave(Invoice entity)
        {
            if (!base.OnAfterSave(entity))
                return false;

            if (entity.Customer != null && entity.Customer.IsNew)
            {
                entity.Customer.IsNew = false;
            }

            return true;
        }

        protected override bool OnBeforeDelete(Invoice entity)
        {
            entity.LineItems.Clear();
            return true;
        }

        #endregion

        #region Emails

        /// <summary>
        /// Sends a Confirmation email for the order by looping through all the
        /// lineitems and sending the confirmation notice from each.
        ///
        /// It also sends a reseller confirmation message to the Vendor
        /// if an EmailTo is specified in the item. The EmailTo can be
        /// an email address or HTTP link. If an Http link the XML of
        /// the message is posted to this link.
        /// </summary>
        /// <returns></returns>
        public bool SendEmailProductConfirmations()
        {
            if (Entity.LineItems.Count < 1)
            {
                ErrorMessage = WebStoreBusinessResources.NoLineItemsToConfirm;
                return false;
            }

            var busProduct = BusinessFactory.Current.GetProductBusiness(Context);
            string Error = "";

            var invoice = Entity;
            var customer = Entity.Customer;

            // Loop through each of the items
            foreach (var lineItem in invoice.LineItems)
            {
                var item = busProduct.LoadBySku(lineItem.Sku);
                if (item == null)
                    continue;

                string regText = item.RegistrationItemConfirmation;
                string descript = item.Description.TrimEnd();

                if (item.UseLicensing && string.IsNullOrEmpty(lineItem.LicenseSerial))
                {
                    var msg = $"License has not been created yet for {lineItem.Sku}.";
                    Error += msg + "\r\n";
                    OnStatusMessage("Confirming " + item.Description.TrimEnd() + " to " + customer.Email +
                                    " failed - license not generated yet.");
                    continue;
                }

                if (!string.IsNullOrEmpty(regText))
                {
                    regText = regText.Replace("{{SerialNumber}}",
                        !string.IsNullOrEmpty(lineItem.LicenseSerial)
                            ? lineItem.LicenseSerial
                            : item.RegistrationPassword);

                    OnStatusMessage("Confirming " + descript + " to " + customer.Email);

                    var email = invoice.ConfirmationEmail;
                    if (string.IsNullOrEmpty(email))
                        email = customer.Email;

                    var emailer = new Emailer();
                    if (!emailer.SendEmail(email, descript + " (Registration Confirmation)", regText, EmailModes.plain))
                    {
                        Error += $"Item {descript} failed to confirm: {emailer.ErrorMessage}";
                        OnStatusMessage(
                            $"Confirmation Email for {item.Description.TrimEnd()} to {customer.Email} failed.");
                    }
                }

                //// Send a vendor confirmation if EmailTo is set
                //if (item.EmailTo.TrimEnd() != "")
                //    VendorConfirmation()

            }

            if (!string.IsNullOrEmpty(Error))
            {
                ErrorMessage = Error;
                return false;
            }

            // Update the Invoice Date
            if (!invoice.IsShipping && invoice.Completed <= wsApp.EmptyDate)
            {
                if (invoice.Completed <= wsApp.EmptyDate)
                    invoice.Completed = DateTime.Now;

                if (string.IsNullOrEmpty(invoice.OrderStatus))
                    invoice.OrderStatus = WebStoreBusinessResources.EmailConfirmationSent;
            }

            return true;
        }


        // /// <summary>
        // /// Sends an order confirmation email
        // /// </summary>
        // /// <param name="subject"></param>
        // /// <param name="messageText"></param>
        // /// <param name="ContentType"></param>
        // /// <returns></returns>
        // public bool _SendOrderConfirmationEmail(string recipient, string subject, string messageText, EmailModes emailMode = EmailModes.plain)
        // {
        //     var emailConfig = wsApp.Configuration.Email;
        //
        //     var emailer = new Emailer();
        //     return emailer.SendEmail(recipient, subject, messageText, emailMode);
        //     if (!result)
        //         return false;
        //
        //     {
        //         Error += $"Item {descript} failed to confirm: {emailer.ErrorMessage}";
        //         OnStatusMessage(
        //             $"Confirmation Email for {item.Description.TrimEnd()} to {customer.Email} failed.");
        //     }
        //
        //
        // }


        public void VendorConfirmation()
        {
            //    // Create confirmation object and serialize
            //    VendorConfirmation Confirm = new VendorConfirmation();
            //    Confirm.InvNo = invoice.Invno.TrimEnd();
            //    Confirm.InvPk = invoice.Pk;
            //    Confirm.InvDate = invoice.Invdate;
            //    Confirm.PoNo = invoice.Ponumber.TrimEnd();

            //    Confirm.Name = customer.Firstname.TrimEnd() + " " + customer.Lastname.TrimEnd();
            //    Confirm.Company = customer.Company.TrimEnd();
            //    Confirm.Email = customer.Email.TrimEnd();
            //    Confirm.ShippingAddress = GetAddress(true);
            //    Confirm.BillingAddress = GetAddress(false);

            //    Confirm.ShipToCustomer = invoice.Shipdisks;
            //    Confirm.Referer = customer.Referral;

            //    Confirm.Sku = item.Sku.TrimEnd();
            //    Confirm.ItemDescription = item.Descript.TrimEnd();
            //    Confirm.Qty = LineItem.Qty;
            //    Confirm.Price = LineItem.Price;
            //    Confirm.Discount = LineItem.Discount;
            //    Confirm.ItemTotal = LineItem.Itemtotal;

            //    OnStatusMessage(WebStoreBusinessResources.VendorConfirmationFor +
            //                    " " + Confirm.ItemDescription +
            //                    " - " + item.Emailto);

            //    if (item.Emailto.ToLower().StartsWith("http"))
            //    {
            //        HttpClient Http = new HttpClient();
            //        Http.PostMode = HttpPostMode.Xml;
            //        Http.AddPostKey(Confirm.ToXml());
            //        Http.GetUrl(item.Emailto);
            //        if (Http.Error)
            //            Error += WebStoreBusinessResources.UnableToConfirmItemWithVendor +
            //                     ":\r\n" + Http.ErrorMessage + "\r\n";
            //    }
            //    else
            //    {
            //        if (!SendEmail(App.Configuration.CompanyName + " " +
            //                       WebStoreBusinessResources.OrderConfirmation + " " +
            //                       descript, Confirm.ToString(true), "text/plain", item.Emailto))
            //        {
            //            Error += string.Format(WebStoreBusinessResources.ItemFailedToConfirm + "\r\n");
            //        }
            //    }
        }
        #endregion

        #region Custom Events

        public Action<string> StatusMessage { get; set; }

        protected void OnStatusMessage(string message)
        {
        }

        #endregion



        public override string ToString()
        {
            return $"{ErrorMessage} #{Entity?.InvoiceNumber} {Entity.InvoiceTotal}";
        }
    }

    public class InvoiceListItem
    {
        public override string ToString()
        {
            return $"{{ InvoiceNumber = {InvoiceNumber}, Name = {Name}, Company = {Company}, InvoiceDate = {InvoiceDate}, InvoiceTotal = {InvoiceTotal}, Id = {Id}, CustId = {CustId} }}";
        }

        public string InvoiceNumber { get; init; }
        public string Name { get; init; }
        public string Company { get; init; }

        public string Email { get; init; }
        public DateTime InvoiceDate { get; init; }
        public decimal InvoiceTotal { get; init; }

        public string Status { get; set; }
        public string Id { get; init; }
        public string CustId { get; init; }

        public List<LineItem> LineItems { get; set; }
    }
}
