using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Westwind.AspNetCore.Errors;
using Westwind.AspNetCore.Extensions;
using Westwind.AspNetCore.Markdown.Utilities;
using Westwind.AspNetCore.Messages;
using Westwind.Globalization.AspNetCore.Extensions;
using Westwind.Globalization.AspnetCore.Utilities;
using Westwind.Utilities;
using Westwind.Web;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;
using Westwind.WebStore.Utilities;
using Westwind.Webstore.Web.Controllers;
using Westwind.Webstore.Web.Models;

namespace Westwind.Webstore.Web.Admin.Controllers;

public class ProductManagerController : WebStoreBaseController
{
    public BusinessFactory BusinessFactory { get; }


    public ProductManagerController(BusinessFactory businessFactory)
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


    [HttpGet]
    [Route("/admin/ProductManager")]
    public IActionResult ProductList()
    {
        var model = CreateViewModel<ProductEditorListViewModel>();
        model.SearchTerm = Request.Query["s"];
        model.ShowNonActive = Request.Query["nonActive"] == "true";

        model.ErrorDisplay.Message = Request.Query["message"];
        var icon = Request.Query["icon"];
        if (!string.IsNullOrEmpty(icon))
        {
            model.ErrorDisplay.Icon = icon;
            model.ErrorDisplay.AlertClass = icon;
        }

        var productBus = BusinessFactory.GetProductBusiness();
        model.ProductList = productBus.GetItems( new InventoryItemsFilter() { SearchTerm = model.SearchTerm }, model.ShowNonActive );

        return View("ProductList",model);
    }

    /// <summary>
    /// Special sky of `new` creates a new product
    /// </summary>
    /// <param name="sku"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("/admin/ProductManager/{sku}")]
    public IActionResult ProductEditor(string sku)
    {
        var model = CreateViewModel<ProductEditorViewModel>();

        var productBus = BusinessFactory.GetProductBusiness();

        if (sku.Equals("new", StringComparison.OrdinalIgnoreCase))
        {
            model.Product = productBus.Create();
        }
        else
        {
            model.Product = productBus.LoadBySku(sku);
            if (model.Product == null)
            {
                model.Product = productBus.Create();
            }
        }

        model.Json = JsonSerializationUtils.Serialize(model.Product, formatJsonOutput: true);

        return View("ProductEditor",model);
    }


    /// <summary>
    /// Special sky of `new` creates a new product
    /// </summary>
    /// <param name="sku"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("/admin/ProductManager/{sku}")]
    public IActionResult ProductEditorUpdate(ProductEditorViewModel model, string sku)
    {
        InitializeViewModel(model);

        if (Request.IsFormVar("btnDeleteProduct"))
            return DeleteProduct(model);

        var productBus = BusinessFactory.GetProductBusiness();

        Product product = null;
        bool isNewItem = false;
        if (sku.Equals("new", StringComparison.OrdinalIgnoreCase))
        {
            product = productBus.Create();
            isNewItem = true;
        }
        else
        {
            product = productBus.LoadBySku(sku);
            if (product == null)
            {
                product = productBus.Create();
            }
        }

        // we'll read directly from POST data using the Binder
        ModelState.Clear();

        // var dataBinder = new FormVariableBinder(Request, product, prefixes: "Product.");
        // dataBinder.PropertyExclusionList.AddRange( new[] {"Id", "ParentSku", "OldPk" });
        // dataBinder.Unbind();

        Request.UnbindFormVarsToObject(product, "Id,ParentSku,OldPk", "Product.");

        model.Product = product;

        if (!productBus.Validate(model.Product))
        {
            model.ErrorDisplay.MessageAsRawHtml = true;
            model.ErrorDisplay.ShowError(productBus.ValidationErrors.ToHtml());
            return View("ProductEditor",model);
        }

        if (!productBus.Save())
        {
            model.ErrorDisplay.ShowError($"Product not saved: {productBus.ErrorMessage}");
            return View("ProductEditor",model);
        }


        model.Json = JsonSerializationUtils.Serialize(model.Product, formatJsonOutput: true);

        model.ErrorDisplay.ShowSuccess("Product saved.");
        if (isNewItem)
        {
            Response.Headers.Add("Refresh", "1,url=/admin/productmanager/" + WebUtility.UrlEncode(product.Sku));
        }

        return View("ProductEditor",model);
    }

    [HttpGet, Route("/admin/ProductManager/{sku}/copy")]
    public ActionResult CopyItem(string sku)
    {
        var productBusiness = BusinessFactory.GetProductBusiness();
        var prod = productBusiness.CopyProduct(sku);

        return Redirect("/admin/ProductManager/" + WebUtility.UrlEncode(prod?.Sku ?? sku));
    }

