
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Westwind.Utilities;
using Westwind.Webstore.Business.Entities;

namespace Westwind.Webstore.Business.Entities
{

    public class Invoice
    {
        public Invoice()
        {

        }

        [Key]
        [StringLength(20)]
        public string Id { get; set; } = wsApp.NewId();

        /// <summary>
        /// An optional invoice number that human readable
        /// </summary>
        [StringLength(40)]
        public string InvoiceNumber { get; set; } = DataUtils.GenerateUniqueId(8);

        /// <summary>
        /// Date of this invoice
        /// </summary>
        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Completion date of the order
        /// </summary>
        public DateTime? Completed { get; set; }

        /// <summary>
        /// Determines whether this order is temporary until it is placed
        /// used for orders that are temporarily tracked until they are actually
        /// placed.
        public bool IsTemporary {get; set; }


        /// <summary>
        /// Any additional notes
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// If set shows who sold this invoice
        /// </summary>
        public string SoldBy { get; set; }

        #region Child Entities

        /// <summary>
        /// Determiones whether this order has a physical shipment that has
        /// to be mailed.
        /// </summary>
        public bool IsShipping { get; set; }

        /// <summary>
        /// Shipping address for this order. Only set if different from
        /// billing address used on this order.
        ///
        /// Stored as JSON in the entity
        /// </summary>
        [NotMapped]
        public Address ShippingAddress
        {
            get
            {
                if (_shippingAddress == null)
                {
                    if (!string.IsNullOrEmpty(ShippingAddressJson))
                        _shippingAddress = JsonSerializationUtils.Deserialize<Address>(ShippingAddressJson);
                    else
                        _shippingAddress = new Address();
                }

                return _shippingAddress;
            }
            set { _shippingAddress = value; }
        }

        private Address _shippingAddress;

        [JsonIgnore]
        public string ShippingAddressJson { get; set; }

        [NotMapped]
        public Address BillingAddress
        {
            get
            {
                if (_billingAddress == null)
                {
                    if (!string.IsNullOrEmpty(BillingAddressJson))
                        _billingAddress = JsonSerializationUtils.Deserialize<Address>(BillingAddressJson);
                    else
                        _billingAddress = new Address();
                }

                return _billingAddress;
            }
            set
            {
                _billingAddress = value;
            }
        }
        private Address _billingAddress;

        [JsonIgnore]
        public string BillingAddressJson { get; set; }

        public virtual List<LineItem> LineItems { get; set; } = new List<LineItem>();

        [StringLength(20)]
        public string CustomerId {get; set; }

        public virtual Customer Customer { get; set; }

        #endregion

        #region Totals

        [Column(TypeName = "decimal(18,4)")]
        public decimal SubTotal {get; set;}

        /// <summary>
        /// TaxRate used for this invoice
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal TaxRate { get; set; }


        /// <summary>
        /// Tax for this invoice
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal Tax {get; set;}

        /// <summary>
        /// Shipping Cost for this order if any
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal Shipping { get; set; }


