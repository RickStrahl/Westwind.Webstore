# Copilot Instructions for West Wind Web Store

## Project Overview
West Wind Web Store is a modular ASP.NET Core e-commerce application built with Entity Framework Core and SQL Server. The architecture emphasizes configuration flexibility and extensibility through dependency injection.

**Key Projects:**
- `Westwind.Webstore.Web`: ASP.NET Core MVC frontend (controllers stored with views in Views/ folders)
- `Westwind.Webstore.Business`: Business logic, EF Core context, migrations
- `Westwind.WebStore.Business.Test`: Unit/integration tests (minimal, mostly integration tests)
- `WebStoreDatabase`: SQL Server database project with schema management
- `Westwind.CreditCardProcessing`: Payment processing (Braintree integration)

## Configuration Architecture
**Critical Pattern:** Configuration is completely external to source control and layered:

1. **Base:** `_webstore-configuration.json` (auto-generated, never committed)
2. **Overrides:** Standard ASP.NET config under `WebStore` key in `appsettings.json`
3. **Admin UI:** Live configuration editing through web interface

**Never commit config files** - they're generated at runtime or via admin UI. See `README.md` for complete config structure examples.

## Development Workflow
**Build & Run:**
- Root-level `run.ps1` → `cd Westwind.Webstore.Web && dotnet watch run --no-hot-reload`
- Main solution: `Westwind.Webstore.sln`

**Database Migrations:**
- Run from `Westwind.Webstore.Business` project (not web project)
- Uses `dotnet ef` commands - see `Migrations.md` for detailed workflows
- **Important:** Invoice->Customer FK requires manual fix in migrations (see `Migrations.md`)

**Testing:**
- `dotnet test Westwind.WebStore.Business.Test/Westwind.WebStore.Business.Test.csproj`

## Core Architectural Patterns
**Controllers & Views:**
- Controllers are stored alongside their views in `Views/{AreaName}/` folders
- Example: `Views/Home/HomeController.cs` and `Views/Home/Index.cshtml` are in same folder
- Controllers inherit from `WebStoreBaseController`

**Business Layer Access:**
- All business logic accessed via `BusinessFactory.Current` or DI
- Business objects inherit from `WebStoreBusinessObject<TEntity>` → `EntityFrameworkBusinessObject`

**View Models:**
- ViewModels inherit from `WebStoreBaseViewModel`
- Use `CreateViewModel<T>()` method for proper initialization

**Configuration Access:**
- Global config via `wsApp.Configuration` static property
- ViewModels automatically get `Configuration` property

**Validation:**
- Uses `ValidationErrorCollection` from Westwind.Utilities for business object validation errors

**Email & Utilities:**
- Email: `AppUtils.SendEmail()` and `Emailer` class
- Utilities centralized in `wsApp.Constants` and `AppUtils`

## Data Layer Details
- **Context:** `WebStoreContext` with lazy loading proxies enabled
- **Connection:** Auto-configured from `_webstore-configuration.json`
- **Design-time:** `WebStoreContextDesignTimeFactory` for migrations

## External Dependencies
- **Westwind.Data.EfCore:** Base business object framework
- **Westwind.AspNetCore:** View rendering, base controllers/viewmodels
- **Westwind.AI:** AI client integration (see `Ai/` folder)
- **Braintree:** Payment processing
- When adding new business logic, place it in `Westwind.Webstore.Business` and expose via `BusinessFactory`.
- For new settings, update both the config object and the admin UI for editing.
- For cross-cutting concerns (logging, email, error handling), use the provided abstractions (`AppUtils`, logger, etc.).


## Patterns

- Use West Wind Tools for Configuration and Utilities whenever possible
- For data access use Westwind.Business.EfCore and Entity Framework combination

---

If any section is unclear or missing, please provide feedback for further refinement.
