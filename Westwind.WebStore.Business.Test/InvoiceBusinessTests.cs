using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;
using Westwind.Webstore.Business.Test;

namespace Westwind.WebStore.Business.Test
{
     public class InvoiceBusinessTests
    {
        public BusinessFactory BusinessFactory { get; set; }

        [SetUp]
        public void Init()
        {
            // Load Factory and Provider
            BusinessFactory = TestHelpers.GetBusinessFactoryWithProvider();
        }

        [Test]
        public async Task CreateTemporaryInvoiceAndAddItemsAndDeleteToInvoice()
        {
            //MartenBusinessStartupStateSettings.EnableLegacyTimestampBehavior = false;

            Invoice loadedInvoices;
            using (var busInvoice = BusinessFactory.GetInvoiceBusiness())
            {
                Invoice invoice;
                invoice = busInvoice.Create();

                invoice.IsTemporary = true;
                var id = invoice.Id;

                busInvoice.AddLineItem("WCONNECT50",1);
                busInvoice.CalculateTotals();

                Assert.True(await busInvoice.SaveChangesAsync() > -1, busInvoice.ErrorMessage);

                var loadedInvoice = busInvoice.Load(id);
                Assert.IsNotNull(loadedInvoice,  busInvoice.ErrorMessage);

                Assert.IsTrue(busInvoice.Delete(loadedInvoice, saveChanges: true), busInvoice.ErrorMessage);


                loadedInvoices = busInvoice.Load(id);
                Assert.IsNull(loadedInvoices,"InvoiceModel should be loadable any more after deleting");
            }
        }

        [Test]
        public async Task CreateInvoiceEfTest()
        {
            using (var busInvoice = BusinessFactory.GetInvoiceBusiness())
            using (var busCustomer = BusinessFactory.GetCustomerBusiness(busInvoice.Context))
            {
                var invoice = busInvoice.Create();

                invoice.InvoiceDate = DateTime.UtcNow;

                busInvoice.AddLineItem("MARKDOWN_MONSTER_2", 1M);

                var customer = busCustomer.Load("sfasifghb7");
                invoice.BillingAddress = CustomerBusiness.GetBillingAddress(customer);

                invoice.CreditCardResult.ProcessingResult = "APPROVED";
                invoice.CreditCardResult.TransactionId = "12345";

                invoice.Customer = customer;
                invoice.CustomerId = customer.Id;

                await busInvoice.SaveAsync();
            }
        }


    }
}
