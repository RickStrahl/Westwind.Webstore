using System;
using NUnit.Framework;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Test;

namespace Westwind.WebStore.Business.Test
{
    [TestFixture]
    public class ProductBusinessTests
    {

        public BusinessFactory BusinessFactory { get; set; }

        [SetUp]
        public void Init()
        {
            // Load Factory and Provider
            BusinessFactory = TestHelpers.GetBusinessFactoryWithProvider();
        }


        [Test]
        public void LoadBySkuTest()
        {
            using (var bus = BusinessFactory.GetProductBusiness())
            {
                var item = bus.LoadBySku("wconnect");

                Assert.That(item, Is.Not.Null);
                Assert.That(item.Sku, Is.EqualTo("wconnect"));
                //
                //
                // ClassicAssert.IsNotNull(item);
                // ClassicAssert.AreEqual(item.Sku, "wconnect");
            }
        }

        [Test]
        public void GetAllInventoryItemsTest()
        {
            using (var bus = BusinessFactory.GetProductBusiness())
            {
                var list = bus.GetItems();

                Assert.IsNotNull(list);
                Assert.IsTrue(list.Count > 0);

                Console.WriteLine(list.Count);
            }
        }

        [Test]
        public void GetFilteredCategoryInventoryItemsTest()
        {
            using (var bus = BusinessFactory.GetProductBusiness())
            {
                var list = bus.GetItems(new InventoryItemsFilter() {Category = "Modeling Tools"});
                Assert.IsNotNull(list);
                Assert.IsTrue(list.Count > 0);
                Console.WriteLine(list.Count);
            }
        }

        [Test]
        public void GetFilteredSearchTermInventoryItemsTest()
        {
            using (var bus = BusinessFactory.GetProductBusiness())

            {

                var list = bus.GetItems(new InventoryItemsFilter() {SearchTerm = "Markdown Monster"});

                Assert.IsNotNull(list);
                Assert.IsTrue(list.Count > 0);

                Console.WriteLine(list.Count);
            }
        }


    }
}

