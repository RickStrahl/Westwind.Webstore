using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using Westwind.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Westwind.AspNetCore.Errors;
using Westwind.WebStore.App;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;

namespace Westwind.Webstore.Web.Service
{

    /// <summary>
    /// Open access product API of Westwind Products
    /// </summary>
    public class ProductsApiController : BaseApiController
    {
        public BusinessFactory BusinessFactory { get; }

        public ProductsApiController(BusinessFactory factory)
        {
            BusinessFactory = factory;
        }

        [HttpGet]
        [Route("/api/products")]
        public IEnumerable<ProductListApiModel> GetProductLists(string searchTerm)
        {
            var productBus = BusinessFactory.GetProductBusiness();
            var items = productBus.GetItems(new InventoryItemsFilter { SearchTerm = searchTerm });
            return items.Select( p=> new ProductListApiModel
            {
                Sku = p.Sku,
                Description = p.Description,
                LongDescription = p.LongDescription,
                Price = p.Price,
                IsStockItem = p.IsStockItem,
                Stock = p.Stock
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("/api/products/{sku}")]
        public ProductApiModel GetProduct(string sku)
        {
            var productBus = BusinessFactory.GetProductBusiness();
            var product = productBus.LoadBySku(sku);
            if (product == null)
            {
                throw new ApiException(productBus.ErrorException, 500);
            }
            var productModel = ApplicationMapper.Current.Map<Product, ProductApiModel>(product);
            return productModel;
        }

    }

    public class ProductListApiModel
    {
        public string Sku { get; set;  }
        public string Description  { get; set;  }
        public string LongDescription { get; set;  }
        public decimal Price  {get; set;  }
        public bool IsStockItem { get; set;  }
        public decimal Stock { get; set;  }


    }

    public class ProductApiModel
    {
        public string Id { get; set; }
        public string Sku { get; set; }
        public string ParentSku { get; set; }

        public string Abstract { get; set; }
        public string Description { get; set; }

        public string LongDescription { get; set; }

        public string Categories { get; set;  }

        public bool IsStockItem { get; set;  }

        public string Manufacturer { get; set; }

        public string Version { get; set;  }

        public DateTime? ProductDate { get; set; }

        public string InfoUrl { get; set; }

        public string ItemImage { get; set; }

        public string RedirectUrl { get; set; }

        public bool InActive { get; set; }

        public int SortOrder {get; set; }

        public decimal Price { get; set; }

        public decimal ListPrice { get; set; }

        public decimal Weight { get; set; }

        public decimal Stock { get; set; }

        public decimal OnOrder { get; set; }

        public DateTime? Expected { get; set;  }

        public bool IsFractional { get; set; }


        public bool UseLicensing { get; set;  }

        public string Type { get; set; }

        public override string ToString()
        {
            return $"{Sku} - {Description}";
        }

    }
}
