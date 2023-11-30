using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Westwind.Utilities;
using Westwind.Utilities.Data;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;
using Westwind.Webstore.Business.Test;


namespace Westwind.WebStore.Business.Test
{
    [TestFixture]
    public class CustomerBusinessTests
    {
        private string ConnectionString = wsApp.Configuration.ConnectionString;
        string EMAIL_TOCHECK ="rstrahl@west-wind.com";

        public BusinessFactory BusinessFactory { get; set; }

        [SetUp]
        public void Init()
        {
            // Load Factory and Provider
            BusinessFactory = TestHelpers.GetBusinessFactoryWithProvider();
        }


        [Test]
        public void TestConnection()
        {
            Console.WriteLine(wsApp.Configuration.ConnectionString);
            var sql = new SqlDataAccess(wsApp.Configuration.ConnectionString);
            var table = sql.ExecuteTable("Test","select * from Customers");

            Assert.IsNotNull(table, sql.ErrorMessage);
            Assert.IsTrue(table.Rows.Count > 0, "Table exists but has no records");
        }

        [Test, Order(1)]
        public void CreateCustomers()
        {
            using(var bus = BusinessFactory.GetCustomerBusiness())
            {
                var rickCustomer = new Customer()
                {
                    Firstname = "Rick",
                    Lastname = "Strahl",
                    Company = "West Wind",
                    Email = "rstrahl@west-wind.com",
                    Password = "ultraseekrit",
                };
                bus.Attach(rickCustomer, true);

                var kevinCustomer = new Customer()
                {
                    Firstname = "Kevin",
                    Lastname = "McNeish",
                    Company = "Oak Leaf",
                    Email = "kevin@oakleaf.com",
                    Password = "nukaseekrit",
                    Addresses = new List<Address>
                    {
                        new Address
                        {
                            AddressType = AddressTypes.Billing,
                            StreetAddress = "133 Holaman Place",
                            City = "Kihei",
                            PostalCode = "96799",
                            Country = "United States",
                            CountryCode = "US"
                        }
                    }
                };
                bus.Attach(kevinCustomer, true);

                Assert.IsTrue(bus.SaveChanges() > -1, bus.ErrorMessage);
            }
        }

        [Test, Order(2)]
        public void QueryCustomers()
        {
            var bus = BusinessFactory.GetCustomerBusiness();

            var custList = bus.Context.Customers
                .Select(c => new
                {
                    c.Firstname, c.Lastname, c.Company, City = c.Addresses.FirstOrDefault().City as string
                });

            Assert.NotNull(custList);
            Assert.True(custList.Any(), "Customer List should not be empty");

            Console.WriteLine(custList.Count().ToString());
        }

        [Test, Order(3)]
        public void QueryCustomerById()
        {
            var bus = this.BusinessFactory.GetCustomerBusiness();


            var id = "CD3BA538-3AAA-41E2-A6BD-9C59ADAC7CF4";

            var c = bus.Context.Customers.FirstOrDefault(c => c.Id == id);


            var c2 = bus.Load(id);
            c2 = bus.Load(id);

            var cust = bus.Context.Customers.FirstOrDefault(c => c.OldPk == 891);
            Assert.IsNotNull(cust);
            Console.WriteLine(cust.Email);
        }

        [Test]
        public void DeleteAllCustomers()
        {
            var bus = BusinessFactory.GetCustomerBusiness();

            Assert.IsTrue(bus.DeleteWhereDirect("1=1") > -1, bus.ErrorMessage);
        }



        [Test]
        public void LoadByIdTest()
        {
            using (var bus = BusinessFactory.GetCustomerBusiness())
            {

                var id = bus.Context.Customers.Select(c => c.Id).FirstOrDefault();


                Console.WriteLine(id);

                if (string.IsNullOrEmpty(id))
                {
                    Assert.Fail("There are no customers that can be loaded.");
                }

                var cust = bus.Load(id);
                Console.WriteLine(cust.Company + " - " + cust.Addresses.FirstOrDefault()?.City);
            }
        }

        [Test]
        public void AddItem()
        {
            using (var bus = BusinessFactory.GetProductBusiness())
            {
                var item = new Product
                {
                    Sku = "NewItem2",
                    Description = "New Item",
                    Price = 199
                };

                bus.Create(item);
                bus.Attach(item, true);

                Assert.IsTrue(bus.SaveChanges() > -1, bus.ErrorMessage);
            }
        }


        [Test]
        public void UpdateItem()
        {


            using (var bus = BusinessFactory.GetProductBusiness())
            {
                var item = bus.Context.Products
                    .FirstOrDefault(i => i.Sku == "wconnect60");

                item.Expected = DateTime.Now;


                Assert.IsTrue(bus.SaveChanges() > -1,bus.ErrorMessage);

                // reload

                item = bus.Context.Products
                    .FirstOrDefault(i => i.Sku == "wconnect60");

                Console.WriteLine(item.Description);
            }
        }



