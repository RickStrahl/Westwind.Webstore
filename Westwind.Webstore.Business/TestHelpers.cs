using Westwind.Webstore.Business;
using Microsoft.Extensions.DependencyInjection;

namespace Westwind.Webstore.Business.Test
{
    public class TestHelpers
    {
        public static BusinessFactory GetBusinessFactoryWithProvider(bool noProviderBuild = false)
        {
            var BusinessFactory = new Webstore.Business.BusinessFactory();
            var services = new ServiceCollection();
            BusinessFactory.AddServices(services);

            if(!noProviderBuild)
                BusinessFactory.ServiceProvider = services.BuildServiceProvider();

            BusinessFactory.Current = BusinessFactory;

            return BusinessFactory;
        }
    }
}
