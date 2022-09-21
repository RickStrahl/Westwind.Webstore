# West Wind Web Store 

## Getting Started

* [Configuration Settings](#configuration-settings)
* Migrations

### Configuration Settings
Configuration is based on a series of configuration files/settings that use a static base file, augmented by standard ASP.NET Configuration customization.

None of the configuration files are part of the GIT repository so they will need to be created or generated.

* **_webstoreconfiguration.json for Base Settings**  
This is the base configuration file that is **not checked into source control and not published to the server**. 

This file is meant to be customized at the running site. This file can be auto-generated from current settings by saving in Admin or when starting up for the first time (assuming app has file write permissions in the start folder).

* **ASP.NET Configuration for Overrides**  
ASP.NET Settings can be used in addition (or instead of) the `_webstoreconfiguration.json` file to override settings set in the base configuration. Use these settings for securing values or for customizing values for specific environments. Settings are organized in this order:

    * **appsettings.json/appsettings.Development.json**
    * **User Security**
    * **Environment**

### Applying Migrations
The Web Store comes with a migration framework that allows you to attempt to bring an existing database up to date. As long as 

Migrations live in the `Westwind.WebStore.Business` project and are best run from there.


