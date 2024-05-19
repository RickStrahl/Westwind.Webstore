using Westwind.CreditCardProcessing;
using Westwind.Utilities.Configuration;

namespace Westwind.Webstore.Business
{


    /// <summary>
    /// Web Store specific configuration settings that are stored separate from the application.json
    /// configuration settings for general application behavior.
    ///
    /// If permissions allow this file is auto-created on startup.
    /// </summary>
    public class WebStoreConfiguration : AppConfiguration
    {
        public string ApplicationCompany { get; set; } = "West Wind Technologies";

        public string ApplicationDomain { get; set; } = "west-wind.com";

        /// <summary>
        /// The Application's home Url with a trailing slash
        /// </summary>
        public string ApplicationHomeUrl { get; set; } = "https://localhost:5201/";

        public string ApplicationName { get; set; } = "West Wind Web Store";
        public string ConnectionString { get; set; } =
            "server=.;database=WestwindWebStore;integrated security=yes;encrypt=false;";

        public string DatabaseName { get; set; } = "WestwindWebStore";

        public string ProductImageUploadFilePath { get; set; } = "/images/product-images/";
        public string ProductImageWebPath { get; set; } = "~/images/product-images/";

        public string Theme { get; set; } = "default";
        public string TitleBottom { get; set; } = "Web Store";
        public string TitleTop { get; set; } = "West Wind";
        public string DefaultCountryCode { get; set; } = "US";
        public string CurrencySymbol { get; set; } = "$";

        public EmailConfiguration Email { get; set; } = new EmailConfiguration();
        public PaymentConfiguration Payment { get; set; } = new PaymentConfiguration();
        public CompanyConfiguration Company { get; set; } = new CompanyConfiguration();

        public InventoryConfiguration Inventory { get; set; } = new InventoryConfiguration();

        public LicensingConfiguration Licensing { get; set; } = new LicensingConfiguration();

        public FraudConfiguration Security { get; set; } = new FraudConfiguration();

        public SystemConfiguration System { get; set; } = new SystemConfiguration();



        protected override IConfigurationProvider OnCreateDefaultProvider(string sectionName, object configData)
        {
            var provider = new JsonFileConfigurationProvider<WebStoreConfiguration>()
            {
                JsonConfigurationFile = "_webstore-configuration.json",
            };
            return provider;
        }

    }


    public class PaymentConfiguration
    {
        public string MerchantId { get; set; }

        public string PublicKey { get; set; }

        public string PrivateKey { get; set;  }


        public string MerchantPassword { get; set; }

        /// <summary>
        /// Determines if the transaction is run in processor test mode (varies by provider)
        /// </summary>
        public bool TestMode { get; set;  }


        /// <summary>
        /// If true cards are processed, otherwise orders are just accepted without credit card processing
        /// for potential later offline processing.
        /// </summary>
        public bool ProcessCardsOnline { get; set; } = true;

        /// <summary>
        /// Default process types: PreAuth, Sale, AuthCapture, Credit, Void etc.
        ///
        /// This is the default process type *if* the order can auto-confirm. Otherwise
        /// the order is processed as a pre-auth.
        /// </summary>
        public ccProcessTypes DefaultCardProcessType { get; set; } = ccProcessTypes.Sale;

        /// <summary>
        /// ProcessorTimeout in seconds
        /// </summary>
        public int ProcessConnectionTimeoutSeconds { get; set; } = 20;

        /// <summary>
        /// Optional log file where CC processing information and errors are written out to
        /// </summary>
        public string LogFile { get; set; } = "/Web Sites/store.west-wind.com/wwwroot/admin/cclog.txt";

        /// <summary>
        /// A referring order url
        /// </summary>
        public string ReferingOrderUrl { get; set; }

        public bool ClearCardInfoAfterApproval { get; set; } = true;

        public string TaxState { get; set; } = "HI";

        public decimal TaxRate { get; set; } = 0.04M;

        /// <summary>
        /// Transaction page that can be accessed by double-click on the
        /// payment status. {0} = TransactionId  {1} = Merchant Id
        /// </summary>
        public string TransactionHtmlLink { get; set;  } = "https://www.braintreegateway.com/merchants/{1}/transactions/{0}";
    }


