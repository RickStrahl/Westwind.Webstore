using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Westwind.Webstore.Business.Entities
{
    [Table("Products")]
    public class Product
    {
        [Key()]
        [StringLength(20)]
        public string Id { get; set; } = wsApp.NewId();


        /// <summary>
        /// Item identifier
        /// </summary>
        [StringLength(100)]
        public string Sku { get; set; }

        /// <summary>
        /// Parent Sku can be used on child items to signify specializations
        /// like:
        ///
        /// * sizes
        /// * colors
        /// * styles
        ///
        /// The <seealso cref="Sku"/> can use common postfixes like _LARGE or _BLUE etc.
        /// </summary>
        public string ParentSku { get; set; }

        /// <summary>
        /// Single line description of the item
        /// </summary>
        //[DuplicateField(PgType = "text")]
        public string Description { get; set; }

        /// <summary>
        /// An item Abstract
        /// </summary>
        public string Abstract { get; set; }

        /// <summary>
        /// Detailed description of this item
        /// </summary>
        public string LongDescription { get; set; }

        /// <summary>
        /// Comma delimited list of categories for this item
        /// </summary>
        public string Categories { get; set;  }


        /// <summary>
        /// Optional Manufacturer
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Optional Version number for this product
        /// </summary>
        public string Version { get; set;  }

        /// <summary>
        /// Date when either the item was entered or the manufacturer
        /// release date for the product
        /// </summary>
        public DateTime? ProductDate { get; set; }

        /// <summary>
        /// URL to a product or information page with more info on the
        /// product. Can point to same site or external.
        /// </summary>
        public string InfoUrl { get; set; }

        /// <summary>
        /// Image file name used from ItemImages folder.
        /// </summary>
        public string ItemImage { get; set; }

        /// <summary>
        /// If set redirects this item to the specified URL rather than
        /// displaying the item as normal.
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// If true won't show in listings or searches
        /// </summary>
        public bool InActive { get; set; }

        /// <summary>
        /// Prioritizes items to be sorted to the top in
        /// list queries.
        /// </summary>
        public int SortOrder {get; set; }

        #region Price and Stock

        /// <summary>
        /// Actual price used for this item
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }

        /// <summary>
        /// Official list price for this item which may be higher than the Price
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal ListPrice { get; set; }

        /// <summary>
        /// Cost of the item
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal Cost { get; set; }


        /// <summary>
        /// Determines whether the item is a physical or virtual item (software)
        /// that can be digitally shipped
        /// </summary>
        public bool IsStockItem { get; set;  }

        /// <summary>
        /// Number of items that are available for physical items
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal Stock { get; set; }


        /// <summary>
        /// Weight of the item if physical. Can be used to calculate
        /// shipping.
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal Weight { get; set; }


        /// <summary>
        /// Number of items that are on order/expected. Use in
        /// combination with <seealso cref="Expected"/>
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal OnOrder { get; set; }

        /// <summary>
        /// Date when the item is expected to be available again.
        /// Use in combination with <seealso cref="OnOrder"/>.
        /// </summary>
        public DateTime? Expected { get; set;  }

        /// <summary>
        /// Determines if the item allows for fractional values
        /// </summary>
        public bool IsFractional { get; set; }




        #endregion


        #region Item Commissions


        /// <summary>
        /// Commission percentage
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal Commission {get; set;  }

        /// <summary>
        /// Base prices for commission if other than the full price.
        /// If this value is 0 the base price is the <seealso cref="Product.Price"/>
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal CommissionBasePrice {get; set; }

        /// <summary>
        /// Email address to send when item is sold on commission
        /// </summary>
        public string EmailTo { get; set; }


        #endregion


        #region Item Registration


        /// <summary>
        /// Confirmation text for item.
        /// </summary>
        public string RegistrationItemConfirmation { get; set; }

        /// <summary>
        /// Registration Password or License Key
        /// </summary>
        public string RegistrationPassword { get; set; }

        /// <summary>
        /// If true allows item to be auto confirmed when order is
        /// completed.
        /// </summary>
        public bool AutoRegister { get; set; }

        /// <summary>
        /// Provide optional comma delimited email addresses to vendors
        /// when an order is confirmed as email CC list.
        /// </summary>
        public string VendorEmail { get; set; }

        #endregion




        #region Item Specials


        /// <summary>
        /// Special text if the item is set as <seealso cref="Special"/>
        /// </summary>
        public string SpecialsText { get; set; }

        /// <summary>
        /// Special header if the item is set as <seealso cref="Special"/>
        /// </summary>
        public string SpecialsHeader { get; set; }

        /// <summary>
        /// Special price if item is set as <seealso cref="Special"/>
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal SpecialsPrice { get; set;  }


        /// <summary>
        /// If > 0  this item shows up on the specials page and the
        /// number represents the sort order of the specials in
        /// descending order.
        /// </summary>
        public int SpecialsOrder { get; set; }

        #endregion

        #region Licensing

        /// <summary>
        /// Determines whether this item uses licensing
        /// </summary>
        public bool UseLicensing { get; set;  }


        /// <summary>
        /// Number of licenses that are applicable for this product (ie. 1 for single, 5 for 5-user license etc.)
        /// </summary>
        public int LicenseCount { get; set; }

        #endregion

        #region Subscription

        /// <summary>
        /// Duration of the Subscription in months
        /// </summary>
        public int SubscriptionRenewalMonths { get; set; } = 0;

        /// <summary>
        /// A discount percentage between 0-100% when the subscription renews
        /// </summary>
        public int SubscriptionRenewalDiscountPercent { get; set;  }

        /// <summary>
        /// Text to use for renewal request emails to users
        /// </summary>
        public string SubscriptionRenewalRequestText { get; set; }

        #endregion


        #region Properties and State



        /// <summary>
        /// Generic properties that can be assigned to this item
        /// </summary>
        //public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public int OldPk { get; set; }

        /// <summary>
        /// An optional type that can be assigned to an item for
        /// grouping beyond categories.
        /// </summary>
        public string Type { get; set; }


        public override string ToString()
        {
            return $"{Sku} - {Description}";
        }
        #endregion

    }

}