        [Test]
        public void QueryItemsBySkuTest()
        {
            var sku = "WCONNECT";

            using (var bus = BusinessFactory.GetProductBusiness())
            {
                var items = bus.Context.Products
                    .Where(i => i.Sku.Contains(sku, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(i => i.Sku);

                Assert.IsTrue(items.Any());

                foreach (var item in items)
                {
                    Console.WriteLine($"{item.Sku}");
                }

                Console.WriteLine(items.Count());

            }
        }


        [Test]
        public void QueryInvoicesWithSkuTest()
        {
            var sku = "WCONNECT60";

            using (var bus = BusinessFactory.GetCustomerBusiness())
            {

                var invoices = bus.Context.Invoices
                    //.Where(i => i.Items.Any(i => i.Sku.Contains(sku, StringComparison.OrdinalIgnoreCase))) // Unsupported Operation

                    .Where(i => i.LineItems.Any(i => i.Sku == sku))  // works
                    .OrderBy(i => i.InvoiceDate);

                Assert.IsTrue(invoices.Any());

                foreach (var invoice in invoices)
                {
                    Console.WriteLine(
                        $"{invoice.InvoiceDate:MMM dd, yyyy} {invoice.InvoiceTotal} Items: {invoice.LineItems.Count}");
                }

                Console.WriteLine(invoices.Count());

            }


        }

        [Test]
        public void ValidateEmailAddress()
        {


            var bus = BusinessFactory.GetCustomerBusiness();

            var cust = bus.GetCustomerByEmailAddress(EMAIL_TOCHECK);
            Assert.IsNotNull(cust, "Couldn't load sample customer");

            var id = cust.Id;
            var isNew = false;


            // Should be valid (no Validation Errors) for existing customer non-new
            var res = bus.ValidateEmailAddress(isNew, cust);
            Assert.IsTrue(res, "There shouldn't be validation errors: " + bus.ValidationErrors.ToString());


            // should be invalid: Existing email but different id
            cust.Id = wsApp.NewId();
            res = bus.ValidateEmailAddress(isNew, cust);
            Assert.IsFalse(res, "Should cause a validation error when email exists but not with current id");
            Console.WriteLine(bus.ValidationErrors);

            bus.ValidationErrors.Clear();

            // New Email Address - should not cause failure
            isNew = true;
            cust.Email = "newaddress@home.com";
            res = bus.ValidateEmailAddress(isNew, cust);
            Assert.IsTrue(res, "There shouldn't be validation errors: " + bus.ValidationErrors.ToString());

            // New Email Address that exists - should cause failure
            isNew = true;
            cust.Email = EMAIL_TOCHECK;
            res = bus.ValidateEmailAddress(isNew, cust);
            Assert.IsFalse(res, "New Email that exists should cause an error");
            Console.WriteLine(bus.ValidationErrors);
        }


        [Test]
        public void UpdateCustomerTest()
        {
            Customer cust = null;
            string nameUpdate;
            using (var bus = BusinessFactory.GetCustomerBusiness())
            {
                cust = bus.GetCustomerByEmailAddress(EMAIL_TOCHECK);

                Assert.That(cust, Is.Not.Null);

                nameUpdate = "Rick " + DateTime.Now;
                cust.Firstname = nameUpdate;

                Assert.That( bus.SaveChanges(), Is.GreaterThan(-1));
            }

            // reload everything
            using (var bus = BusinessFactory.GetCustomerBusiness())
            {
                cust = bus.GetCustomerByEmailAddress(EMAIL_TOCHECK);
            }
            
            Assert.That(nameUpdate, Is.EqualTo(cust.Firstname));
        }

        [Test]
        public void UpdateCustomerRawTest()
        {
            var nameUpdate = "Rick " + DateTime.Now;

            using (var bus = BusinessFactory.GetCustomerBusiness())
            {

                var cust = bus.Context.Customers.FirstOrDefault(c => c.Email == EMAIL_TOCHECK);
                cust.Firstname = nameUpdate;
                bus.SaveChanges();

            }
            using (var bus = BusinessFactory.GetCustomerBusiness()) {
                var cust = bus.Context.Customers.FirstOrDefault(c => c.Email == EMAIL_TOCHECK);
                Assert.That(nameUpdate, Is.EqualTo(cust.Firstname));
            }
        }

        [Test]
        public void UpdateCustomerBusObjectTest()
        {

            var newValue = "Werx " + DateTime.Now.ToString();

            using (var bus = BusinessFactory.GetCustomerBusiness())
            {

                var cust = bus.Load("96nr7t67ce");
                cust.Company = newValue;
                Assert.IsTrue(bus.Save(), bus.ErrorMessage);
            }
            using (var bus = BusinessFactory.GetCustomerBusiness())
            {
                var cust = bus.Load("96nr7t67ce");
                var entry = bus.Context.Entry(cust);
                Console.WriteLine(entry.State);

                Assert.That(newValue, Is.EqualTo(cust.Company));
                Console.WriteLine(cust.Company);
            }
        }

        //[Test]
        //public void AutoMapperMapTest()
        //{
        //    var bus = BusinessFactory.GetCustomerBusiness();
        //    var cust = bus.GetCustomerByEmailAddress(EMAIL_TOCHECK);
        //    Assert.IsNotNull(cust, "Couldn't load sample customer");

        //    var cust2 = bus.GetCustomerByEmailAddress("louis.fischer@uhc.com");
        //    Assert.IsNotNull(cust2, "Couldn't load sample customer 2");

        //    var mapper = ApplicationMapper.Current;
        //    mapper.Map(cust, cust2);

        //    var json = JsonSerializationUtils.Serialize(cust2, false, true);
        //    Console.WriteLine(json);

        //    json = JsonSerializationUtils.Serialize(cust, false, true);
        //    Console.WriteLine(json);
        //}

    }
}
