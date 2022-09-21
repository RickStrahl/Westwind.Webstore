using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Westwind.Data.EfCore;
using Westwind.Webstore.Business.Entities;

namespace Westwind.Webstore.Business
{
    public class LookupBusiness : WebStoreBusinessObject<Lookup>

    {
        public LookupBusiness(WebStoreContext context) : base(context)
        { }

        /// <summary>
        /// Returns a list of all countries and codes
        /// </summary>
        /// <returns></returns>
        public List<CountryListItem> GetCountries()
        {
            return Context.Lookups
                .Where(l =>    l.Key == "COUNTRY")
                .OrderBy(l=> l.CData1)
                .Select(c => new CountryListItem() {Country = c.CData1, CountryCode = c.CData})
                .ToList();
        }

        /// <summary>
        /// Returns all the US and Canadian states
        /// </summary>
        /// <returns></returns>
        public List<StateListItem> GetStates()
        {
            return Context.Lookups
                .Where(l => l.Key == "STATE")
                .OrderBy(l=> l.CData1)
                .Select(c => new StateListItem() {State = c.CData1, StateCode = c.CData})
                .ToList();
        }


        /// <summary>
        /// Returns a Promo Code percentage from the look ups table.
        /// if not found returns 0.00.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public decimal GetPromoCodePercentage(string code)
        {
            if (string.IsNullOrEmpty(code)) return 0;

            code = code.ToUpper();

            return Context.Lookups
                .Where(l => l.Key == "PROMO" && l.CData.ToUpper() == code)
                .Select(l => l.NData/100)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns a list of selected items
        /// </summary>
        /// <returns></returns>
        public List<FeaturedProduct> GetFeaturedProducts()
        {
            var list = Context.Products
                .FromSqlRaw(
                    @"SELECT top 100 CData as Sku, Products.Description, Products.ItemImage FROM Lookups, Products
WHERE [Key] = 'FEATUREDSKU' and Lookups.CData = products.sku
ORDER BY Lookups.nData Desc")
                .Select(p => new FeaturedProduct()
                {
                    Sku = p.Sku, Description = p.Description,
                    ItemImage = p.ItemImage
                })
                .ToList();

            return list;
        }
    }

    public class FeaturedProduct
    {
        public string Sku { get; set;  }
        public string Description { get; set; }

        public string ItemImage { get; set; }
    }


    public class CategoryBusiness : EntityFrameworkBusinessObject<WebStoreContext,Category>
    {
        public CategoryBusiness(WebStoreContext context) : base(context)
        {
        }

        public Dictionary<string,string> GetCategoryList()
        {
            var productBusiness = BusinessFactory.Current.GetProductBusiness(Context);

            var list = Context.Categories
                .OrderBy(c => c.CategoryName)
                .Select(c => new KeyValuePair<string, string>(c.CategoryName, c.Description));

            var dict = new Dictionary<string, string>(list);

            var delKeys = new List<string>();
            foreach (var kv in dict)
            {
                if (!productBusiness.HasAnyCategory(kv.Key))
                    delKeys.Add(kv.Key);
            }
            foreach (var key in delKeys)
            {
                dict.Remove(key);
            }

            return dict;
        }
    }



    public class CountryListItem
    {
        public string CountryCode { get; set; }
        public string Country { get; set;  }
    }

    public class StateListItem
    {
        public string StateCode { get; set; }
        public string State { get; set;  }
    }

}

