using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Westwind.Webstore.Business.Entities.Context
{
    public class WebStoreContextDesignTimeFactory : IDesignTimeDbContextFactory<WebStoreContext>
    {
        public WebStoreContext CreateDbContext(string[] args)
        {
            Console.WriteLine("CreateDbContext DesignTime: " + wsApp.Configuration.ConnectionString);

            string conn = wsApp.Configuration.ConnectionString;
            if (string.IsNullOrEmpty(conn))
                conn = "server=.;database=WestwindWebStore; integrated security=true; encrypt=false;";

            var optionsBuilder = new DbContextOptionsBuilder<WebStoreContext>();
            optionsBuilder.UseSqlServer(conn, opt =>
            {
                opt.CommandTimeout(15);
                opt.EnableRetryOnFailure();
            });
            if (wsApp.IsDevelopment)
                optionsBuilder.LogTo(Console.WriteLine);

            return new WebStoreContext(optionsBuilder.Options);
        }
    }
}
