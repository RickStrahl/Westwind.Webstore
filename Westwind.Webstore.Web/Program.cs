/*
 * CommandLine Parameters supported:
 *   -CreateDatabase    -   Creates database and default lookup values
 *                          uses configured connectionstring
*/
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
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
using Newtonsoft.Json;
using Serilog;
using Westwind.AspNetCore;
using Westwind.AspNetCore.Errors;
using Westwind.AspNetCore.Extensions;
using Westwind.AspNetCore.LiveReload;
using Westwind.AspNetCore.Middleware;
using Westwind.Globalization.AspnetCore;
using Westwind.Utilities;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;
using Microsoft.AspNetCore.Hosting.Server.Features;

var configFile = "_webstore-configuration.json";
bool noConfig = !File.Exists(configFile);

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// constants
wsApp.IsDevelopment = builder.Environment.IsDevelopment();
wsApp.EnvironmentName = builder.Environment.EnvironmentName;

wsApp.Constants.StartupFolder = Environment.CurrentDirectory;
wsApp.Constants.WebRootFolder = System.IO.Path.Combine(wsApp.Constants.StartupFolder, "wwwroot");
//DataProtector.UniqueIdentifier = "WebStore!*9@11";

// base configuration (read from _webstore-configuration.json initially)
// then apply ASP.NET Core configuration on top of it here
var config = wsApp.Configuration;
builder.Configuration.GetSection("WebStore").Bind(config);
services.AddSingleton(config);

if (noConfig)
{

    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine($"*** Configuration file '{configFile}' not found. Creating...");
    Console.ResetColor();

    try
    {
        config.Write(); // write out full configuration
        Console.WriteLine("Please edit the configuration file and set the connection string and then restart the application.");
        ShellUtils.ShellExecute(configFile);
    }
    catch
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Couldn't find and write configuration file.\nPlease check for write permissions, or manually create: {configFile}.");
    }
    return;
}

if (CommandLineProcessor.CreateDatabase(args))
    return;

// logging
var logConfig = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .Enrich.FromLogContext()
    // .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}---{NewLine}")
    .WriteTo.File(
        System.IO.Path.Combine(wsApp.Constants.WebRootFolder, "admin", "applicationlog.txt"),
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
    // opt.ResourceAccessMode = ResourceAccessMode.DbResourceManager;  // .DbResourceManager or .Resx

    // // Make sure the database you connect to exists
    // opt.ConnectionString = "host=localhost;database=localizations;uid=localizations;pwd=local";

    // Database provider used - Sql Server is the default
    //opt.DataProvider = DbResourceProviderTypes.SqlServer;

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
        builder => builder
            .WithOrigins(wsApp.Configuration.ApplicationHomeUrl,
                    "http://localhost:5173",
                    "https://localhost:5174",
                    "https://websurgeserver.west-wind.com")
            //.AllowAnyOrigin()  // doesn't work with AllowCredentials
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

// Configure UserState object to automatically fill for base controllers
UserStateWebSettings.Current = new UserStateWebSettings()
{
    IsUserStateEnabled = true,
    PersistanceMode = UserStatePersistanceModes.IdentityClaims,
    CookieEncryptionKey = "djad3ad4ad9Qd4W3td#2pI0o@--",
    CookieTimeoutDays = 5
};


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

services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
    using var lookups = BusinessFactory.Current.GetLookupBusiness();
    var res = lookups.GetPromoCodePercentage("RESELLER");
    Console.WriteLine("EF pre-loading completed.");
}).FireAndForget();

if(wsApp.Configuration.System.LiveReloadEnabled)
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

}

if (wsApp.Configuration.System.RedirectToHttps)
    app.UseHsts();


