using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Westwind.AspNetCore;
using Westwind.AspNetCore.Components;
using Westwind.AspNetCore.Security;
using Westwind.Webstore.Business;

namespace Westwind.Webstore.Web.Models
{
    public class WebStoreBaseViewModel : BaseViewModel<WebStoreAppUserState>
    {
        public WebStoreConfiguration Configuration { get; } = wsApp.Configuration;

        
        public WebStoreBaseViewModel()
        {
            ErrorDisplay = new ErrorDisplayModel();
        }

        public WebStoreBaseViewModel(WebStoreAppUserState userState)
        {
            UserState = userState;
        }


    }
}
