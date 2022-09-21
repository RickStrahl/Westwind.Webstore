using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Westwind.Utilities;
using Westwind.Utilities.Data;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;
using Westwind.WebStore.Business.Test;


namespace Westwind.Webstore.Business.Test
{
    [TestFixture]
    public class LookupBusinessTests
    {
        private string ConnectionString = wsApp.Configuration.ConnectionString;

        public BusinessFactory BusinessFactory { get; set; }

        [SetUp]
        public void Init()
        {
            // Load Factory and Provider
            BusinessFactory = TestHelpers.GetBusinessFactoryWithProvider();
        }


        [Test]
        public void GetCountriesTest()
        {
            var busLookups = BusinessFactory.GetLookupBusiness();

            var countries = busLookups.GetCountries();

            Assert.IsTrue(countries.Count > 0, "No lookups returned: " + busLookups.ErrorMessage);
        }

        [Test]
        public void GetStatesTest()
        {
            var busLookups = BusinessFactory.GetLookupBusiness();

            var states = busLookups.GetStates();

            Assert.IsTrue(states.Count > 0, "No lookups returned: " + busLookups.ErrorMessage);
        }

        [Test]
        public void GetFeaturedItemsTest()
        {
            var busLookups = BusinessFactory.GetLookupBusiness();

            var lookups = busLookups.GetFeaturedProducts();

            Assert.IsTrue(lookups.Count > 0, "No lookups returned: " + busLookups.ErrorMessage);
        }




    }
}
