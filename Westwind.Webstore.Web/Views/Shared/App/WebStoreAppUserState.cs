using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Westwind.AspNetCore.Security;
using Westwind.Webstore.Business.Entities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Westwind.Webstore.Business;

namespace Westwind.Webstore.Web.Models
{

    public class WebStoreAppUserState : UserState
    {
        //public ClaimsPrincipal User;

        /// <summary>
        /// Parameterless constructor required for serialization
        /// </summary>
        public WebStoreAppUserState()
        {

        }

        public WebStoreAppUserState(ClaimsPrincipal user)
        {
            //User = user;
        }

        /// <summary>
        /// Working invoice to track shopping cart. This value comes
        /// from AppCookies which are separately stored as you can
        /// add items before being logged in.
        /// </summary>
        public string InvoiceId { get; set; }

        public int CartItemCount { get; set; }

        public bool IsTwoFactorValidated { get; set; }


        public string LanguageId { get; set; } = "en";


        /// <summary>
        /// Sets invoice related settings on the WebStoreUserState.
        /// You can also pass null to clear the items from userstate.
        /// </summary>
        /// <param name="invoice"></param>
        public void SetInvoiceSettings(Invoice invoice)
        {
             if (invoice != null)
            {
                InvoiceId = invoice.Id;
                CartItemCount = invoice.LineItems.Count;
            }
            else
            {
                InvoiceId = null;
                CartItemCount = 0;
            }
        }

        /// <summary>
        /// Determines whether the user is logged
        /// in or an anonymous user.
        /// </summary>
        /// <returns></returns>
        public override bool IsAuthenticated()
        {
            if (!wsApp.Configuration.Security.UseTwoFactorAuthentication)
                return !string.IsNullOrEmpty(UserId);
            else
            {
                return !string.IsNullOrEmpty(UserId) && IsTwoFactorValidated;
            }
        }


        /// <summary>
        /// Resets the value to the defaults
        /// </summary>
        public override void Clear()
        {
            base.Clear();

            InvoiceId = null;
            CartItemCount = 0;
        }
    }



}

