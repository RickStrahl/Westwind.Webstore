using Westwind.AspNetCore;
using Westwind.Webstore.Web.Models;
using Westwind.Webstore.Business.Entities;

namespace Westwind.Webstore.Web.Controllers
{
    public class WebStoreBaseController : BaseController<WebStoreAppUserState>
    {

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
