using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Westwind.AI;
using Westwind.AI.Chat;
using Westwind.Utilities;
using Westwind.Webstore.Business.Entities;

namespace Westwind.Webstore.Business.Ai;

public class AddressParser
{
    public async Task<Customer> ParseAddress(string addressToParse, Customer customer = null)
    {
        if (customer == null)
            customer = new Customer();

        var client = new GenericAiChatClient(wsApp.Configuration.AiConnection);

        string prompt = $$"""
Take an address entered and convert it into a JSON structure of the following format:

{
    "firstName": "",
    "lastName": "",
    "middleInitial": "",
    "company": "",
    "streetAddress": "",
    "unit": "",
    "city": "",
    "stateOrProvince": "",
    "postCode": "",
    "country": "",
    "countryCode": ""
}

Return only the result as raw JSON string not Markdown.

Here's the address to parse:
{{addressToParse}}
""";
        var json = await client.Complete(prompt);

        if (client.HasError)
        {
            ErrorMessage = client.ErrorMessage;
            return null;
        }

        if (string.IsNullOrEmpty(json))
        {
            ErrorMessage = "No response from AI service.";
            return null;
        }

        json = json.Trim();
        if (!json.StartsWith("{") || !json.EndsWith("}"))
        {
            ErrorMessage = "Invalid JSON response from AI service.\n" + json ;
            return null;
        }

        var result = JsonSerializationUtils.Deserialize<ParseCustomer>(json);

        customer.Firstname = result.FirstName;
        customer.Lastname = result.LastName;
        customer.Company = result.Company;
        var address = customer.Addresses.FirstOrDefault();
        if (address == null)
        {
            address = new Address();
            customer.Addresses.Add(address);
        }
        address.StreetAddress = result.StreetAddress;
        address.City = result.City;
        address.State = result.StateOrProvince;
        address.PostalCode = result.PostCode;
        address.Country = result.Country;
        address.CountryCode = result.CountryCode;

        return customer;
    }

    public string ErrorMessage { get; set;  }
}

class ParseCustomer
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleInitial { get; set; }
    public string Company { get; set; }
    public string StreetAddress { get; set; }
    public string Unit { get; set; }
    public string City { get; set; }
    public string StateOrProvince { get; set; }
    public string PostCode { get; set; }
    public string Country { get; set; }
    public string CountryCode { get; set; }
}


