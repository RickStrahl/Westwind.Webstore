# Copilot Instructions for West Wind Web Store

## Project Overview
- **West Wind Web Store** is a modular ASP.NET Core web application for e-commerce, with a focus on extensibility and configuration flexibility.
- The solution is split into several projects:
  - `Westwind.Webstore.Web`: Main ASP.NET Core web frontend (MVC).
  - `Westwind.Webstore.Business`: Business logic, data access, migrations.
  - `Westwind.WebStore.Business.Test`: Unit/integration tests for business logic.
  - `WebStoreDatabase`: Database project (SQL Server, schema, migrations).
  - `Westwind.CreditCardProcessing`: Payment integration logic.

## Configuration & Environment
- **Configuration is layered:**
  - Base: `_webstore-configuration.json` (not in source control, generated at runtime or via admin UI).
  - Overrides: `appsettings.json`, `appsettings.Development.json`, environment variables, user secrets, etc. (all under the `WebStore` key).
  - See `README.md` for sample config structure and override patterns.
- **Never commit real config files**—use the admin UI or startup to generate them.

## Build, Run, and Test
- **Build:** Use the solution file `Westwind.Webstore.sln` with Visual Studio or `dotnet build`.
- **Run:**
  - Use `run.ps1` or `publish.ps1` for local/dev automation.
  - The web project (`Westwind.Webstore.Web`) is the main entry point.
- **Database:**
  - Migrations are in `Westwind.Webstore.Business/Migrations/`.
  - Apply migrations from the business project, not the web project.
- **Testing:**
  - Tests are in `Westwind.WebStore.Business.Test`.
  - Use `dotnet test Westwind.WebStore.Business.Test/Westwind.WebStore.Business.Test.csproj`.

## Key Patterns & Conventions
- **Dependency Injection:**
  - Business logic is accessed via `BusinessFactory` (see `HomeController` and other controllers).
- **Error Handling:**
  - Centralized in `HomeController.Error`—logs critical errors and emails admins if enabled.
- **ViewModels:**
  - Use `CreateViewModel<T>()` to instantiate and initialize view models.
- **Email:**
  - Email sending is abstracted via `AppUtils.SendEmail`.
- **Validation:**
  - Use `ValidationErrorCollection` for collecting and displaying validation errors.
- **Admin UI:**
  - Configuration and some data management can be done via the web admin interface.

## External Integrations
- **Payment:**
  - Braintree integration in `Westwind.CreditCardProcessing` and business logic.
- **Licensing:**
  - Licensing logic and server integration in `Westwind.Licensing` and config.
- **Localization:**
  - Uses `Westwind.Globalization.AspnetCore.Utilities` for i18n/l10n.

## Notable Files & Directories
- `Westwind.Webstore.Web/Controllers/HomeController.cs`: Main controller, error handling, MVP/discount flows.
- `Westwind.Webstore.Business/`: Business logic, migrations, data access.
- `DefaultConfigurationFiles/`: Example config files (not used at runtime).
- `WebStoreDatabase/`: SQL project, schema, and compare files.

## Project-Specific Advice
- Always use the admin UI or startup logic to generate config files—never copy example files directly.
- When adding new business logic, place it in `Westwind.Webstore.Business` and expose via `BusinessFactory`.
- For new settings, update both the config object and the admin UI for editing.
- For cross-cutting concerns (logging, email, error handling), use the provided abstractions (`AppUtils`, logger, etc.).


## Patterns

- Use West Wind Tools for Configuration and Utilities whenever possible
- For data access use Westwind.Business.EfCore and Entity Framework combination

---

If any section is unclear or missing, please provide feedback for further refinement.
