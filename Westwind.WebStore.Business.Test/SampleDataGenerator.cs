#if false
using System.Collections.Generic;
using Marten;
using Westwind.Webstore.Business.Entities;

namespace MartenXUnitTests.Models
{
    public class SampleDataGenerator
    {
        public DocumentStore Store { get; }

        public SampleDataGenerator(DocumentStore store)
        {
            Store = store;
        }

        public void GenerateData()
        {
            Customer rickCustomer;
            Customer kevinCustomer;

            using (var session = Store.LightweightSession())
            {

                rickCustomer = new Customer()
                {
                    Firstname = "Rick",
                    Lastname = "Strahl",
                    Company = "West Wind",
                    Email = "rstrahl@west-wind.com",
                    Password = "ultraseekrit",
                    Addresses = new List<Address>
                    {
                        new Address
                        {
                            AddressName = "Primary",
                            StreetAddress = "32 Kaiea Place",
                            City = "Paia",
                            PostalCode = "96779",
                            Country = "United States",
                            CountryCode = "US"
                        }
                    }

                };
                session.Store(rickCustomer);

                kevinCustomer = new Customer()
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
                            AddressName = "Primary",
                            StreetAddress = "133 Holaman Place",
                            City = "Kihei",
                            PostalCode = "96799",
                            Country = "United States",
                            CountryCode = "US"
                        }
                    }
                };
                session.Store(kevinCustomer);
                
                session.SaveChanges();
            }


            using (var session = Store.LightweightSession())
            {
                var order = new Order
                {
                    Customer = rickCustomer,
                    OrderNumber = "13123",
                };

                var item = new OrderItem
                {
                    Sku = "wconnect",
                    Description = "Web Connection",
                    Quantity = 1,
                    Price = 399.00M,
                    CustomerId = rickCustomer.Id,

                };
                order.Items.Add(item);

                item = new OrderItem
                {
                    Sku = "markdown_monster",
                    Description = "Markdown Monster",
                    Quantity = 2,
                    Price = 49M,
                    Discount = 0.15M,
                    CustomerId = rickCustomer.Id
                };
                order.Items.Add(item);
                order.CalculateTotals();

                session.Store<Order>(order);

                session.SaveChanges();

                order = new Order
                {
                    Customer = kevinCustomer,
                    OrderNumber = "154123",
                };

                order.Items.AddRange(new[]
                {
                    new OrderItem
                    {
                        Sku = "helpbuilder",
                        Description = "Help Builder",
                        Quantity = 1,
                        Price = 399.00M,
                        CustomerId = kevinCustomer.Id,
                        OrderId = order.Id

                    },
                    new OrderItem
                    {
                        Sku = "markdown_monster",
                        Description = "Markdown Monster",
                        Quantity = 1,
                        Price = 49M,
                        Discount = 0.10M,
                        CustomerId = kevinCustomer.Id,
                        OrderId = order.Id
                    }
                });
                order.CalculateTotals();
                session.Store<Order>(order);


                session.SaveChanges();
            }






        }

    }
}
#endif