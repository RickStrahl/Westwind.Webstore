﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Westwind.AspNetCore.Errors;
using Westwind.AspNetCore.LiveReload;
using Westwind.AspNetCore.Middleware;
using Westwind.AspNetCore.Security;
using Westwind.Globalization;
using Westwind.Globalization.AspnetCore;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// constants
wsApp.IsDevelopment = builder.Environment.IsDevelopment();
wsApp.Constants.StartupFolder = Environment.CurrentDirectory;
wsApp.Constants.WebRootFolder = System.IO.Path.Combine(wsApp.Constants.StartupFolder, "wwwroot");
//DataProtector.UniqueIdentifier = "WebStore!*9@11";

// base configuration
var config = wsApp.Configuration;
builder.Configuration.GetSection("WebStore").Bind(config);
services.AddSingleton(config);

// logging
var logConfig = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .Enrich.FromLogContext()
    // .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}---{NewLine}")
    .WriteTo.File(
        System.IO.Path.Combine(wsApp.Constants.WebRootFolder, "admin","applicationlog.txt"),
        fileSizeLimitBytes: 3_000_000,
        retainedFileCountLimit: 5,
        rollOnFileSizeLimit: true,
        shared: true,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}{NewLine}---",
        flushToDiskInterval: TimeSpan.FromSeconds(20));
Log.Logger = logConfig.CreateLogger();
builder.Logging.AddSerilog(Log.Logger);

Log.Logger.Information("Application Started.");

services.AddLiveReload(config =>
{
    config.LiveReloadEnabled = wsApp.Configuration.System.LiveReloadEnabled;
    config.RefreshInclusionFilter = path =>
    {
        if (path.Contains("/LocalizationAdmin", StringComparison.OrdinalIgnoreCase))
            return RefreshInclusionModes.DontRefresh;

        return RefreshInclusionModes.ContinueProcessing;
    };
});

// Replace StringLocalizers with Db Resource Implementation
services.AddSingleton(typeof(IStringLocalizerFactory),
    typeof(DbResStringLocalizerFactory));
services.AddSingleton(typeof(IHtmlLocalizerFactory),
    typeof(DbResHtmlLocalizerFactory));

services.AddWestwindGlobalization(opt =>
{
    // the default settings come from DbResourceConfiguration.json if exists
    // you can override the settings here, the config you create is added
    // to the DI system (DbResourceConfiguration)

    // Resource Mode - from Database (or Resx for serving from Resources)
    opt.ResourceAccessMode = ResourceAccessMode.Resx; //DbResourceManager;  // .DbResourceManager or .Resx

    // // Make sure the database you connect to exists
    // opt.ConnectionString = "host=localhost;database=localizations;uid=localizations;pwd=local";

    // Database provider used - Sql Server is the default
    opt.DataProvider = DbResourceProviderTypes.SqlServer;

    // The table in which resources are stored
    opt.ResourceTableName = "localizations";

    opt.AddMissingResources = false;
    opt.ResxBaseFolder = "~/Properties/";

    // Set up security for Localization Administration form
    opt.ConfigureAuthorizeLocalizationAdministration(actionContext =>
    {
        // return true or false whether this request is authorized
        return true;   //actionContext.HttpContext.User.Identity.IsAuthenticated;
    });

});

services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.WithOrigins(wsApp.Configuration.ApplicationHomeUrl)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// set up and configure Authentication - make sure to call .UseAuthentication()
services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)

    .AddCookie(o =>
    {
        o.LoginPath = "/account/signin";
        o.LogoutPath = "/account/signout";
        o.SlidingExpiration = true;
        o.ExpireTimeSpan = new TimeSpan(30, 0, 0, 0);
        o.Cookie.Name = "ww_ws";
    });



//services.AddRazorPages().AddRazorRuntimeCompilation();
var aspnetServices = services.AddControllersWithViews()
    .AddNewtonsoftJson(opt =>
    {
        if (builder.Environment.IsDevelopment())
            opt.SerializerSettings.Formatting = Formatting.Indented;
    })
    .AddRazorOptions(opt =>
    {
        // Add ShoppingCart View Folder to Shared list
        opt.ViewLocationExpanders.Add(new ViewLocationExpander());
    })
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();
if (wsApp.Configuration.System.LiveReloadEnabled)
    aspnetServices.AddRazorRuntimeCompilation();

// this *has to go here* after view localization has been initialized
// so that Pages can localize - note required even if you're not using
// the DbResource manager.
services.AddTransient<IViewLocalizer, DbResViewLocalizer>();

services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

// We want a transient context!
services.AddTransient<WebStoreContext>( services =>
{
    var options = WebStoreContext.CreateDbContextOptions();
    var context = new WebStoreContext(options);
    return context;
});

BusinessFactory.AddServices(services, noDbContext: true);

// Create global instance accessible as a Singleton (BusinessFactory.Current)
BusinessFactory.CreateFactoryWithProvider(wsApp.Configuration.ConnectionString);


