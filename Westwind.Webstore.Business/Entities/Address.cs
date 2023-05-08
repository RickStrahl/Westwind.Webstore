using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Westwind.Webstore.Business.Entities;

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

    /// <summary>
    /// Email Address at the time of the order
    /// </summary>
    public string Email { get; set;  }


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
