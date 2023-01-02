using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;
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


        [Required] [StringLength(100)] public string Password { get; set; }

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



    [DebuggerDisplay("{AddressName} {AddressType} {StreetAddress}")]
    [Table( "Addresses")]
    public class Address
    {
        [Key()]
        [StringLength(20)]
        public string Id { get; set; } = wsApp.NewId();

        [StringLength(20)] public string CustomerId { get; set; } = string.Empty;

        /// <summary>
        /// A user identifier for the address
        /// </summary>
        public string AddressName { get; set; }

        /// <summary>
        /// The full street address
        /// </summary>
        public string StreetAddress { get; set; }

        /// <summary>
        /// The city of the address
        /// </summary>
        public string City {get; set;}

        /// <summary>
        /// Zip or postal code
        /// </summary>
        public string PostalCode {get; set;}

        /// <summary>
        /// State or province
        /// </summary>
        public string State {get; set; }

        /// <summary>
        /// Full country name
        /// </summary>
        public string Country {get; set; }


        /// <summary>
        /// Country code for country lookup
        /// </summary>
        public string CountryCode {get; set; }


        /// <summary>
        /// Type of address: shipping, billing, other
        /// </summary>
        [Column(TypeName = "varchar(10)")]
        public AddressTypes AddressType { get; set; } = AddressTypes.Other;

        /// <summary>
        /// Sort order for multiple addresses displayed
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Optional - For shipping addresses if the name is different it can be specified here
        /// </summary>
        public string AddressFullname {get; set;}

        /// <summary>
        /// Optional - For shipping addresses if the company is different than primary customer
        /// the company can be specified here.
        /// </summary>
        public string AddressCompany {get; set; }

        /// <summary>
        /// Optional - For shipping addresses the the Telephone number can be specified.
        /// </summary>
        public string Telephone {get; set; }


        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(AddressFullname) &&
                   string.IsNullOrEmpty(AddressCompany) &&
                   string.IsNullOrEmpty(StreetAddress);
        }

        /// <summary>
        /// Retrieves a country name from a country code and returns it
        ///
        /// Note: does not set the Country field
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        public string GetCountryFromCode(string countryCode = null)
        {
            if (countryCode == null)
                countryCode = CountryCode;

            if (string.IsNullOrEmpty(countryCode))
                return countryCode;

            return wsApp.Countries?.FirstOrDefault(c =>
                !string.IsNullOrEmpty(c.CountryCode) &&
                c.CountryCode.Equals(countryCode, StringComparison.InvariantCultureIgnoreCase))?.Country;
        }

        public string ToString(bool noFullName = false)
        {
            if (IsEmpty()) return string.Empty;

            var sb = new StringBuilder();

            if (!noFullName && !string.IsNullOrEmpty(AddressFullname))
            {
                sb.AppendLine(AddressFullname);
                if (!string.IsNullOrEmpty(AddressCompany))
                    sb.AppendLine(AddressCompany);
            }

            if (!string.IsNullOrEmpty(this.StreetAddress))
                sb.AppendLine(StreetAddress.Trim());

            if (!(string.IsNullOrEmpty(City) && string.IsNullOrEmpty(State) && string.IsNullOrEmpty(PostalCode)))
            {
                if (!string.IsNullOrEmpty(this.City))
                    sb.Append(City + ", ");
                if (!string.IsNullOrEmpty(State))
                    sb.Append((State + " ") + PostalCode);
                else
                    sb.Append("Postal Code: " + PostalCode);

                sb.AppendLine();
            }

            string country = null;
            if (!string.IsNullOrEmpty(CountryCode)) // && CountryCode != wsApp.Configuration.DefaultCountryCode)
            {
                country = GetCountryFromCode(CountryCode);
                if (!string.IsNullOrEmpty(country))
                    sb.AppendLine(country);
            }

            return sb.ToString();
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
