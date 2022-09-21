using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Westwind.AspNetCore;
using Westwind.AspNetCore.Components;
using Westwind.AspNetCore.Security;
using Westwind.Webstore.Business;

namespace Westwind.Webstore.Web.Models
{
    public class WebStoreBaseViewModel : BaseViewModel
    {
        public WebStoreConfiguration Configuration { get; } = wsApp.Configuration;

        /// <summary>
        /// Hold information about the currently logged in user if any
        /// </summary>
        public UserState UserState { get; set;  }

        public WebStoreAppUserState AppUserState {get; set;}

        public ClaimsPrincipal User { get; set; }


        public WebStoreBaseViewModel()
        {
            ErrorDisplay = new ErrorDisplayModel();
        }

        public WebStoreBaseViewModel(UserState userState)
        {
            UserState = userState;
            AppUserState = userState as WebStoreAppUserState;            
        }


    }
}
