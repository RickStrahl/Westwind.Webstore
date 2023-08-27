using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Westwind.AspNetCore.Errors;
using Westwind.AspNetCore.Messages;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;
using Westwind.Webstore.Web.Controllers;
using Westwind.Webstore.Web.Models;

namespace Westwind.Webstore.Web.Views.LookupsManager;

public class LookupsManagerController : WebStoreBaseController
{
    public BusinessFactory Factory { get; }

    public LookupsManagerController(BusinessFactory factory)
    {
        Factory = factory;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        base.OnActionExecuting(context);

        if (!UserState.IsAdmin)
        {
            context.HttpContext.Response.Redirect("/account/signin?returnurl=" + Request.Path);
            await context.HttpContext.Response.CompleteAsync();
            return;
        }

        await next();
    }


    [HttpGet, Route("/admin/lookupsmanager")]
    public IActionResult Index([FromQuery] string key = null)
    {
        var model = CreateViewModel<LookupsManagerViewModel>();

        var lookups = Factory.GetLookupBusiness();
        model.LookupItems = lookups.GetAllItems(noStateOrCountry:true, key: key);

        return View("LookupsEditor", model);
    }

    #region Editing API requests

    [HttpDelete]
    [Route("/admin/lookupsmanager/{id}")]
    public ApiResponse DeleteLookupItem(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ApiException("Invalid lookup item id.");

        var lookups = Factory.GetLookupBusiness();
        if (!lookups.DeleteDirect(id))
        {
            throw new ApiException("Couldn't delete lookups item: " + lookups.ErrorMessage);
        }

        return new ApiResponse {  Message = "Item deleted." };
    }

    [HttpPost,Route("/admin/lookupsmanager")]
    public Lookup SaveLookupItem([FromBody] Lookup model)
    {
        var lookups = Factory.GetLookupBusiness();

        Lookup lookup ;

        if (model == null)
            throw new ApiException("Missing lookup item to update.");

        if (string.IsNullOrEmpty(model.Id) ||
            model.Id.Equals("new", StringComparison.OrdinalIgnoreCase))
        {
            lookup = lookups.Create();
            if (string.IsNullOrEmpty(model.Key))
                throw new ApiException("Please provide a key for your lookup item.");
        }
        else
        {
            lookup = lookups.Load(model.Id);
            if (lookup == null)
                throw new ApiException("Invalid lookup item id to update.");
        }

        lookup.CData1 = model.CData1;
        lookup.CData = model.CData;
        lookup.NData = model.NData;
        if (!string.IsNullOrEmpty(model.Key))
            lookup.Key = model.Key;

        if (!lookups.Save(lookup))
            throw new ApiException("Couldn't save lookup item: " + lookups.ErrorMessage);

        return lookup;
    }

    #endregion
}

public class LookupsManagerViewModel : WebStoreBaseViewModel
{
    public List<Lookup> LookupItems { get; set; } = new();
}

