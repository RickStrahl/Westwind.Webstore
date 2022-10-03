using NUnit.Framework;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Test;

namespace Westwind.WebStore.Business.Test;

[TestFixture]
public class AdminBusinessTests
{
    private string ConnectionString = wsApp.Configuration.ConnectionString;

    public BusinessFactory BusinessFactory { get; set; }

    [SetUp]
    public void Init()
    {
        // Load Factory and Provider
        BusinessFactory = TestHelpers.GetBusinessFactoryWithProvider();
    }

    [Test]
    public void SkuEmailListTest()
    {
        var adminBus = BusinessFactory.GetAdminBusiness();
        var list = adminBus.EmailListFromSkus(null, 90);

        Assert.IsNotNull(list);
        Assert.IsNotEmpty(list);


    }

}
