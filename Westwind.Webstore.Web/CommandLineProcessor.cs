using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Westwind.Webstore.Business;

/// <summary>
/// Command line operations
/// </summary>
public static class CommandLineProcessor
{
    /// <summary>
    /// Creates the database and runs initial migration
    /// </summary>
    /// <param name="args"></param>
    /// <returns>true if app should exit after processing</returns>
    public static bool CreateDatabase(string[] args)
    {
        if (!args.Any(s => s.Contains("-CreateDatabase", StringComparison.OrdinalIgnoreCase)))
            return false;

        Console.WriteLine("Creating Web Store Database using " + wsApp.Configuration.ConnectionString);

        try
        {
            var factory = BusinessFactory.CreateFactoryWithProvider();
            var bus = factory.GetLookupBusiness();
            bus.Context.Database.Migrate();

            if (!bus.InsertInitialData())
                Console.WriteLine("Database created, but Lookups data not created...");
            else
                Console.WriteLine("Database created.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to create database:");
            Console.WriteLine(" - " + ex.Message);
        }

        return true;
    }

}
