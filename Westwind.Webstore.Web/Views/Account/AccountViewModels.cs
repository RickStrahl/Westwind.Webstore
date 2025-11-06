using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Primitives;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;
using Westwind.Webstore.Web.Models;

namespace Westwind.Webstore.Web.Views
{
    public class ProfileViewModel : WebStoreBaseViewModel
    {
        public CustomerViewModel Customer { get; set; } = new CustomerViewModel();

        public Address BillingAddress { get; set;  } = new Address();

        public Address ShippingAddress { get; set; }

        public bool IsNewUser { get; set; }

        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        public bool IsOrderProfile { get; set; }

        /// <summary>
        /// Return URL if successfully saved
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// ValidationKey captured
        /// </summary>
        public string Evc { get; set;  }
    }



    public class CustomerViewModel : WebStoreBaseViewModel
    {
        public string Id { get; set; } = wsApp.NewId();

        [Required]
        public string Email {get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }

        public string Company { get; set; }

        public string CustomerNotes { get; set;  }

        // public string TwoFactorKey { get; set; }

        public bool UseTwoFactorAuth { get; set; }
    }

    public class MvpPerksViewModel : CustomerViewModel
    {
        public string City { get; set;  }

        public string CountryCode { get; set; }
        public string MvpLink { get; set; }
        public bool ReceiveMarkdownMonster { get; set; }
        public bool ReceiveWebSurge { get; set; }

        public bool ReceiveDocumentationMonster { get; set; }
        public bool IsDiscountRequest { get; set; }

    }


    public class OrderHistoryViewModel : WebStoreBaseViewModel
    {
        public List<Invoice> InvoiceList { get; set; }
        public Customer Customer { get; set; }
        public string DisplayMode { get; set; } = "Recent";
    }

    public class SigninViewModel : WebStoreBaseViewModel
    {
        public Customer Customer { get; set; }

        public string Username { get; set; }

        [Required] public string Email { get; set; }

        [Required] public string Password { get; set; }

        public bool RememberPassword { get; set; }

        public string ReturnUrl { get; set; }

        public string TwoFactorValidationCode { get; set; }

        #region Token Return

        public bool IsTokenRequest { get; set; }

        public string Scope { get; set; }

        public string UserToken { get; set; }

        public string TokenReturnUrl { get; set; }
        public string TokenId { get; set; }

        #endregion
    }


    public class SetupTwoFactorViewModel : WebStoreBaseViewModel
    {
        public string TwoFactorSetupKey { get; set; }

        public string QrCodeImageData { get; set; }
        public string ValidationKey { get; set; }
        public string CustomerSecretKey { get; set; }
    }

    public class TwoFactorValidationViewModel : WebStoreBaseViewModel
    {
        public string ValidationCode { get; set; }
        public string ReturnUrl { get; set; }

        #region Token Return

        public bool IsTokenRequest { get; set; }

        public string App { get; set; }

        public string UserToken { get; set; }

        public string TokenReturnUrl { get; set; }
        public string TokenId { get; set; }

        #endregion
    }

    public class PasswordRecoveryModel : WebStoreBaseViewModel
    {
        //    internal User User { get; set; }

        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string PasswordRecovery { get; set; }

        [Required]
        public string ValidationId { get; set; }



    }

    public class AddressBoxViewModel
    {
        /// <summary>
        /// The small headline above the address/name
        /// </summary>
        public string Title { get; set; } = "billing address";

        /// <summary>
        /// Override the name to display
        /// </summary>
        public string Name { get; set;  }

        /// <summary>
        /// Override the Company to display
        /// </summary>
        public string Company { get; set;  }


        /// <summary>
        /// If set Name and company are not rendered
        /// </summary>
        public bool dontShowNameAndCompany { get; set;  }

        public Address Address { get; set; }


    }

}
