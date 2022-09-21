using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Westwind.Webstore.Business.Entities
{

    /// <summary>
    /// Lookup item used from a key list of stored in the database
    /// </summary>

    
    public class Lookup
    {
        [Key()]
        [StringLength(20)]
        public string Id { get; set; } = wsApp.NewId();

        public string Type { get; set;  }

        /// <summary>
        /// Group identitifier for lookups. Each set of items
        /// has a shared key (ie. "promo", 
        /// </summary>
        public string Key { get; set; }
        
        public string CData { get; set;  }
        public string CData1 { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal NData { get; set; }

        public override string ToString()
        {
            return $"{Type} - {Key}: {CData} -  {NData}";
        }
    }

}