var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("en"),
    //new CultureInfo("de-DE"),
    //new CultureInfo("de")
};
app.UseRequestLocalization(options =>
{
    // ALWAYS display prices in the default currency symbol
    var cult = new RequestCulture("en-US");
    cult.Culture.NumberFormat.CurrencySymbol = wsApp.Configuration.CurrencySymbol;
    options.DefaultRequestCulture = cult;
    foreach (var culture in supportedCultures)
    {
        culture.NumberFormat.CurrencySymbol = wsApp.Configuration.CurrencySymbol;
    }
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

if (wsApp.Configuration.System.RedirectToHttps)
    app.UseHttpsRedirection();

// root path
app.UseCustomHeaders((opt) =>
{
    opt.PrimaryRequestsOnly = false; // only HTML/API requests

    opt.HeadersToAdd.Add("X-Frame-Options", "DENY");
    opt.HeadersToAdd.Add("X-XSS-Protection", "1; mode=block");
    opt.HeadersToAdd.Add("X-Content-Type-Options", "nosniff");
    opt.HeadersToAdd.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");

    // Can't remove but we can 'replace'
    opt.HeadersToAdd.Add("Server", "nginx");
});



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
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Added image path mapping to: {itemImagePath}\n");
        Console.ResetColor();
    }
    else
    {
        app.UseStaticFiles();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Warning: Image Path doesn't exist: {itemImagePath}\n");
        Console.ResetColor();

        Log.Warning($"Image Path doesn't exist: {itemImagePath}");
    }
}

app.UseStatusCodePages(new StatusCodePagesOptions
{
    HandleAsync =  (ctx) =>
    {
        // Custom Status Code Error Handling
        if (ctx.HttpContext.Response.StatusCode == 404)
        {
            ctx.HttpContext.Response.Redirect("/home/missingpage?url=" + ctx.HttpContext.Request.GetUrl());
            return Task.FromResult(0);
        }
        if (ctx.HttpContext.Response.StatusCode == 401)
        {
            throw new HttpRequestException("Unauthorized: " + ctx.HttpContext.Request.Path, null, statusCode: System.Net.HttpStatusCode.Unauthorized);
        }

        // pass exception as is to error handler
        return Task.FromResult(0);
    }
});

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.UseCors("CorsPolicy");

if (wsApp.Configuration.System.ShowConsoleRequestTimings)
{
    app.Use(async (ctx, next) =>
    {
        ctx.Request.EnableBuffering();
        var timer = new Stopwatch();
        timer.Start();

        await next(ctx);

        timer.Stop();
        var method = ctx.Request.Method;
        Console.WriteLine( method + " " + wsApp.Configuration.ApplicationHomeUrl.TrimEnd('/',' ') +  ctx.Request.Path.Value  + " - " + timer.ElapsedMilliseconds.ToString("n0") + "ms");
        if (method == "POST" || method == "PUT")
        {
            ctx.Request.Body.Position = 0;
            var body = await ctx.Request.GetRawBodyStringAsync();
            if (body != null)
            {
                if (body.Length > 2000)
                    body = body.GetMaxCharacters(2000) + "...\n2,000 bytes of " + ctx.Request.ContentLength + " bytes.";
                Console.WriteLine(body + "\n");
            }
        }
    });
}



app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    //endpoints.MapRazorPages();
});



wsApp.Constants.AppStartedOn = DateTime.Now;



app.Start();

Console.ForegroundColor = ConsoleColor.DarkYellow;
Console.WriteLine($@"---------------------------------
West Wind Web Store v{wsApp.Version}
---------------------------------");
Console.ResetColor();

var urlList = app.Urls;
string urls = string.Join(" ", urlList);

Console.Write("    Urls: ");
Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.WriteLine(urls, ConsoleColor.DarkCyan);
Console.ResetColor();

Console.WriteLine($" Runtime: {RuntimeInformation.FrameworkDescription} - {builder.Environment.EnvironmentName}");
Console.WriteLine($"Platform: {RuntimeInformation.OSDescription} ({RuntimeInformation.OSArchitecture})");
Console.WriteLine();

//Console.WriteLine(config.ConnectionString);

app.WaitForShutdown();






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
