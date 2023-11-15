using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;


namespace Westwind.Webstore.Business.Entities
{
    public class WebStoreContext : DbContext
    {
        public WebStoreContext() : base(CreateDbContextOptions())
        {

        }

        public WebStoreContext(DbContextOptions options) : base(options)
        {

        }

        public static DbContextOptions CreateDbContextOptions(DbContextOptionsBuilder builder = null,
            string connectionString = null, ILoggerFactory loggerFactory = null)
        {
            if (string.IsNullOrEmpty(connectionString))
                connectionString = wsApp.Configuration.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
                connectionString = wsApp.Constants.DefaultConnectionString;

            if (builder == null)
                builder = new DbContextOptionsBuilder();

            builder
                .UseLazyLoadingProxies()
                .UseSqlServer(connectionString,
                    opt =>
                    {
                        opt.EnableRetryOnFailure()
                            .CommandTimeout(15)
                            .MigrationsAssembly("Westwind.WebStore.Business");
                    });
            if (wsApp.Configuration.System.ShowConsoleDbCommands)
                builder.LogTo(Console.WriteLine,  new  [] { RelationalEventId.CommandExecuted})
                    .EnableSensitiveDataLogging();

            if (loggerFactory != null)
                builder.UseLoggerFactory(loggerFactory);

            return builder.Options;
        }

        /// <summary>
        /// Allows creating a new WebStore Context outside of DI for a few
        /// edge case scenarios (like app startup) where DI may not be
        /// available.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static WebStoreContext CreateContext(string connectionString = null)
        {
            var builder = new DbContextOptionsBuilder<WebStoreContext>();
            builder.UseSqlServer(connectionString ?? wsApp.Configuration.ConnectionString);
            if (wsApp.Configuration.System.ShowConsoleDbCommands)
                builder.LogTo(Console.WriteLine,  new  [] { RelationalEventId.CommandExecuted})
                    .EnableSensitiveDataLogging();
            var context = new WebStoreContext(builder.Options);
            return context;
        }

        public string ConnectionString { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<LineItem> LineItems { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Lookup> Lookups { get; set; }

        public DbSet<Category> Categories { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity("Westwind.Webstore.Business.Entities.Invoice", b =>
            //{
            //    b.HasOne("Westwind.Webstore.Business.Entities.Customer", "Customer");

            //    b.Navigation("Customer");
            //});

            base.OnModelCreating(builder);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (optionsBuilder.IsConfigured)
                return;

            // Auto configuration
            ConnectionString = wsApp.Configuration.ConnectionString;
            optionsBuilder.UseSqlServer(ConnectionString);
            if (wsApp.Configuration.System.ShowConsoleDbCommands)
                optionsBuilder.LogTo(Console.WriteLine,  new  [] { RelationalEventId.CommandExecuted})
                    .EnableSensitiveDataLogging();
        }

    }

    /// <summary>
    /// Custom class that returns a single query item with a string value (id)
    /// Use for direct DB queries that return simple lists
    /// </summary>
    public class IdListItem {
        public string Id {get; set;}
    }
}
