using System.Globalization;
using Microsoft.AspNetCore.Mvc.Filters;
using Westwind.AspNetCore;
using Westwind.Webstore.Web.Models;
using Westwind.Webstore.Business.Entities;
using Westwind.Webstore.Business;

namespace Westwind.Webstore.Web.Controllers
{
    public class WebStoreBaseController : BaseController<WebStoreAppUserState>
    {
        /// <summary>
        /// Handle Language Selection based on UserState LanguageId
        /// UserState.LanguageId is set from the customer record
        /// when logging in and/or from a potential drop down with
        /// language code selections (if implemented).
        ///
        /// If not using localization all of this still works fine as
        /// the default language code is used.
        /// </summary>
        /// <param name="context">Contrller Active Context passed for base behavior</param>
        protected override void Initialize(ActionExecutingContext context)
        {
            // UserState created here (if available)
            base.Initialize(context);

            if (!string.IsNullOrEmpty(UserState?.LanguageId))
            {
                if (!string.IsNullOrEmpty(UserState.LanguageId))
                {
                    try
                    {
                        // Override User Language and currency settings
                        var culture = new CultureInfo(UserState.LanguageId);
                        culture.NumberFormat.CurrencySymbol = wsApp.Configuration.CurrencySymbol;
                        CultureInfo.CurrentCulture = culture;
                        CultureInfo.CurrentUICulture = culture;
                        
                    }
                    catch
                    {
                        /* default culture */
                    }
                }
            }
        }

        protected override TViewModel CreateViewModel<TViewModel>()
        {
            var model = base.CreateViewModel<TViewModel>();

            var webStoreModel = model as WebStoreBaseViewModel;
            if (webStoreModel != null)
            {
                webStoreModel.UserState = UserState;
            }

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
                webStoreModel.UserState = UserState;
            }

        }

        public void SetAppUserFromCustomer(Customer customer)
        {
            UserState.Name = customer.UserDisplayName;
            UserState.Email = customer.Email;
            UserState.UserId = customer.Id;
            UserState.IsAdmin = customer.IsAdminUser;
            UserState.LanguageId = customer.LanguageId;

            if (string.IsNullOrEmpty(customer.TwoFactorKey))
            {
                UserState.IsTwoFactorValidated = true;
            }
            // otherwise we need to validate the user with the two-factor page
            // after the initial sign in
        }

        public void ClearAppUser()
        {
            UserState.Clear();
            UserState.IsAuthenticated();
            UserState.IsTwoFactorValidated = false;
        }
    }

}
