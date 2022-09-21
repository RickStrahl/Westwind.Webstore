using System;
using System.Collections.Generic;
using System.Linq;
using Westwind.Webstore.Business.Entities;

namespace Westwind.Webstore.Business
{
    public class ProductBusiness : WebStoreBusinessObject<Product>
    {
        public ProductBusiness(WebStoreContext context) : base(context)
        {

        }

        protected override bool OnBeforeSave(Product item)
        {
            item.Sku = item.Sku.ToLower();
            return true;
        }

        /// <summary>
        /// Retrieves an inventory Item by sku
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        public Product LoadBySku(string sku)
        {
            if (string.IsNullOrEmpty(sku))
                return null;

            sku = sku.ToLower();
            var item = LoadBase(i => i.Sku == sku);
            return item;
        }



        /// <summary>
        /// Returns items for the Item display list
        /// </summary>
        public List<Product> GetItems(InventoryItemsFilter filter = null, bool showInactiveItems = false)
        {
            if (filter == null)
                filter = new InventoryItemsFilter();

            IEnumerable<Product> list = null;
            list = Context.Products;

            if (!showInactiveItems)
                list = list.Where(i => !i.InActive);

            if (filter.ListOrder == InventoryListOrder.Specials)
            {
                list = list.Where(i => i.SpecialsOrder > 0);
            }

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                list = list.Where(item =>
                    item.Sku.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    item.Description.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(filter.Category))
            {
                list = list.Where(item =>
                    item.Categories != null &&
                    item.Categories.Contains(filter.Category, StringComparison.OrdinalIgnoreCase));
            }

            IEnumerable<Product> result = null;
            if (filter.ListOrder == InventoryListOrder.Default)

                result = list
                    .OrderByDescending(item => item.SortOrder)
                    .ThenBy(item => item.Description.ToLower());

            else if (filter.ListOrder == InventoryListOrder.Description)
                result = list.OrderBy(item => item.Description);
            else if (filter.ListOrder == InventoryListOrder.Sku)
                result = list.OrderBy(item => item.Sku);
            else if (filter.ListOrder == InventoryListOrder.Price)
                result = list.OrderBy(item => item.Price);
            else if (filter.ListOrder == InventoryListOrder.Date)
                result = list.OrderBy(item => item.ProductDate);
            else if (filter.ListOrder == InventoryListOrder.Specials)
                result = list.OrderByDescending(item => item.SpecialsOrder);

            var resultList = result.ToList();

            return resultList;
        }

        /// <summary>
        /// Checks to see if at least one product has a given category
        /// </summary>
        /// <param name="kvKey"></param>
        /// <returns></returns>
        public bool HasAnyCategory(string kvKey)
        {
            if (string.IsNullOrEmpty(kvKey)) return false;
            return Context.Products.Any(p =>
                                            !string.IsNullOrEmpty(p.Categories) &&
                                            p.Categories.Contains(kvKey) &&
                                            !p.InActive);
        }

        public bool HasCategory(string kvKey, Product product = null)
        {
            if (product == null)
                product = Entity;
            if (product == null || string.IsNullOrEmpty(product.Categories))
                return false;

            return product.Categories.Contains(kvKey);
        }

    }


    public class InventoryItemsFilter
    {
        public string Category { get; set; }

        public string SearchTerm { get; set; }

        public InventoryListOrder ListOrder = InventoryListOrder.Default;
    }

    public enum InventoryListOrder
    {
        Default,
        Description,
        Price,
        Sku,
        Date,
        Specials
    }
}

