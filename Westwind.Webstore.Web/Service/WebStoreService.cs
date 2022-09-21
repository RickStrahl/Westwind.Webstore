using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Westwind.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Westwind.Webstore.Business;

namespace Westwind.Webstore.Web.Service
{

    public class WebStoreService : BaseApiController
    {

        [HttpGet]
        [Route("service/products")]
        public IEnumerable<ProductResult> GetProducts(string searchTerm)
        {
            var productBus = BusinessFactory.Current.GetProductBusiness();
            var items = productBus.GetItems(new InventoryItemsFilter { SearchTerm = searchTerm });
            return items.Select( p=> new ProductResult
            {
                Sku = p.Sku,
                Description = p.Description,
                LongDescription = p.LongDescription,
                Price = p.Price,
                IsStockItem = p.IsStockItem,
                Stock = p.Stock
            });
        }

    }

    [DebuggerDisplay("{Sku} - {Description}")]
    public class ProductResult
    {
        public string Sku { get; set; }
        public string Description { get; set;  }
        public string LongDescription { get; set; }
        public decimal Price { get; set; }
        public bool IsStockItem { get; set; }
        public decimal Stock { get; set; }

    }
}
