using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Westwind.Utilities;
using Westwind.Webstore.Business.Entities;
using Westwind.Webstore.Business.Properties;

namespace Westwind.Webstore.Business
{
    public class wsApp
    {
        public static WebStoreConfiguration Configuration { get; set; }

        public static wsAppConstants Constants = new wsAppConstants();

        public static bool IsDevelopment { get; set; }

        public static string Version
        {
            get
            {
                if (_version == null)
                    _version = Assembly.GetEntryAssembly().GetName().Version.ToString();

                return _version;
            }
        }
        public static string _version;

        public static Dictionary<string, string> Categories
        {
            get
            {
                if (_categories != null && _categories.Count > 0) return _categories;

                if (BusinessFactory.Current == null) return default;
                var productBus = BusinessFactory.Current.GetProductBusiness();

                try
                {
                    _categories = BusinessFactory.Current.GetCategoryBusiness().GetCategoryList();

                }
                catch
                {
                    throw new ApplicationException(
                        "Unable to retrieve categories: Please check and ensure the connection string for is correct.");
                }

                return _categories;
            }
            set => _categories = value;
        }
        private static Dictionary<string, string> _categories;
        private static List<CountryListItem> _countries;
        private static List<StateListItem> _states;


        public static List<CountryListItem> Countries
        {
            get
            {
                if (_countries == null || _countries.Count == 0)
                    LoadLists();

                return _countries;
            }
            set => _countries = value;
        }

        public static List<StateListItem> States
        {
            get
            {
                if (_states == null || _states.Count == 0)
                    LoadLists();

                return _states;
            }
            set => _states = value;
        }

        public static List<FeaturedProduct> FeaturedItems { get; set;  }


        public static DateTime EmptyDate { get; set; }


        static wsApp()
        {
            Configuration = new WebStoreConfiguration();
            Configuration.Initialize();

            Task.Run(() => LoadLists());
        }

        private static void LoadLists(WebStoreContext context = null)
        {
            if (context == null)
                context = WebStoreContext.CreateContext();

            try
            {
                Categories = new CategoryBusiness(context).GetCategoryList();
            }
            catch
            {
                throw new ApplicationException(
                    "Unable to retrieve categories: Please check and ensure the connection string for is correct.");
            }


            var lookupBus = new LookupBusiness(context);
            try
            {
                Countries = lookupBus.GetCountries();
                Countries.Insert(0, new CountryListItem() { Country = $"*** {WebStoreBusinessResources.PleaseSelectACountry}" });

                Countries.InsertRange(1,
                    new[]
                    {
                        new CountryListItem() {CountryCode = "US", Country = "United States"},
                        new CountryListItem() {CountryCode = "CA", Country = "Canada"},
                        new CountryListItem() {CountryCode = "DE", Country = "Germany"},
                        new CountryListItem() {CountryCode = "CH", Country = "Switzerland"},
                        new CountryListItem() {CountryCode = "AU", Country = "Australia"},
                        new CountryListItem() {CountryCode = "GB", Country = "United Kingdom"},
                        new CountryListItem() { Country = "----------------"}
                    });

                Countries.Add(new CountryListItem() { Country = "Other - my country is not listed", CountryCode = "OTHER" });
                States = lookupBus.GetStates();
                States.Insert(0, new StateListItem() { State = $"*** {WebStoreBusinessResources.PleaseSelectAState}" });

            }
            catch
            {
                throw new ApplicationException(
                    "Unable to retrieve Countries and States: Please check and ensure the connection string for is correct.");
            }

            try
            {
                FeaturedItems = lookupBus.GetFeaturedProducts();
            }
            catch
            {
                throw new ApplicationException(
                    "Unable to retrieve Featured Products: " + lookupBus.ErrorMessage);
            }
        }


        public static string NewId()
        {
            return DataUtils.GenerateUniqueId(10);
        }

    }

    public class wsAppConstants
    {
        public DateTime EmptyDate { get; } = TimeUtils.MIN_DATE_VALUE;

        public DateTime AppStartedOn { get; set; }

        public string DefaultConnectionString { get; set; } = "server=.;database=WestwindWebStore; integrated security=true; encrypt=false;";

        public string StartupFolder { get; set; }
        public string WebRootFolder { get; set; }

        public string CreditCardLogPath { get;  } =  "./wwwroot/admin/cclog.txt";

        public string EncKey { get; set; } = "WebStore89#15$212";
    }

}
