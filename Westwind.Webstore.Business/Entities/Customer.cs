using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
using System.Diagnostics;
using Braintree;
using Newtonsoft.Json;
using Westwind.Utilities;

namespace Westwind.Webstore.Business.Entities
{
    /// <summary>
    /// Customer information
    /// </summary>
    [DebuggerDisplay("{Lastname}, {Firstname} - {Company} - {Id}")]
    public class Customer
    {
        public Customer()
        {
        }

        [Key()]
        [StringLength(20)]
        public string Id { get; set; } = wsApp.NewId();

        [Required] [StringLength(150)] public string Email { get; set; }

        [Required] [StringLength(100)] public string Firstname { get; set; }

        [Required] [StringLength(100)] public string Lastname { get; set; }

        public string Fullname
        {
            get
            {
                return (string.IsNullOrEmpty(Firstname) ? string.Empty :  Firstname + " " ) + Lastname;
            }
        }

        /// <summary>
        /// Company for this customer
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// User's display name (first name)
        /// </summary>
        public string UserDisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(Firstname))
                    return Firstname;
                else if (!string.IsNullOrEmpty(Company))
                    return Company;

                return Lastname;
            }
        }

        /// <summary>
        /// THe user initials made up from firstname and last name
        /// </summary>
        [StringLength(2)]
        public string Initials
        {
            get
            {
                if (!string.IsNullOrEmpty(Firstname) && !string.IsNullOrEmpty(Lastname))
                    return Firstname[0].ToString().ToUpper() + Lastname[0].ToString().ToUpper();
                else
                    return "n/a";
            }

        }


        /// <summary>
        /// Customer's password used to log in
        /// </summary>
        [Required] [StringLength(100)] public string Password { get; set; }

        /// <summary>
        /// Two Factor key if user enabled it
        /// </summary>
        [StringLength(30)] public string TwoFactorKey { get; set;  }


        /// <summary>
        /// Validation code set when verifying the user
        /// </summary>
        [StringLength(40)]
        public string ValidationKey { get; set; }

        /// <summary>
        /// Determines if this user is a system administrator
        /// </summary>
        public bool IsAdminUser { get; set; }

        /// <summary>
        /// Main Telephone number
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// Any additional customer notes for administration
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Any notes the customer wants to associate with his/her account
        /// </summary>
        public string CustomerNotes { get; set; }

        /// <summary>
        /// Initial referral code
        /// </summary>
        public string ReferralCode { get; set; }

        /// <summary>
        /// Determines whether the customer account is active
        /// </summary>
        public bool IsActive { get; set; }


        /// <summary>
        /// Determines whether this customer is being added
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public bool IsNew { get; set; }

        /// <summary>
        /// Date of the last order placed
        /// </summary>
        public DateTime? LastOrder { get; set; }

        /// <summary>
        /// When customer was first entered
        /// </summary>
        public DateTime Entered { get; set; } = DateTime.Now;

        /// <summary>
        /// When customer was last updated
        /// </summary>
        public DateTime Updated { get; set; } = DateTime.Now;


        /// <summary>
        /// List of addresses - first item is the primary address that is displayed
        /// for the customer and used for orders unless the user chooses a different one
        /// </summary>
        public virtual List<Address> Addresses { get; set; } = new List<Address>();


        /// <summary>
        /// Store Language used
        /// </summary>
        public string LanguageId { get; set; }

        /// <summary>
        /// Theme used for the store's styling
        /// </summary>
        public string Theme { get; set; } = "Light";

        /// <summary>
        /// Object that is serialized and allows storing additional data.
        /// </summary>
        [NotMapped]
        public CustomerExtraProperties ExtraProperties
        {
            get
            {
                if (_extraProperties == null)
                {
                    if (!string.IsNullOrEmpty(ExtraPropertiesStorage))
                        _extraProperties = JsonSerializationUtils.Deserialize<CustomerExtraProperties>(ExtraPropertiesStorage);
                }

                if (_extraProperties == null)
                    _extraProperties = new CustomerExtraProperties();

                return _extraProperties;
            }
            set
            {
                _extraProperties = value;
            }
        }

        /// <summary>
        /// JSON Storage for the ExtraProperties object
        /// </summary>
        public string ExtraPropertiesStorage {
            get
            {
                return _extraPropertiesStorage;
            }
            set
            {
                _extraPropertiesStorage = value;
                _extraProperties = null; // lazy load
            }

        }

        internal string _extraPropertiesStorage = null;


        internal CustomerExtraProperties _extraProperties;

        public int OldPk { get; set; }

        public override string ToString()
        {
            return $"{Lastname}, {Firstname} - {Company} - {Id}";
        }
    }


    public enum AddressTypes
    {
        Billing,
        Shipping,
        Other
    }

    /// <summary>
    /// Dynamic extra properties that can be added to without schema changes
    /// </summary>
    public class CustomerExtraProperties
    { }


}
