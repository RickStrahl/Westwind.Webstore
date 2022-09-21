using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;

namespace Westwind.Webstore.Business
{
    public class BusinessFactory
    {
        public static BusinessFactory Current { get; set;  }

        public IServiceProvider ServiceProvider { get; set; }

        public BusinessFactory(IServiceProvider serviceProvider = null)
        {
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// This creates a Business Factory instance (also stored on `Current`
        /// with a ServiceProvider.
        ///
        /// Use this to run business objects independently of an existing provider
        /// (ie. ASP.NET Provider config)
        /// </summary>
        /// <param name="connectionString">Optional explicit connection string otherwise config values are used</param>
        /// <param name="services">Optionally pass in a service collection to add services to</param>
        /// <returns></returns>
        public static BusinessFactory CreateFactoryWithProvider( string connectionString = null, IServiceCollection services = null)
        {
            var businessFactory = new BusinessFactory();
            if (services == null)
                 services = new ServiceCollection();

            BusinessFactory.AddServices(services, connectionString);

            businessFactory.ServiceProvider = services.BuildServiceProvider();
            BusinessFactory.Current = businessFactory;

            return businessFactory;
        }

        /// <summary>
        /// Adds required services to a service collection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString"></param>
        /// <param name="noDbContext"></param>
        public static void AddServices(IServiceCollection services, string connectionString = null, bool noDbContext = false)
        {
            var config = wsApp.Configuration;
            services.AddSingleton(config);

            services.AddTransient(typeof(ProductBusiness));
            services.AddTransient(typeof(CustomerBusiness));
            services.AddTransient(typeof(InvoiceBusiness));
            services.AddTransient(typeof(LookupBusiness));
            services.AddTransient(typeof(CategoryBusiness));
            services.AddTransient(typeof(AdminBusiness));

            if (!noDbContext)
            {
                services.AddDbContext<WebStoreContext>(options =>
                {
                    if (!string.IsNullOrEmpty(connectionString))

                    WebStoreContext.CreateDbContextOptions(options);
                });
                services.AddTransient(typeof(WebStoreContext));
            }

            services.AddTransient(typeof(BusinessFactory));
        }

        public virtual void UseServices(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public virtual CustomerBusiness GetCustomerBusiness(WebStoreContext context = null)
        {
            var inst =  ServiceProvider.GetService<CustomerBusiness>();
            if (context != null)
                inst.Context = context;
            return inst;
        }

        public virtual ProductBusiness GetProductBusiness(WebStoreContext context = null)
        {
            var inst = ServiceProvider.GetService<ProductBusiness>();
            if (context != null)
                inst.Context = context;

            return inst;
        }
        public virtual InvoiceBusiness GetInvoiceBusiness(WebStoreContext context = null)
        {
            var inst = ServiceProvider.GetService<InvoiceBusiness>();
            if (context != null)
                inst.Context = context;

            return inst;
        }
        public virtual LookupBusiness GetLookupBusiness(WebStoreContext context = null)
        {
            var inst = ServiceProvider.GetService<LookupBusiness>();
            if (context != null)
                inst.Context = context;

            return inst;
        }

        public virtual CategoryBusiness GetCategoryBusiness(WebStoreContext context = null)
        {
            var inst = ServiceProvider.GetService<CategoryBusiness>();
            if (context != null)
                inst.Context = context;

            return inst;
        }

        public virtual AdminBusiness GetAdminBusiness(WebStoreContext context = null)
        {
            var inst = ServiceProvider.GetService<AdminBusiness>();
            if (context != null)
                inst.Context = context;

            return inst;
        }
    }
}
