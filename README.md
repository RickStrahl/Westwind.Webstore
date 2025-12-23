# West Wind Web Store 
This is essentially a code dump of my online store. It's not meant as a product or even an inspiring example it's here primarily for my own reference and  I use it as a code reference that I can link to from posts and other reference materials.

As such there's no documentation, no support, no nothing. Feel free to browse around or use at your own risk.


## Getting Started

* [Configuration Settings](#configuration-settings)
* [Migrations](#applying-migrations)


### Configuration Settings
Configuration is based on a series of configuration files/settings that use a static base file, augmented by standard ASP.NET Configuration customization.

None of the configuration files are part of the GIT repository so they will need to be created or generated.

* **_webstore-configuration.json for Base Settings**  
This is the base configuration file that is **not checked into source control and not published to the server**. 

* **ASP.NET Core Settings Override: appsetting.json, Environment, Command Line etc.**  
On top of the base configuration file you can **override** or **replace** these existing settings in any of the ASP.NET default configurations. To so use the base `WebStore` key. 


#### _webstore-configuration.json
This file is not installed by default but can be written out from Administration settings or manually created to use as a base container for settings. The settings here reflect the base settings that are read before any other settings from the ASP.NET settings are applied. Think of the settings in this file as the base that can be overwritten for specific deployment/run scenarios.

The base configuration looks like this and maps the structure of the `WebStoreConfiguration` object which is accessed in the application via `wsApp.Configuration`  or via DI.

```json
{
  "ApplicationCompany": "West Wind Technologies",
  "ApplicationDomain": "my-domain.com",
  "ApplicationHomeUrl": "https://store.my-domain.com/",
  "ApplicationName": "West Wind Web Store",
  "ConnectionString": "server=.;database=WestwindWebStore;integrated security=yes;encrypt=false;",
  "ProductImageUploadFilePath": "wwwroot\\images\\product-images\\",
  "ProductImageWebPath": "/images/product-images/",
  "Theme": "default",
  "TitleBottom": "Web Store",
  "TitleTop": "West Wind",
  "DefaultCountryCode": "US",
  "CurrencySymbol": "$",
  "Email": {
    "AdminSenderEmail": "admin@my-domain.com",
    "MailServer": "smtp.mail-server.net:587",
    "MailServerUsername": "webstore@my-domain.com",
    "MailServerPassword": "mortalMistake#4",
    "SenderEmail": "sales@my-domain.com",
    "SenderName": "Bigshot Company",
    "UseSsl": true,
    "CcList": "admin@my-domain.com",
    "AdminCcList": "admin@my-domain.com",
    "SendAdminEmails": true
  },
  "Payment": {
    "MerchantId": "",
    "PublicKey": "m",
    "PrivateKey": "",
    "MerchantPassword": "",
    "TestMode": true,
    "ProcessCardsOnline": true,
    "DefaultCardProcessType": "Sale",
    "ProcessConnectionTimeoutSeconds": 20,
    "LogFile": "logs\\cclog.txt",
    "ReferingOrderUrl": null,
    "ClearCardInfoAfterApproval": true,
    "TaxState": "HI",
    "TaxRate": 0.04,
    "TransactionHtmlLink": "https://www.braintreegateway.com/merchants/{1}/transactions/{0}"
  },
  "Company": {
    "ReportCompanyLogoImage": "/images/WestWindLogo.png",
    "CompanyName": "Bigshot Company",
    "Address": "Boulder, CO 33121\nUSA",
    "Telephone": "",
    "Email": "sales@my-domain.com",
    "WebSite": "my-domain.com"
  },
  "Licensing": {
    "IsLicensingEnabled": true,
    "ServerUrl": "https://licensing.my-domain.com",
    "Username": "licenseuser",
    "Password": "FrizzleFry#1"
  },
  "Security": {
    "ValidateEmailAddresses": true,
    "UseOrderFormRecaptcha": true,
    "GoogleRecaptureSecret": "",
    "GoogleRecaptureSiteKey": ",
    "UseOrderFormTimeout": false,
    "OrderFormMinimumSecondsTimeout": 10,
    "OrderFormMaximumMinutesTimeout": 15
  },
  "System": {
    "LiveReloadEnabled": true,
    "RedirectToHttps": false,
    "ErrorDisplayMode": "Application",
    "CookieTimeoutDays": 1
  }
}
```

> #### Configuration File does not install from Git
> Since this file is custom for each installation we don't distribute this file through Git meaning you won't see this file initially in your installation. When you run the application for the first time, the store will try to write out this file from the initial default settings as `_webstore-configuration.json` in your application's root folder.
>
> You can also write out the file **with current running settings** from the Administration page by **View/Edit Configuration settings -> Write Configuration Settings** (requires that the app can start however)


This file is meant to be customized at the running site. This file can be auto-generated from current settings by saving in Admin or when starting up for the first time (assuming app has file write permissions in the start folder).

#### ASP.NET Configuration for Overrides
On top of the base configuration, you can then override settings via the standard ASP.NET configuration mechanisms in `appsettings`, Environment, Commandline, User Secrets etc. via the `WebStore` base key in the configuration.

The store does not ship `appsettings.json` or `appsettings.Development.json` in order to avoid overwriting local customization settings, but here is what those files look like with a few overrides:


**appsettings.json**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Warning"
    }
  },
  "AllowedHosts": "*",
  "WebStore": {
    "ApplicationHomeUrl": "https://local.my-domain.west-wind.com/",
    "System": {
      "LiveReloadEnabled": false,
      "RedirectToHttps": true,
      "ErrorDisplayMode": "Application"
    }
  }
}
```

**appsettings.Development.json**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Information",
      "Microsoft.Hosting.Lifetime": "Warning"
    }
  },
  "AllowedHosts": "*",
  "WebStore": {
    "ConnectionString": "server=local.my-domain;database=WestwindWebStore;integrated security=yes;encrypt=false;",
    "ApplicationHomeUrl": "https://local.my-domain.west-wind.com/",
    "System": {
      "LiveReloadEnabled": true,
      "RedirectToHttps": false,
      "ErrorDisplayMode": "Developer"
    }
  }
}
```

## Initial Database Creation



### Applying Migrations
The Web Store comes with a migration framework that allows you to attempt to bring an existing database up to date. As long as 

Migrations live in the `Westwind.WebStore.Business` project and are best run from there.