    private IActionResult DeleteProduct(ProductEditorViewModel model)
    {
        var productBusiness = BusinessFactory.GetProductBusiness();
        if (!productBusiness.DeleteDirect(model.Product.Id))
        {
            model.ErrorDisplay.ShowError($"Couldn't delete Product: {productBusiness.ErrorMessage}");
            return View("ProductEditor",model);
        }

        var msg = WebUtility.UrlEncode($"Item '{model.Product.Description}' has been deleted.");
        return Redirect($"/admin/productmanager?error-message={msg}&error-icon=info");
    }

    [HttpDelete]
    [Route("/admin/productmanager/product/delete/{productId}")]
    public JsonResult DeleteProduct(string productId)
    {
        var productBusiness = BusinessFactory.GetProductBusiness();
        if (!productBusiness.DeleteDirect(productId))
            return JsonError($"Couldn't delete Product: {productBusiness.ErrorMessage}");

        return Json(new ApiError("Product has been deleted") { isError = false });
    }

    #region Product Upload

    [HttpPost]
    [Route("/admin/productmanager/image/{sku}")]
    public ApiResponse UploadImageFile(string sku,[FromQuery] string imageName)
    {
        var result = new ApiResponse();

        if (Request.Form.Files.Count < 1)
        {
            result.SetError("No image file uploaded.");
        }

        var productBusiness = BusinessFactory.GetProductBusiness();
        var product = productBusiness.LoadBySku(sku);
        if (product == null)
        {
            result.SetError("Invalid product Sku.");
            return result;
        }

        if (imageName.Contains("/") || imageName.Contains("\\") || !imageName.Contains("."))
        {
            result.SetError("Invalid file name.");
            return result;
        }

        var ext = Path.GetExtension(imageName)?.ToLower();
        if (ext != ".png" && ext != ".jpg" | ext != ".gif")
        {
            result.SetError("Only .png, .jpg and .gif image types are allowed.");
            return result;
        }

        if (string.IsNullOrEmpty(imageName))
        {
            imageName = product.Sku?.Trim() + ".png";
        }

        var filename = Path.Combine(wsApp.Constants.StartupFolder, "wwwroot", "images", "product-images", imageName);
        var file = Request.Form.Files[0];

        try
        {
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                file.CopyTo(fs);
            }
        }
        catch (Exception ex)
        {
            result.SetError($"Upload failed: {ex.Message}" );
            return result;
        }

        result.Message = "Ok";

        OptimizePngImage(filename);

        return result;
    }

    [HttpPost]
    [Route("/admin/productmanager/image/paste/{sku}")]
    public async Task<ApiResponse> UploadImagePaste(string sku, [FromQuery] string imageName)
    {
        var result = new ApiResponse();

        var filename = Path.Combine(wsApp.Constants.StartupFolder, "wwwroot", "images", "product-images", imageName);
        var filedata = await Westwind.AspNetCore.Extensions.HttpRequestExtensions.GetRawBodyStringAsync(Request);

        filedata = filedata.Substring(filedata.IndexOf(',') +1);
        var fileBytes = Convert.FromBase64String(filedata);

        try
        {
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                fs.Write(fileBytes);
            }
        }
        catch (Exception ex)
        {
            result.SetError($"Upload failed: {ex.Message}" );
            return result;
        }

        result.Message = "Ok";

        OptimizePngImage(filename);

        return result;
    }


    /// <summary>
    /// Tries to optimize png images in the background
    /// This is not fast and does not happen right away
    /// so generally this can be applied when images are saved.
    /// </summary>
    /// <param name="pngFilename">Filename to optimize</param>
    /// <param name="level">Optimization Level from 1-9</param>
    public static void OptimizePngImage(string pngFilename, int level = 9)
    {
        wsImageUtils.ResizeImage(pngFilename, pngFilename, 970, 800);

        try
        {
            var filename = Path.Combine(wsApp.Constants.StartupFolder, "pingo.exe");
            var pi = new ProcessStartInfo(filename,
                $"-auto \"{pngFilename}\"");

            //pi.WindowStyle = ProcessWindowStyle.Hidden;
            pi.WorkingDirectory = wsApp.Constants.StartupFolder;
            var p = Process.Start(pi);
        }
        catch
        {
        }
    }
    #endregion
}

public class ProductEditorViewModel : WebStoreBaseViewModel
{
    public string Json { get; set;  }

    public Product Product { get; set; }

}

public class ProductEditorListViewModel : WebStoreBaseViewModel
{
    public List<Product> ProductList { get; set; } = new List<Product>();

    public int ProductCount { get; set; }
    public string SearchTerm { get; set; }

    public bool ShowNonActive { get; set; }
}

