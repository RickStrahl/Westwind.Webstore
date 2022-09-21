//using System;
//using System.Security.Claims;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Westwind.AspNetCore.Security;
//using Westwind.Webstore.Business.Entities;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;

//namespace Westwind.Webstore.Web.Models
//{

//    public class xAppUser : AppUserBase
//    {


//        public AppUser(ClaimsPrincipal user, AppCookies cookies) : base(user)
//        {
//            Cookies = cookies;
//        }

//        /// <summary>
//        /// Values that are stored in a single serialized cookie. These values
//        /// are tracked before a user is logged in. InvoiceId...
//        /// </summary>
//        public AppCookies Cookies { get; set; }


//        /// <summary>
//        /// Customer Id 
//        /// </summary>
//        public Guid UserId
//        {
//            get
//            {
//                Guid userId = Guid.Empty;

//                var strId = GetClaim("UserId");
//                if (!string.IsNullOrEmpty(strId))
//                    Guid.TryParse(strId, out userId);

//                return userId;
//            }
//        }


//        /// <summary>
//        /// Login name: Email address in this application
//        /// </summary>
//        public string Username => GetClaim("Username");


//        /// <summary>
//        /// User display name - first name
//        /// </summary>
//        public string UserDisplayname => GetClaim("Username");

//        /// <summary>
//        /// User's email address - same as Username in this app
//        /// </summary>
//        public string Email => GetClaim("Email");

//        /// <summary>
//        /// Working invoice to track shopping cart. This value comes
//        /// from AppCookies which are separately stored as you can
//        /// add items before being logged in.
//        /// </summary>
//        public Guid InvoiceId
//        {
//            get
//            {
//                return Cookies.InvoiceId;       
//            }
//            set
//            {
//                Cookies.InvoiceId = value;
//            }
//        }


//        /// <summary>
//        /// Determines whether the current user is authenticated. Checks
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        public override bool IsAuthenticated()
//        {
//            if (!base.IsAuthenticated() || string.IsNullOrEmpty(Username))
//                return false;

//            return true;
//        }


//        public bool IsAdmin
//        {
//            get
//            {
//                var admin = GetClaim("IsAdmin");
//                if (admin == "true")
//                    return true;
//                return false;
//            }
//        }

//        public static ClaimsIdentity GetClaimsIdentityFromCustomer(Customer cust )
//        {           
//            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
//            if (cust != null)
//            {

//                identity.AddClaim(new Claim("Email", cust.Email));
//                identity.AddClaim(new Claim("Username", cust.UserDisplayName));
//                identity.AddClaim(new Claim("UserId", cust.Id.ToString()));
//                identity.AddClaim(new Claim("IsAdmin", "true"));
//                if (cust.IsAdminUser)
//                    identity.AddClaim(new Claim(ClaimTypes.Role, RoleNames.Administrators));
//            }

//            return identity;
//        }
//    }

//    public static class ClaimsPrincipalExtensions
//    {
//        public static AppUser GetAppUser(this ClaimsPrincipal user, HttpContext context = null)
//        {
//            return new AppUser(user, new AppCookies(context)) { HttpContext = context };
//        }
//        public static AppUser GetAppUser(this HttpContext context)
//        {
//            var appCookies = AppCookies.Read(context);
//            return new AppUser(context.User, appCookies) { HttpContext = context };
//        }

//    }

//    public struct RoleNames
//    {

//        public const string Administrators = "Administrators";

//    }
//}

