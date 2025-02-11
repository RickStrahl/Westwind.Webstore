using System.Collections.Generic;
using System.Linq;
using Westwind.Utilities;
using Westwind.Webstore.Business.Entities;

namespace Westwind.Webstore.Business.Utilities;

/// <summary>
/// Seed Data Exporter and importer that captures
/// Categories and Lookups as JSON and allows importing
/// from the exported Json structure.
/// </summary>
public class ImportExportSeedData
{
    /// <summary>
    /// Exports the Categories and Lookup tables
    /// as a Json Structure
    /// </summary>
    /// <returns></returns>
    public static string Export()
    {
        var bus = BusinessFactory.Current.GetAdminBusiness();

        var data = new ExportSeedData()
        {
            Categories = bus.Context.Categories,
            Lookups = bus.Context.Lookups.OrderBy(l=> l.Key)
        };

        return JsonSerializationUtils.Serialize(data, formatJsonOutput: true);
    }

    public static ExportSeedData ExportObject()
    {
        var bus = BusinessFactory.Current.GetAdminBusiness();
        return new ExportSeedData()
        {
            Categories = bus.Context.Categories,
            Lookups = bus.Context.Lookups.OrderBy(l=> l.Key)
        };
    }

    /// <summary>
    /// Imports from a JSON string and applies to the database
    /// </summary>
    /// <param name="json">Json from Export method</param>
    /// <param name="removeExisting">Remove existing records if true</param>
    public static bool Import(string json, bool removeExisting = false)
    {
        var bus = BusinessFactory.Current.GetAdminBusiness();

        var data = JsonSerializationUtils.Deserialize<ExportSeedData>(json);

        if (removeExisting)
        {
            // clear out existing data
            foreach (var cat in bus.Context.Categories)
                bus.Context.Categories.Remove(cat);
            foreach (var lookup in bus.Context.Lookups)
                bus.Context.Lookups.Remove(lookup);
        }

        // add new data
        bus.Context.Categories.AddRange(data.Categories);
        bus.Context.Lookups.AddRange(data.Lookups);

        return bus.Context.SaveChanges() > -1;
    }

    /// <summary>
    /// Imports from a JSON string and applies to the database
    /// </summary>
    /// <param name="data">Seed data object</param>
    /// <param name="removeExisting">remove existing records if true</param>

    public static bool ImportObject(ExportSeedData data, bool removeExisting = false)
    {
        var bus = BusinessFactory.Current.GetAdminBusiness();

        if (removeExisting)
        {
            // clear out existing data
            foreach (var cat in bus.Context.Categories)
                bus.Context.Categories.Remove(cat);
            foreach (var lookup in bus.Context.Lookups)
                bus.Context.Lookups.Remove(lookup);
        }

        // add new data
        bus.Context.Categories.AddRange(data.Categories);
        bus.Context.Lookups.AddRange(data.Lookups);

        return bus.Context.SaveChanges() > -1;
    }
}

public class ExportSeedData
{
    public IEnumerable<Category> Categories { get; set; }
    public IEnumerable<Lookup> Lookups { get; set; }
}
