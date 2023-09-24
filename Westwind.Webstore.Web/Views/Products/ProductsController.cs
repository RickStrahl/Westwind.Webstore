using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc;
using Westwind.AspNetCore;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;
using Westwind.Webstore.Web.Models;

namespace Westwind.Webstore.Web.Controllers
{
    public class ProductsController : WebStoreBaseController
    {
        public BusinessFactory BusinessFactory { get; }

        public ProductsController(BusinessFactory businessFactory)
        {
            BusinessFactory = businessFactory;
        }


        [HttpGet, Route("products"),  Route("products/{category}")]
        public IActionResult Products(string category, string s, string order)
        {
            var model = CreateViewModel<ProductListViewModel>();

            var filter =
                new InventoryItemsFilter()
                {
                    Category = category,
                    SearchTerm = s
                };

            // Check if the category is valid and assign message to error model
            if (!string.IsNullOrEmpty(filter.Category))
            {
                var lookupCategory =wsApp.Categories.FirstOrDefault(c => c.Key == filter.Category);
                filter.Category = lookupCategory.Key as string;

                if (string.IsNullOrEmpty(filter.Category))
                {
                    model.ErrorDisplay.Message = $"<b>{category}</b> is an <b>invalid category</b>. Please use a valid category.";
                    model.ErrorDisplay.MessageAsRawHtml = true;
                    //model.ErrorDisplay.Icon = "warning";
                    model.ItemList = new List<Product>();
                    return View("Products", model);
                }

                category = filter.Category;
            }


            List<Product> itemList;
            using (var busItem = BusinessFactory.GetProductBusiness())
            {
                itemList = busItem.GetItems(filter);
            }

            model.ItemList = itemList;
            model.SelectedCategory= category;

            return View("Products", model);
        }



        [HttpGet, Route("product/{sku}/{quantity?}")]
        public IActionResult Product(string sku, decimal quantity)
        {
            var model = this.CreateViewModel<ProductViewModel>();

            model.Quantity = quantity;
            if (model.Quantity == 0)
                model.Quantity = 1;

            model.Sku = sku;

            using (var busItem = BusinessFactory.GetProductBusiness())
            {
                model.Item = busItem.LoadBySku(sku.ToLower());
                if (model.Item == null)
                {
                    return Products(null, null, null);
                }
                if (!string.IsNullOrEmpty(model.Item.RedirectUrl))
                {
                    return RedirectPermanent(model.Item.RedirectUrl);
                }
            }


            model.Prepare();
            return View("Product",model);
        }



        [HttpGet, Route("api/product/search/{searchText}")]
        public IActionResult ApiProductSearch(string searchText)
        {
            List<object> list = new List<object>();
            using (var busItem = BusinessFactory.GetProductBusiness())
            {
                var filter = new InventoryItemsFilter() { SearchTerm = searchText };
                var matches = busItem.GetItems(filter)
                    .Select(p => new { id = p.Sku, name = p.Description });

                foreach(var prod in matches)
                    list.Add(prod);
            }

            return Json(list);
        }

        [HttpGet, Route("api/product/searchall/{searchText}")]
        public IActionResult ApiProductSearchAll(string searchText)
        {
            List<object> list = new List<object>();
            using (var busItem = BusinessFactory.GetProductBusiness())
            {
                var filter = new InventoryItemsFilter() { SearchTerm = searchText };
                var matches = busItem.GetItems(filter, true)
                    .Select(p => new { id = p.Sku, name = p.Description, inactive = p.InActive })
                    .OrderBy(p => p.inactive)
                    .ThenBy(p => p.name?.ToLower());

                foreach(var prod in matches.Where(m=> !m.inactive))
                    list.Add(prod);
                foreach (var prod in matches.Where(m => m.inactive))

                    list.Add(new { name = prod.name + "*", prod.inactive, prod.id});

            }

            return Json(list);
        }

        public IActionResult AddToShoppingCart()
        {
            return null;
        }
    }
}