        [Column(TypeName ="decimal(18,4)")]
        public decimal InvoiceTotal { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Weight { get; set; }

        /// <summary>
        /// Optional Promo code to apply to the order
        /// </summary>
        public string PromoCode { get; set; }

        #endregion

        #region Extra Properties

        /// <summary>
        /// JSON Storage for the ExtraProperties object
        /// </summary>
        public string ExtraPropertiesStorage
        {
            get
            {
                return _extraPropertiesStorage;
            }
            set
            {
                _extraPropertiesStorage = value;
                _extraProperties = null; // lazy load
            }

        }
        internal string _extraPropertiesStorage = null;


        /// <summary>
        /// Object that is serialized and allows storing additional data.
        ///
        /// MUST BE SAVED
        /// </summary>
        [NotMapped]
        public CustomerExtraProperties ExtraProperties
        {
            get
            {
                if (_extraProperties == null)
                {
                    if (!string.IsNullOrEmpty(ExtraPropertiesStorage))
                        _extraProperties = JsonSerializationUtils.Deserialize<CustomerExtraProperties>(ExtraPropertiesStorage);
                }

                if (_extraProperties == null)
                    _extraProperties = new CustomerExtraProperties();

                return _extraProperties;
            }
            set
            {
                _extraProperties = value;
            }
        }

        internal CustomerExtraProperties _extraProperties;


        #endregion

        #region Order Status

        /// <summary>
        /// Optional PO Number that customers can use to associate their
        /// order with this order.
        /// </summary>
        public string PoNumber { get; set; }

        /// <summary>
        /// Optional order status that describes the current state of this order
        /// </summary>
        public string OrderStatus { get; set; }


        public int OldPk { get; set; }


        /// <summary>
        /// Calculates the SubTotal and InvoiceTotal and assigns it to this invoice
        /// instance.
        /// </summary>
        /// <returns></returns>Invoice
        public decimal CalculateTotals()
        {
            decimal subTotal = 0M;

            foreach (var item in LineItems)
            {
                subTotal += item.CalculateItemTotal();
            }

            SubTotal = subTotal;

            CalculateTax();

            InvoiceTotal = SubTotal + Tax;

            return InvoiceTotal;
        }

        public decimal CalculateTax()
        {
            Tax = 0;
            TaxRate = 0;

            if (BillingAddress?.State == null)
                return 0M;

            if (BillingAddress.State.Equals(wsApp.Configuration.Payment.TaxState, StringComparison.OrdinalIgnoreCase))
            {
                TaxRate = wsApp.Configuration.Payment.TaxRate;
                Tax = SubTotal * TaxRate;
            }

            return Tax;
        }
        #endregion

        [Required]
        public InvoiceCreditCardData CreditCard { get; set;  } = new InvoiceCreditCardData();

        [Required]
        public InvoiceCreditCardResultData CreditCardResult { get; set; } = new InvoiceCreditCardResultData();


        public bool CanProcessCreditCard()
        {
            return !(string.IsNullOrEmpty(CreditCard.Nonce) &&
                     string.IsNullOrEmpty(CreditCard.CardNumber) &&
                     string.IsNullOrEmpty(CreditCardResult.AuthCode) &&
                     string.IsNullOrEmpty(CreditCardResult.TransactionId));
        }

        public override string ToString()
        {
            if (Customer != null)
            {
                return $"{InvoiceNumber} - {InvoiceDate.ToString("MMM dd, yyyy")} - {InvoiceTotal} - {(string.IsNullOrEmpty(Customer.Fullname) ? Customer.Company : Customer.Fullname)}";
            }

            return $"{InvoiceNumber} - {InvoiceDate.ToString("MMM dd, yyyy")} - {InvoiceTotal}";
        }
    }

    [Owned]
    public class InvoiceCreditCardData
    {
        /// <summary>
        /// The full credit card number.
        /// IMPORTANT: Do not store this number if possible and rely on Nonce processing
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Last four of Credit card captured for identification
        /// </summary>
        public string LastFour { get; set; }

        /// <summary>
        /// Expiration date in the format 12/2020
        /// </summary>
        public string Expiration { get; set; }

        /// <summary>
        /// Card security code from the back of the card
        /// </summary>
        public string SecurityCode { get; set; }

        /// <summary>
        /// Type of credit card. PP for PayPal, CC for credit card or the actual type of card
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Nonce used when capturing CC info on the client side
        /// </summary>
        public string Nonce { get; set; }

        /// <summary>
        /// Descriptor used when capturing CC info on the client side
        /// </summary>
        public string Descriptor { get; set; }

        /// <summary>
        /// IP Address of the user making this order for reference
        /// </summary>
        public string IpAddress { get; set; }

        public override string ToString()
        {
            return this.Type;
        }
    }

    [Owned]
    public class InvoiceCreditCardResultData
    {
        /// <summary>
        /// The credit card authorization code for this transaction.
        /// This code is needed for Pre-Auth and Capture operations.
        /// </summary>
        public string AuthCode { get; set; }

        /// <summary>
        /// The transaction id for this transaction. This is the final
        /// code that is stored with the CC company.
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// AVS Results code if there's a processing problem with AVS
        /// </summary>
        public string AvsCode { get; set; }

        /// <summary>
        /// Returns a simple result code from Credit Card processing
        /// APPROVED, DECLINED, FRAUD, FAILED or empty/null
        /// </summary>///
        public string ProcessingResult { get; set; }

        /// <summary>
        /// Returns the raw processing result from the processor
        /// </summary>
        public string RawProcessingResult { get; set; }

        public string ProcessingError { get; set; }


        public bool IsApproved()
        {
            return ProcessingResult != null &&
                   ProcessingResult.Equals("APPROVED", StringComparison.OrdinalIgnoreCase);
        }


        public override string ToString()
        {
            if (string.IsNullOrEmpty(ProcessingResult))
                return "Unprocessed";

            if (ProcessingResult.Equals("AUTHORIZED"))
                return "Payment Pending";

            return ProcessingResult ?? "Unprocessed";
        }
    }


}
