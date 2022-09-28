using Westwind.Webstore.Business.Entities;
using Westwind.Webstore.Web.Models;

namespace Westwind.Webstore.Web;

public class CustomerEditorViewModel : WebStoreBaseViewModel
{
    public Customer Customer { get; set; } = new Customer();

    public Address BillingAddress { get; set; } = new Address();

    public string SearchTerm { get; set; }

}
