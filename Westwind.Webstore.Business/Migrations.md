# Migrations

[Documentation](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)


### Install the Dotnet EF Core Tools

```ps
dotnet tool install --global dotnet-ef
```

### Create New

```ps
dotnet ef migrations add Initial-Migration
```

### Update Database

```ps
dotnet ef database update
```

> Make sure that you are pointing at the correct database. The connection string will come from the local `_webstore-configuration.json` file in the WestStore.Business project folder and should be set there.

### Clear out Migrations and Start Over

<small>[Blog Post](https://weblog.west-wind.com/posts/2016/Jan/13/Resetting-Entity-Framework-Migrations-to-a-clean-Slate)</small>

* Delete all Migrations
* Create a new Migration `Initial-Migration`
* Comment out `Up()` method to avoid updating  (or update with `-IgnoreChanges`???)
* Update the database  



### Fix up Invoice->Customer Relationship in Initial Migration!
We need to allow for a foreign key that is not required to allow for saving a temp invoice without a customer FK required.

```csharp
// in BuildTargetModel() method:
modelBuilder.Entity("Westwind.Webstore.Business.Entities.Invoice", b =>
{
    b.HasOne("Westwind.Webstore.Business.Entities.Customer", "Customer")
        .WithMany()
        .HasForeignKey("CustomerId")
        // THIS IS THE KEY!
        .IsRequired(false);
        
        b.Navigation("Customer");
});
```

Then **explicitly remove the Foreign Key from the Invoices Table**!.

I haven't found a way to make EF express the `1->0 | 1` relation ship without creating a PK despite using nullable values for the `string? CustomerId` and `Customer? Customer` properties.