using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Westwind.Utilities;
using Westwind.Webstore.Business;
using Westwind.Webstore.Web.Controllers;
using Westwind.Webstore.Web.Models;
using Westwind.AspNetCore.Extensions;
using Westwind.Webstore.Business.Entities;

namespace Westwind.Webstore.Web.Views.Admin
{
    public class AdminController : WebStoreBaseController
    {
        private BusinessFactory BusinessFactory { get;  }

        public AdminController(BusinessFactory businessFactory)
        {
            BusinessFactory = businessFactory;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            base.OnActionExecuting(context);

            if (!UserState.IsAdmin)
            {
                context.HttpContext.Response.Redirect("/account/signin");
                await context.HttpContext.Response.CompleteAsync();
                return;
            }

            await next();
        }


        [Route("/admin")]
        public ActionResult Index()
        {
            var model = CreateViewModel<AdminViewModel>();

            var msg = TempData["message"] as string;
            if (!string.IsNullOrEmpty(msg))
                model.ErrorDisplay.Message = msg;
            var msgicon = TempData["messageIcon"] as string;
            if (!string.IsNullOrEmpty(msgicon))
            {
                model.ErrorDisplay.Icon = msgicon;
                model.ErrorDisplay.AlertClass = msgicon;
            }

            return View(model);
        }

        [HttpGet]
        [Route("/admin/configuration")]
        public ActionResult Configuration()
        {
            var model = CreateViewModel<ConfigurationViewModel>();
            model.ConfigurationJson = JsonSerializationUtils.Serialize(model.Configuration, false, true, false);

            return View(model);
        }

        [HttpGet]
        [Route("/admin/clearlog")]
        public ActionResult ClearLog()
        {
            var model = CreateViewModel<AdminViewModel>();
            try
            {
                // doesn't work - file is locked
                var file = Path.Combine(wsApp.Constants.WebRootFolder, "admin", "applicationlog.txt");
                System.IO.File.WriteAllText(file,"");
            }
            catch(Exception ex)
            {
                model.ErrorDisplay.ShowError("Failed to delete log file: " + ex.Message);
                return View("index", model);
            }

            model.ErrorDisplay.ShowSuccess("Log file cleared.");
            return View("index", model);
        }

        [HttpPost]
        [Route("/admin/configuration")]
        public ActionResult UpdateConfiguration(ConfigurationViewModel model)
        {
            InitializeViewModel(model);

            if (Request.IsFormVar("btnUpdateConfiguration"))
            {
                var config =
                    JsonSerializationUtils.Deserialize(model.ConfigurationJson, typeof(WebStoreConfiguration)) as
                        WebStoreConfiguration;
                if (config != null)
                {
                    wsApp.Configuration = config;
                    model.ErrorDisplay.ShowInfo("Running configuration has been updated.");
                }
                else
                {
                    model.ErrorDisplay.ShowError("Configuration could not be updated - invalid JSON.");
                }

                // see actual current values
                ModelState.Clear();
                model.ConfigurationJson = JsonSerializationUtils.Serialize(wsApp.Configuration, false, true, false);
            }
            else if (Request.IsFormVar("btnWriteConfiguration"))
            {
                if (wsApp.Configuration.Write())
                    model.ErrorDisplay.ShowInfo($"Configuration has been written out.");
                else
                    model.ErrorDisplay.ShowError($"Configuration could not be written.");
            }


            return View("Configuration",model);
        }



        [Route("/admin/backup"), HttpGet]
        public ActionResult Backup()
        {
            var model = CreateViewModel<AdminViewModel>();

            var adminBus = BusinessFactory.GetAdminBusiness();


            var basePath = Request.MapPath("/temp");
            var outFile =
                Request.MapPath($"{basePath}/Backup-WebStore-{DateTime.Now.ToString("yyyy-MM-dd-HH-mm")}.bak");

            if (Directory.Exists(basePath))
                FileUtils.DeleteFiles(basePath, "*.*", true);

            ErrorDisplay.Timeout = 10000;

            var zipFile = adminBus.BackupDatabase(outFile, zipDatabase: true);
            if (zipFile == null)
            {
                ErrorDisplay.ShowError(adminBus.ErrorMessage);
            }
            else
            {
                var zipFileName = Path.GetFileName(zipFile);
                ErrorDisplay.ShowSuccess($"<a href='/temp/{zipFileName}'>Download Backup File</a>", "Backup Completed");
                ErrorDisplay.MessageAsRawHtml = true;
            }

            return View("Index",model);
        }

        [Route("/admin/throw")]
        public IActionResult ThrowException()
        {
            throw new ApplicationException("Test Exception - an error thrown to demonstrate error handling.");
        }
    }


    public class AdminViewModel : WebStoreBaseViewModel
    {
        public string ApplicationVersion { get; } = typeof(AdminController).Assembly.GetName().Version.ToString();
        public string ApplicationDate { get;  } =
            TimeUtils.FriendlyDateString(new FileInfo(typeof(wsApp).Assembly.Location).LastWriteTime);

    }

    public class ConfigurationViewModel : WebStoreBaseViewModel
    {
        public string ConfigurationJson { get; set; }

    }
}
