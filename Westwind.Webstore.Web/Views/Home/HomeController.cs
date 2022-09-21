using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Westwind.AspNetCore;
using Westwind.AspNetCore.Markdown;
using Westwind.Utilities;
using Westwind.Webstore.Business;
using Westwind.Webstore.Web.App;
using Westwind.Webstore.Web.Models;

namespace Westwind.Webstore.Web.Controllers
{
    public class HomeController : WebStoreBaseController
    {
        public BusinessFactory BusinessFactory { get; }


        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,  BusinessFactory businessFactory)
        {
            BusinessFactory = businessFactory;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = CreateViewModel<ProductListViewModel>();

            var bus = BusinessFactory.GetProductBusiness();
            var productList = bus.GetItems(new InventoryItemsFilter { ListOrder = InventoryListOrder.Specials});

            model.ItemList = productList;
            return View("Index", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionHandlerPath=  HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exceptionHandler =   HttpContext.Features.Get<IExceptionHandlerFeature>();

            var mainException = exceptionHandler?.Error;
            var pathException = exceptionHandlerPath?.Error;

            var model = new ErrorViewModel {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Error = pathException,
                StatusCode = (int) HttpStatusCode.InternalServerError
            };

            var header = StringUtils.GetLines(mainException.Message).FirstOrDefault();

            if (pathException is HttpRequestException httpEx)
            {
                model.StatusCode = (int) httpEx.StatusCode.Value;
                Response.StatusCode = (int) model.StatusCode;
            }
            else
            {
                var msg = $@"
<style>
body {{ font-family: sans-serif }}
</style>
# {header}

**({(int) model.StatusCode}) {exceptionHandlerPath?.Path}**

```
{mainException.StackTrace}*
```
---
";
                _logger.LogCritical(pathException,msg);

                if (wsApp.Configuration.Email.SendAdminEmails)
                {
                    AppUtils.SendEmail(wsApp.Configuration.Email.AdminSenderEmail,
                        $"Web Store Error: {model.StatusCode} " + exceptionHandlerPath?.Path,
                        Markdown.Parse(msg),
                         out string error, noHtml: false);
                }
            }


            InitializeViewModel(model);

            return View(model);
        }


    }
}
