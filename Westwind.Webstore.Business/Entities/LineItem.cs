using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Westwind.Utilities;

namespace Westwind.Webstore.Business.Entities
{

    [Table("LineItems")]
    public class LineItem
    {
        public LineItem()
        {
        }

        /// <summary>
        /// A small item id
        /// </summary>
        [Key()]
        [StringLength(20)]
        public string Id { get; set; } = wsApp.NewId();

        /// <summary>
        /// The parent order id that this item belongs to
        /// </summary>
        [StringLength(20)]
        public string InvoiceId {get; set;}

        /// <summary>
        /// Id of the customer of this order. Provided for easier search access
        /// </summary>
        [StringLength(20)]
        public string CustomerId {get; set;}


        /// <summary>
        /// The human readable product ID used to identify the item
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Single line description of the item
        /// </summary>
        public string Description { get; set; }

        public string ExtraData { get; set; }

        /// <summary>
        /// Quantity ordered
        /// </summary>
        [Column(TypeName ="decimal(18,4)")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Actual order item price - discount is applied after
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }

        /// <summary>
        /// Discount percentage expressed as a fractional decimal value (0.1 = 10%)
        /// that is applied to the price/item total
        /// </summary>
        [Column(TypeName ="decimal(18,4)")]
        public decimal DiscountPercent { get; set; }

        /// <summary>
        /// Total for this item
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal ItemTotal => CalculateItemTotal();


        /// <summary>
        /// Determines whether this item can automatically be registered
        /// after the order has completed
        /// </summary>
        public bool AutoRegister { get; set; }

        /// <summary>
        /// item image name
        /// </summary>
        public string ItemImage
        {
            get
            {
                return _itemImage ?? (Sku?.ToLower() + ".png");
            }
            set { _itemImage = value; }
        }
        private string _itemImage;

        /// <summary>
        /// Set if this item uses licensing for registration
        /// </summary>
        public bool UseLicensing { get; set;  }

        /// <summary>
        /// The license serial number
        /// </summary>
        public string LicenseSerial { get; set;  }


        /// <summary>
        /// An optional license Expiration Date
        /// </summary>
        public DateTime SubscriptionExpires { get; set; } = wsApp.Constants.EmptyDate;

        /// <summary>
        /// An optional auto-renewal for a subscription at current prices
        /// </summary>
        public bool SubscriptionAutoRenewal { get; set;  }


        /// <summary>
        /// Last update time (actually creation time) to allow sorting
        /// </summary>
        public DateTime Updated { get; set; } = DateTime.Now;

        /// <summary>
        /// Calculates the line item total and returns the value
        /// </summary>
        /// <returns></returns>
        public decimal CalculateItemTotal()
        {
            var itemTotal = Quantity * Price;
            if (DiscountPercent > 0)
            {
                if (DiscountPercent > 100)
                    DiscountPercent = 100;

                if (DiscountPercent > 1)
                    DiscountPercent = DiscountPercent / 100;

                itemTotal *= (1 - DiscountPercent);
            }

            return itemTotal;
        }

        public override string ToString()
        {
            return $"{Sku} - {Quantity} - {Price}";
        }
    }
}