    public class CompanyConfiguration
    {
        public string ReportCompanyLogoImage { get; set;  }
        public string CompanyName { get; set;  }
        public string Address { get; set;  }

        public string Telephone { get; set; }

        public string Email { get; set;  }
        public string WebSite { get; set; }
    }

    public class EmailConfiguration
    {
        public string AdminSenderEmail { get; set; }
        public string MailServer { get; set; }
        public string MailServerUsername { get; set; }
        public string MailServerPassword { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public bool UseTls { get; set; }

        /// <summary>
        /// Email Address(es) added to the CC list when confirmations are sent
        /// </summary>
        public string CcList { get; set; }

        /// <summary>
        /// Email Address(es) added to the CC list when Admin messages are sent
        /// </summary>
        public string AdminCcList { get; set; }

        /// <summary>
        /// Determines whether Admin emails are sent on:
        /// * Errors
        /// * CC Fraud
        /// </summary>
        public bool SendAdminEmails { get; set; }

        /// <summary>
        /// If true automatically sends product confirmations on
        /// orders processed through the admin interface. Otherwise
        /// explicit order confirmation is required.
        /// </summary>
        public bool AutoConfirmAdminOrders { get; set; } = false;
    }


    public class InventoryConfiguration
    {
        /// <summary>
        /// Base Shipping cost for physical items if no explicit per weight shipping cost
        /// is provided.
        /// </summary>
        public decimal BaseShippingCostPerWeightUnit { get; set; } = 5.0M;

        /// <summary>
        /// Shipping cost for secondary weight after the first weight unit: ie. 2nd, 3rd, 4th pound/oz after first.
        /// Generally shipping is most expensive for the first weight unit and gets cheaper for additional weight.
        /// Use this value to represent the discount for multiple items.
        /// </summary>
        public decimal ShippingPercentageForSecondaryWeightUnit { get; set; } = 0.5M;

        /// <summary>
        /// Shipping cost multiplier for international orders. Whatever the base price ends up
        /// being it's multiple by this value to get the international shipping cost.  
        /// </summary>
        public decimal ShippingCostInternationalMultiplier { get; set; } = 2M;
    }

    public class FraudConfiguration
    {

        /// <summary>
        /// Determines whether email addresses have to be validated in order
        /// for the account to be usable.
        /// </summary>
        public bool ValidateEmailAddresses { get; set; } = true;

        /// <summary>
        /// Determines whether two-factor authentication is optionally
        /// used to validate accounts.
        /// </summary>
        public bool UseTwoFactorAuthentication { get; set; } = false;

        /// <summary>
        /// Use ReCaptcha
        /// </summary>
        public bool UseOrderFormRecaptcha { get; set; } = true;

        /// <summary>
        ///  The site key embedded into the HTML page to process the capture
        /// </summary>
        public string  GoogleRecaptureSiteKey { get; set; }

        /// <summary>
        /// The secret key used to validate a recapture value
        /// captured from the the client
        /// </summary>
        public string GoogleRecaptureSecret { get; set; }

        public bool UseOrderFormTimeout { get; set; } = true;

        public int OrderFormMinimumSecondsTimeout { get; set; } = 10;

        public int OrderFormMaximumMinutesTimeout { get; set; } = 15;

    }

    public class LicensingConfiguration
    {
        public bool IsLicensingEnabled { get; set;  }

        public string ServerUrl { get; set;  }

        public string Username { get; set;  }

        public string Password { get; set;  }
    }

    public class SystemConfiguration
    {
        public bool LiveReloadEnabled { get; set; }
        public bool RedirectToHttps { get; set; }

        public bool ShowLocalization { get; set; } = true;

        public ErrorDisplayModes ErrorDisplayMode { get; set; } = ErrorDisplayModes.Application;
        public int CookieTimeoutDays { get; set; } = 2;

        public bool ShowConsoleRequestTimings { get; set; }


        public bool ShowConsoleDbCommands { get; set; }
    }

    public enum ErrorDisplayModes
    {
        Application,
        ApplicationPlusDetail,
        Developer
    }
}