// ***  BUILD ***
var app = builder.Build();

// Warm up Context in the background
Task.Run(() =>
{
    // DI doesn't work here because no request scope (DbContextOptions)

    //var factory = BusinessFactory.CreateFactoryWithProvider();
    //var lookups = factory.GetLookupBusiness();

    var lookups = app.Services.GetService<LookupBusiness>();
    var res = lookups.GetPromoCodePercentage("RESELLER");
    Console.WriteLine("Lookup retrieved.");
});

app.UseLiveReload();


if (config.System.ErrorDisplayMode == ErrorDisplayModes.Developer)
{
    app.UseDeveloperExceptionPage();
    ApiExceptionFilterAttribute.ShowExceptionDetail = true;
}
else
{
    app.UseExceptionHandler("/Home/Error");
    ApiExceptionFilterAttribute.ShowExceptionDetail = config.System.ErrorDisplayMode != ErrorDisplayModes.Application;

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("en"),
    // new CultureInfo("de-DE"),
    // new CultureInfo("de")
};
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});


app.UseCustomHeaders((opt) =>
{
    opt.PrimaryRequestsOnly = false; // only HTML/API requests

    opt.HeadersToAdd.Add("X-Frame-Options", "DENY");
    opt.HeadersToAdd.Add("X-XSS-Protection", "1; mode=block");
    opt.HeadersToAdd.Add("X-Content-Type-Options", "nosniff");
    opt.HeadersToAdd.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");

    opt.HeadersToRemove.Add("X-Powered-By");
    opt.HeadersToRemove.Add("x-aspnet-version");
});


if (wsApp.Configuration.System.RedirectToHttps)
    app.UseHttpsRedirection();

// root path

var itemImagePath = wsApp.Configuration.ProductImageUploadFilePath;
if (!string.IsNullOrEmpty(itemImagePath))
{
    itemImagePath = System.IO.Path.GetFullPath(itemImagePath);
    if (System.IO.Directory.Exists(itemImagePath))
    {
        app.UseStaticFiles()
           .UseStaticFiles(new StaticFileOptions()
           {
                FileProvider = new PhysicalFileProvider(itemImagePath),
                RequestPath = new PathString("/product-images"),
                DefaultContentType = "application/octet-stream"
           });
        Console.WriteLine($"Added image path mapping to: {itemImagePath}");
    }
    else
    {
        app.UseStaticFiles();
        Console.WriteLine("Warning: " + $"Image Path doesn't exist: {itemImagePath}");
        Log.Warning($"Image Path doesn't exist: {itemImagePath}");
    }
}

app.UseStatusCodePages(new StatusCodePagesOptions
{
    HandleAsync = (ctx) =>
    {
        if (ctx.HttpContext.Response.StatusCode == 404)
        {
            throw new HttpRequestException("Page not  found: " + ctx.HttpContext.Request.Path, null, statusCode: System.Net.HttpStatusCode.NotFound);
        }
        else if (ctx.HttpContext.Response.StatusCode == 401)
        {
            throw new HttpRequestException("Unauthorized: " + ctx.HttpContext.Request.Path, null, statusCode: System.Net.HttpStatusCode.Unauthorized);
        }

        return Task.FromResult(0);
    }
});

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.UseCors("CorsPolicy");


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    //endpoints.MapRazorPages();
});



Console.ForegroundColor = ConsoleColor.DarkYellow;
Console.WriteLine($@"---------------------------------
West Wind Web Store
---------------------------------");
Console.ResetColor();


var urls = builder.WebHost.GetSetting(WebHostDefaults.ServerUrlsKey)?.Replace(";", " ");
Console.Write($"    Urls: ");
Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.WriteLine($"{urls}", ConsoleColor.DarkCyan);
Console.ResetColor();

Console.WriteLine($" Runtime: {RuntimeInformation.FrameworkDescription} - {builder.Environment.EnvironmentName}");
Console.WriteLine($"Platform: {RuntimeInformation.OSDescription}");
Console.WriteLine();


if (!System.IO.File.Exists("_webstore-configuration.json"))
{
    try
    {
        wsApp.Configuration.Write(); // write out full configuration
    }catch { }
}

app.Run();


/// <summary>
/// Add custom View Locations:
/// * ShoppingCart folder - so Confirmations can render
/// </summary>
public class ViewLocationExpander: IViewLocationExpander {

    /// <summary>
    /// Used to specify the locations that the view engine should search to
    /// locate views.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="viewLocations"></param>
    /// <returns></returns>
    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations) {

        //{2} is area, {1} is controller,{0} is the action
        string[] locations = new string[] { "/Views/ShoppingCart/{0}.cshtml"};
        return viewLocations.Union(locations);
    }

    public void PopulateValues(ViewLocationExpanderContext context) {
        context.Values["customviewlocation"] = nameof(ViewLocationExpander);
    }
}
