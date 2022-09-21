using System;
using System.Collections.Generic;
using Westwind.AspNetCore;
using Westwind.Webstore.Business.Entities;
using Westwind.Webstore.Web.Models;

namespace Westwind.Webstore.Web.Controllers
{

    public class ProductViewModel : WebStoreBaseViewModel
    {
        public Product Item { get; set; }


        public decimal ListPrice { get; set; }

        #region Custom Display
            public decimal YouSave { get; set; }
            public string YouSavePercent { get; set; }
        #endregion

        #region Capture

        public string Sku { get; set;  } 

        public decimal Quantity { get; set; } = 1M;

            public string StringQuantity
            {
                get
                {
                    return Quantity.ToString("N2").Replace(".00", "");
                }
            }

            #endregion

        /// <summary>
        ///  Prepare the model for rendering fixing up custom display values
        /// </summary>
        /// <returns></returns>
        public void Prepare()
        {
            if (Item.Price < Item.ListPrice)
            {
                YouSavePercent = Math.Round((1M - Item.Price / Item.ListPrice) * 100, 0) + "%";
                YouSave = Item.ListPrice - Item.Price;
            }
        }
    }

    public class ProductListViewModel : WebStoreBaseViewModel
    {
        public  List<Product> ItemList { get; set; }
        public string SelectedCategory { get; set; }

        public bool ShowPriceAndPurchaseButtong { get; set; } 
    }



}
