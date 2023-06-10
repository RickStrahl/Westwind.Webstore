using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Westwind.Webstore.Business.Entities
{
    /// <summary>
    /// A list of categories.
    /// </summary>
    [Table("Categories")]
    public class Category
    {
        [Key()]
        [StringLength(20)]
        public string Id { get; set; } = wsApp.NewId();

        [StringLength(20)]
        public string ParentId { get; set; }

        public string CategoryName { get; set; }
        public string Description { get; set;  }
        public string Type { get; set;  }
        public string Keywords { get; set; }
    }
}
