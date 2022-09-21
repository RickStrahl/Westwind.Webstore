//using System;
//using Microsoft.AspNetCore.Http;
//using Westwind.Utilities;

//namespace Westwind.Webstore.Web.Models
//{
//    public class AppCookies
//    {

//        private const string STR_COOKIE_NAME = "ww_ws_ck";

//        internal HttpContext HttpContext;


//        public AppCookies(HttpContext context)
//        {
//            HttpContext = context;
//        }

//        public AppCookies()
//        { }

//        private bool HasChanged = false;


//        // *** Add properties here that will be persisted in a single cookie
//        //     Any change to the values will write out a new cookie.

//        public Guid InvoiceId
//        {
//            get
//            {
//                return _InvoiceId;
//            }
//            set
//            {
//                if (_InvoiceId == value) return;
//                _InvoiceId = value;
//                HasChanged = true;
//            }            
//        }
//        private Guid _InvoiceId;


        
//        public void Write()
//        {
//            if (HasChanged)
//            {
//                var cookieValue = StringSerializer.SerializeObject(this);
//                SetCookie(STR_COOKIE_NAME, cookieValue);
//            }
//        }

//        public static AppCookies Read(HttpContext context)
//        {

//            if (context.Request.Cookies.Keys.Contains(STR_COOKIE_NAME))
//                return new AppCookies(context);

//            var data = context.Request.Cookies[STR_COOKIE_NAME];
//            if (string.IsNullOrEmpty(data))
//                return new AppCookies(context);


//            var appCookies = StringSerializer.Deserialize<AppCookies>(data);
//            appCookies.HttpContext = context;
//            return appCookies;
//        }

//        public void SetCookie(string name, string value)
//        {
//            if (HttpContext == null)
//                throw new InvalidOperationException("Can't set cookie in AppUser. Make sure HttpContext is set when you create the AppUser.");

//            var opt = new CookieOptions()
//            {
//                Expires = DateTime.UtcNow.AddDays(2),
//                HttpOnly = true,
//                SameSite = SameSiteMode.Strict
//            };
//            HttpContext.Response.Cookies.Append(name, value);
//        }

//        /// <summary>
//        /// Returns a cookie value or null if it doesn't exist
//        /// </summary>
//        /// <param name="name"></param>
//        /// <returns></returns>
//        public string GetCookie(string name)
//        {
//            if (HttpContext == null)
//                throw new InvalidOperationException("Can't set cookie in AppUser. Make sure HttpContext is set when you create the AppUser.");

//            if (!HttpContext.Request.Cookies.Keys.Contains(name))
//                return null;

//            return HttpContext.Request.Cookies[name];
//        }
//    }
//}

