using Microsoft.Extensions.DependencyInjection;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;

namespace Westwind.Webstore.Business.Test
{
    public class TestHelpers
    {
        public static BusinessFactory GetBusinessFactoryWithProvider()
        {
            BusinessFactory.Current = BusinessFactory.CreateFactoryWithProvider(wsApp.Configuration.ConnectionString);
            return BusinessFactory.Current;
        }
    }
}
