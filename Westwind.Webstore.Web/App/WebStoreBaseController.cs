using System;
using System.Globalization;
using System.IO;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Westwind.AspNetCore;
using Westwind.AspNetCore.Security;
using Westwind.Webstore.Web.Models;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.AspNetCore.Localization;
using Westwind.Utilities;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;

namespace Westwind.Webstore.Web.Controllers
{
    public class WebStoreBaseController : BaseController<UserState>
    {

        /// <summary>
        /// Persistent application data that is passed through with cookie
        /// state.
        /// </summary>
        public WebStoreAppUserState AppUserState { get; set; }

        /// <summary>
        /// Keep track of initial state so we don't write out cookies when nothing has changed
        /// </summary>
        private string _initialAppUserState = null;

        private const string CookieName = "ww_ws_us";

        protected override void Initialize(ActionExecutingContext context)
        {
            ViewBag.ErrorDisplay = ErrorDisplay;
            CreateUserState();

            // var cult = context.HttpContext.Features.Get<IRequestCultureFeature>();
            //
            // cult.RequestCulture = new RequestCulture();
            // cult.RequestCulture.Culture = new CultureInfo("en-us");
            // cult.RequestCulture.Culture  = new CultureInfo(cult.RequestCulture.Culture.LCID);
            //
            // // Need to do this dynamically at runtime
            // cult.RequestCulture.Culture  .NumberFormat.CurrencySymbol = wsApp.Configuration.CurrencySymbol;
        }

        public override void OnActionExecuted(ActionExecutedContext context) {
            base.OnActionExecuted(context);
            PersistUserState();
        }

        protected void CreateUserState()
       {
            // Create UserState
            var rawCookie = HttpContext.Request.Cookies[CookieName];

            if (string.IsNullOrEmpty(rawCookie))
            {
                AppUserState = new WebStoreAppUserState(HttpContext.User);
            }
            else
            {
                try
                {
                    _initialAppUserState = Encryption.DecryptString(rawCookie, wsApp.Constants.EncKey, true);

                    //_initialAppUserState = DataProtector.UnProtect(rawCookie);

                    AppUserState = UserState.CreateFromString<WebStoreAppUserState>(_initialAppUserState);
                    if (AppUserState == null)
                        AppUserState = new WebStoreAppUserState(HttpContext.User);
                }
                catch
                {
                    AppUserState = new WebStoreAppUserState(HttpContext.User);
                }
            }
            UserState = AppUserState;
        }

        private void PersistUserState()
        {
            // Persist UserState in Cookie if state has changed
            var updatedUserState = AppUserState.ToString();
            if (updatedUserState != _initialAppUserState)
            {
                var rawCookie = Encryption.EncryptString(updatedUserState, wsApp.Constants.EncKey, true);
                //var rawCookie = DataProtector.Protect(updatedUserState);
                //var rawCookie = updatedUserState;

                HttpContext.Response.Cookies.Delete("CookieName");

                var cookieTimeoutDays = !AppUserState.IsAdmin ? wsApp.Configuration.System.CookieTimeoutDays :5;

                HttpContext.Response.Cookies.Append("CookieName", rawCookie, new CookieOptions
                {
                     SameSite = SameSiteMode.Strict,
                     HttpOnly = true,
                     Expires = DateTimeOffset.UtcNow.AddDays(cookieTimeoutDays)
                });
            }
        }


        ///// <summary>
        ///// Initialize the AppUser that holds all relevant security state
        ///// from various sources
        ///// </summary>
        ///// <param name="context"></param>
        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    AppUser = HttpContext.User.GetAppUser();
        //    AppUser.UserState = UserState;

        //}

        protected override TViewModel CreateViewModel<TViewModel>()
        {
            var model = base.CreateViewModel<TViewModel>();

            var webStoreModel = model as WebStoreBaseViewModel;
            if (webStoreModel != null)
            {
                webStoreModel.UserState = AppUserState;
                webStoreModel.AppUserState = AppUserState as WebStoreAppUserState;
            }
            webStoreModel.User = HttpContext.User as ClaimsPrincipal;

            return model;
        }

        /// <summary>
        /// Updates a ViewModel and adds values to some of the
        /// stock properties of the Controller.
        ///
        /// This default implementation initializes the ErrorDisplay and UserState
        /// objects after creation.
        /// </summary>
        protected override void InitializeViewModel(BaseViewModel model)
        {
            base.InitializeViewModel(model);

            var webStoreModel = model as WebStoreBaseViewModel;
            if (webStoreModel != null)
            {
                webStoreModel.UserState = AppUserState;
                webStoreModel.AppUserState = AppUserState as WebStoreAppUserState;
            }
            webStoreModel.User = HttpContext.User as ClaimsPrincipal;
        }

        public void SetAppUserFromCustomer(Customer customer)
        {
            AppUserState.Name = customer.UserDisplayName;
            AppUserState.Email = customer.Email;
            AppUserState.UserId = customer.Id;
            AppUserState.IsAdmin = customer.IsAdminUser;

            if (string.IsNullOrEmpty(customer.TwoFactorKey))
            {
                AppUserState.IsTwoFactorValidated = true;
            }
            // otherwise we need to validate the user with the two-factor page
            // after the initial sign in
        }

        public void ClearAppUser()
        {
            AppUserState.Clear();
            AppUserState.IsAuthenticated();
            AppUserState.IsTwoFactorValidated = false;
        }
    }

}
