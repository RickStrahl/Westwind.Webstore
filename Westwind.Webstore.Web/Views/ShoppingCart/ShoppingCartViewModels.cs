using Westwind.Webstore.Business.Entities;
using Westwind.Webstore.Web.Models;

namespace Westwind.Webstore.Web.Views
{

    public class OrderProfileViewModel : ProfileViewModel
    {
        public InvoiceViewModel Invoice { get; set; } = new InvoiceViewModel();

    }

    public class OrderFormViewModel : WebStoreBaseViewModel
    {
        public InvoiceViewModel InvoiceModel { get; set; } = new InvoiceViewModel();


        public string ReCaptchaResult { get; set; }

        public string Nonce { get; set;  }

        public string Descriptor { get; set; }

        public string dtto { get; set; }

        public string Evc { get; set; }
    }

    public class OrderFormFastViewModel : OrderFormViewModel
    {
        public Product Product { get; set; }


        public string Sku { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Company { get; set; }

        public string Email { get; set; }

        public string StreetAddress { get; set; }
        public string CountryCode { get; set; }
        public string Country { get; set; }

        public string Password { get; set; }


        public decimal OrderTotal { get; set; }
        public decimal Tax { get; set; }
    }




    public class InvoiceViewModel
    {
        public InvoiceViewModel()
        {
            Invoice = new Invoice();
        }

        public InvoiceViewModel(Invoice invoice)
        {
            Invoice = invoice;
            if (Invoice == null)
                Invoice = new Invoice();
        }

        public Invoice Invoice { get; set; }


        public string Notes { get; set; }

        public string PromoCode { get; set; }

        public string PoNumber { get; set; }

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
        /// BrainTree Device Data
        /// </summary>
        public string DeviceData { get; set; }

        /// <summary>
        /// IP Address of the user making this order for reference
        /// </summary>
        public string IpAddress { get; set; }


        #region Line Item Formatting

        /// <summary>
        /// Determines whether line item images are displayed
        /// </summary>
        public bool NoLineItemImages { get; set;  }

        /// <summary>
        /// Determines whether line item qty can be edited
        /// </summary>
        public bool CanEditLineItemQuantity { get; set; }

        /// <summary>
        /// Determines whether you can edit line items
        /// </summary>
        public bool CanEditLineItems { get; set; }

        /// <summary>
        /// Determines whether lineitems can be deleted
        /// </summary>
        public bool CanDeleteLineItems { get; set; }

        /// <summary>
        /// Determines whether the promo code can be edited
        /// </summary>
        public  bool CanEditPromoCode { get; set; }



        #endregion
    }
}
