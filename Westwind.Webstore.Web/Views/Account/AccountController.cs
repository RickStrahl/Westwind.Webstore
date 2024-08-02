using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Braintree;
using Google.Authenticator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Westwind.AspNetCore.Errors;
using Westwind.AspNetCore.Extensions;
using Westwind.Utilities;
using Westwind.Utilities.Data.Security;
using Westwind.WebStore.App;
using Westwind.Webstore.Business;
using Westwind.Webstore.Web.App;
using Westwind.Webstore.Web.Controllers;
using Westwind.Webstore.Web.Models;
using Customer = Westwind.Webstore.Business.Entities.Customer;


namespace Westwind.Webstore.Web.Views
{
    public class AccountController : WebStoreBaseController
    {
        public BusinessFactory BusinessFactory { get; }

        public AccountController(BusinessFactory businessFactory)
        {
            BusinessFactory = businessFactory;
        }

        #region Authentication

        /// <summary>
        /// igns in a user via the UI.
        ///
        /// Optionally poss `isTokenRequest=true&tokenId={yourId}&scope=WebSurge`
        /// The token id can be used to retrieve a token that was generated
        /// after authorization.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Signin(SigninViewModel model)
        {
            if (model == null)
                model = CreateViewModel<SigninViewModel>();
            else
                InitializeViewModel(model);

            if (model.IsTokenRequest && model.UserState.IsAuthenticated())
            {
                var tokenManager = new UserTokenManager(wsApp.Configuration.ConnectionString);
                model.TokenId = Request.Query["tokenId"];
                var userToken = tokenManager.CreateNewToken(model.UserState.UserId, tokenIdentifier: model.TokenId);

                // go to return url if provided
                if (!string.IsNullOrEmpty(model.TokenReturnUrl))
                    return Redirect(model.TokenReturnUrl +
                                    (model.TokenReturnUrl.Contains("?") ? "&" : "?") +
                                    $"userToken={userToken}&tokenId={model.TokenId}");

                // display on page
                return Redirect($"~/account/UserToken?scope={model.Scope}&userToken={userToken}&tokenId={model.TokenId}");
            }

            return View("SignIn", model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult SignIn(SigninViewModel model)
        {
            InitializeViewModel(model);

            // won't read both from Form and Query - so manually
            model.IsTokenRequest = !string.IsNullOrEmpty(Request.Query["IsTokenRequest"]);
            model.TokenId = Request.Query["tokenId"];
            model.TokenReturnUrl = Request.Query["tokenReturnUrl"];
            model.Scope = Request.Query["Scope"];

            if (!ModelState.IsValid)
            {
                model.ErrorDisplay.AddMessages(ModelState);
                model.ErrorDisplay.ShowError("Please correct the following:");
                return View(model);
            }

            var customerBusiness = BusinessFactory.GetCustomerBusiness();
            var customer = customerBusiness.AuthenticateAndRetrieveUser(model.Email, model.Password);
            if (customer == null)
            {
                model.ErrorDisplay.ShowError(customerBusiness.ErrorMessage);
                return View(model);
            }

            // log user in - may have to provide two-factor as well
            SetAppUserFromCustomer(customer);

            if (wsApp.Configuration.Security.UseTwoFactorAuthentication &&
                !string.IsNullOrEmpty(customer.TwoFactorKey))
            {
                var url = $"~/account/twofactor?ReturnUrl={model.ReturnUrl}";
                if (model.IsTokenRequest)
                    url += $"&isTokenRequest=true&scope={model.Scope}&tokenId={model.TokenId}&tokenReturnUrl={model.TokenReturnUrl}";
                return Redirect(url);
            }

            if (model.IsTokenRequest)
            {
                var tokenManager = new UserTokenManager(wsApp.Configuration.ConnectionString)
                {
                    TokenTimeoutSeconds = 60 * 60 * 60 * 24 * 7
                };
                var userToken = tokenManager.CreateNewToken(customer.Id, null, tokenIdentifier: model.TokenId);


                // go to return url if provided
                if (!string.IsNullOrEmpty(model.TokenReturnUrl))
                    return Redirect(model.TokenReturnUrl +
                                    (model.TokenReturnUrl.Contains("?") ? "&" : "?") +
                                    $"userToken={userToken}&tokenId={model.TokenId}");

                // display on page
                return Redirect($"~/account/UserToken?scope={model.Scope}&userToken={userToken}&tokenId={model.TokenId}");
            }

            if (!string.IsNullOrEmpty(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return Redirect("/");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Signout()
        {
            ClearAppUser();

            TempData["DisplayMessage"] = "You've been signed out.";
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("~/");
        }

        [HttpGet]
        public IActionResult UserToken([FromQuery] string userToken, [FromQuery] string scope)
        {
            var model = new UserTokenModel { Token = userToken, Scope = scope };
            InitializeViewModel(model);

            return View( model );
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/isAuthenticated")]
        public bool IsAuthenthenticated()
        {
            return UserState.IsAuthenticated() || User.Identity.IsAuthenticated;
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("api/isAuthenticated/{userToken}")]
        public bool IsAuthenthenticatedToken(string userToken)
        {
            return UserState.IsAuthenticated() || User.Identity.IsAuthenticated;
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Info()
        {
            var model = CreateViewModel<ProfileViewModel>();
            return View(model);
        }

        #endregion

        #region TwoFactor Authentication

        [Route("/account/setuptwofactor")]
        [HttpGet]
        [HttpPost]
        public IActionResult SetupTwoFactor(string task, SetupTwoFactorViewModel model)
        {
            task = task ?? string.Empty;

            if (model == null)
                model = CreateViewModel<SetupTwoFactorViewModel>();
            else
                InitializeViewModel(model);

            if (!UserState.IsAuthenticated())
                return RedirectToAction("Profile", "Account");

            var customerBus = BusinessFactory.GetCustomerBusiness();
            var customer = customerBus.Load(UserState.UserId);

            if (customer == null)
                return RedirectToAction("Profile", "Account");

            // REMOVE operation
            if (task.Equals("remove", StringComparison.InvariantCultureIgnoreCase))
            {
                customer.TwoFactorKey = null;
                customerBus.Save();
                return RedirectToAction("Profile", "Account");
            }

            if (!string.IsNullOrEmpty(customer.TwoFactorKey))
                return RedirectToAction("Profile", "Account");

            var twoFactor = new TwoFactorAuthenticator();
            if (string.IsNullOrEmpty(model.CustomerSecretKey))
                model.CustomerSecretKey = DataUtils.GenerateUniqueId(16);

            var setupInfo = twoFactor.GenerateSetupCode(
                wsApp.Configuration.ApplicationName,
                customer.Email,
                model.CustomerSecretKey,
                false, 10);

            model.TwoFactorSetupKey = setupInfo.ManualEntryKey;
            model.QrCodeImageData = setupInfo.QrCodeSetupImageUrl;

            // Explicitly validate before saving
            if (Request.IsFormVar("btnValidate") && !string.IsNullOrEmpty(model.ValidationKey))
            {
                if (twoFactor.ValidateTwoFactorPIN(model.CustomerSecretKey, model.ValidationKey))
                {
                    customer.TwoFactorKey = model.CustomerSecretKey;
                    if (customerBus.Save())
                    {
                        return RedirectToAction("Profile", "Account");
                    }

                    ErrorDisplay.ShowError("Unable to set up Two Factor Authentication");
                }
                else
                    ErrorDisplay.ShowError("Invalid Validation code.");
            }

            return View(model);
        }

        [Route("/account/twofactor")]
        [HttpGet]
        [HttpPost]
        public IActionResult TwoFactorValidation(TwoFactorValidationViewModel model)
        {
            if (string.IsNullOrEmpty(UserState.UserId))
                return RedirectToAction("Signin");

            if (model == null)
            {
                model = CreateViewModel<TwoFactorValidationViewModel>();
            }
            else
            {
                InitializeViewModel(model);
            }

            model.ReturnUrl = Request.Query["ReturnUrl"];
            model.IsTokenRequest = !string.IsNullOrEmpty(Request.Query["IsTokenRequest"]);
            model.TokenId = Request.Query["TokenId"];
            model.App = Request.Query["App"];
            model.TokenReturnUrl = Request.Query["TokenReturnUrl"];

            if (string.IsNullOrEmpty(model.ReturnUrl))
                model.ReturnUrl = "/";

            if (Request.IsPostback())
            {
                if (string.IsNullOrEmpty(model.ValidationCode))
                {
                    model.ErrorDisplay.ShowError("Please enter a validation code from your Authenticator app.");
                    return View(model);
                }

                var customerBus = BusinessFactory.GetCustomerBusiness();
                var customer = customerBus.Load(UserState.UserId);
                if (customer == null)
                    return RedirectToAction("Signin");

                var twoFactor = new TwoFactorAuthenticator();
                UserState.IsTwoFactorValidated = twoFactor.ValidateTwoFactorPIN(customer.TwoFactorKey, model.ValidationCode.Replace(" ", ""));

                if (UserState.IsTwoFactorValidated)
                {
                    UserState.IsAdmin = customer.IsAdminUser; // reset admin explicitly

                    if (model.IsTokenRequest)
                    {
                        var tokenManager = new UserTokenManager(wsApp.Configuration.ConnectionString);
                        var userToken = tokenManager.CreateNewToken(customer.Id, tokenIdentifier: model.TokenId);

                        // go to return url if provided
                        if (!string.IsNullOrEmpty(model.TokenReturnUrl))
                            return Redirect(model.TokenReturnUrl +
                                (model.TokenReturnUrl.Contains("?") ? "&" : "?") +
                                $"app={model.App}&userToken={userToken}");

                        // display as page
                        return Redirect($"~/account/UserToken?app={model.App}&userToken={userToken}&tokenId={model.TokenId}");
                    }

                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    UserState.IsAdmin = false;
                }

                ErrorDisplay.ShowError("Invalid validation code. Please try again.");
            }

            return View(model);
        }
        #endregion

        #region UserToken Validation


        /// <summary>
        /// Retrieves a user Token based on a token identifier.
        ///
        /// Also set when using /account/signin?
        /// </summary>
        /// <param name="tokenIdentitifier">A previously passed token identifier which is created as part
        /// of the signin</param>
        /// <returns></returns>
        [Route("api/account/usertokenretrieval/{tokenIdentifier}")]
        public object UserTokenRetrievalByTokenIdentifier(string tokenIdentifier)
        {
            var manager = new UserTokenManager(wsApp.Configuration.ConnectionString);
            var userToken =  manager.GetTokenByTokenIdentifier(tokenIdentifier);
            if (userToken == null)
            {
                Response.StatusCode = 404;
                return new ApiException(manager.ErrorMessage, 404);
            }

            return new { userToken = userToken.Id };
        }
        #endregion

        #region Profile Editing and Creation

        [HttpGet]
        [Route("shoppingcart/orderprofile")]
        [Route("account/profile")]
        [Route("profile")]
        [Route("account/profile/new")]
        [AllowAnonymous]
        public ActionResult Profile([FromQuery] string returnUrl = null, string action = null)
        {
            var isNewRequest = Request.Path.Value.Contains("/new");

            var userId = UserState.UserId;

            var model = CreateViewModel<ProfileViewModel>();

            model.IsOrderProfile = HttpContext.Request.Path.Value.Contains("/orderprofile", StringComparison.OrdinalIgnoreCase);
            model.ReturnUrl = returnUrl;

            var customerBusiness = BusinessFactory.GetCustomerBusiness();
            Customer customer;
            if (!isNewRequest && !UserState.IsAuthenticated())
            {
                var redirectUrl = model.IsOrderProfile ? "/shoppingcart/orderprofile" : "/account/profile";
                return Redirect($"/account/signin?returnurl={redirectUrl}");
            }

            if (isNewRequest && !UserState.IsAuthenticated())
            {
                customer = customerBusiness.Create();
                model.IsNewUser = true;
            }
            else
                customer = customerBusiness.Load(userId);

            if (customer == null)
            {
                // invalid id - user is no longer logged in or ID has changed
                return Redirect("~/account/signout");
            }

            // map to view model
            ApplicationMapper.Current.Map(customer, model.Customer);
            model.Customer.UseTwoFactorAuth = !string.IsNullOrEmpty(customer.TwoFactorKey);

            model.BillingAddress = CustomerBusiness.GetBillingAddress(customer);

            return View(model);
        }

        [HttpPost]
        [Route("shoppingcart/orderprofile")]
        [Route("account/profile")]
        [Route("profile")]
        [Route("account/profile/new")]
        [AllowAnonymous]
        public async Task<ActionResult> Profile(ProfileViewModel model)
        {
            var routeData = ControllerContext.RouteData;

            InitializeViewModel(model);

            if (string.IsNullOrEmpty(model.ReturnUrl))
                model.ReturnUrl = HttpContext.Request.Query["ReturnUrl"].FirstOrDefault();

            var userId = UserState.UserId;

            if (string.IsNullOrEmpty(model.Customer.Id) || model.Customer.Id != UserState.UserId)
            {
                model.IsNewUser = true;
                model.Customer.Id = wsApp.NewId();
            }

            var customerBusiness = BusinessFactory.GetCustomerBusiness();

            Customer customer = null;
            if (!model.IsNewUser)
            {
                customer = customerBusiness.Load(model.Customer.Id);
            }

            if (model.IsNewUser || customer == null)
            {
                customer = customerBusiness.Create();
                model.IsNewUser = true;
                customer.IsNew = true;
                customer.Password = model.Password;
                customer.ValidationKey = EmailAddressValidator.GenerateValidationKey();
            }

            string oldEmail = customer.Email;

            // Map model data onto business object for each block captured
            var mapper = ApplicationMapper.Current;
            mapper.Map(model.Customer, customer);

            var address = CustomerBusiness.GetBillingAddress(customer);

            mapper.Map(model.BillingAddress, address);
            //    , opt =>
            //{
            //    opt.BeforeMap((src, target) =>
            //    {
            //        src.Id = target.Id;
            //        src.CustomerId = target.CustomerId;
            //    });
            //});

            // TODO: For now assume first address is billing address
            if (customer.Addresses.Count < 1)
                customer.Addresses.Add(address);

            bool validationResult = customerBusiness.Validate(customer);
            if (customer.IsNew)
            {
                if (string.IsNullOrEmpty(model.Password) ||
                    string.IsNullOrEmpty(model.PasswordConfirm) ||
                    model.Password != model.PasswordConfirm)
                {
                    model.ErrorDisplay.AddMessage(AppResources.Account.PasswordMissingOrdontMatch, "Password");
                    customer.Password = null;
                    validationResult = false;
                }
                else
                {
                    if(!customerBusiness.ValidatePassword(model.Password))
                        validationResult = false;
                }

                customerBusiness.Attach(customer, true);
            }

            // email has changed - validate it
            if (wsApp.Configuration.Security.ValidateEmailAddresses &&
                (string.IsNullOrEmpty(oldEmail) ||
                 (oldEmail != customer.Email)))
            {
                // check email validation code
                var validator = new EmailAddressValidator();
                if (validator.IsCodeValidated(model.Evc, customer.Email, false))
                {
                    customer.ValidationKey = null;
                    customer.IsActive = true;
                }
                else
                {
                    ErrorDisplay.AddMessage("Your email address has not been validated. Please use the validate button on the email field.", "txtEmail");
                    validationResult = false;
                }
            }

            if (!validationResult)
            {
                model.ErrorDisplay.AddMessages(customerBusiness.ValidationErrors, null);
                model.ErrorDisplay.ShowError("Please fix the following");
                return View(model);
            }

            address.Country = address.GetCountryFromCode(address.CountryCode);

            if (!await customerBusiness.SaveAsync())
                model.ErrorDisplay.ShowError(customerBusiness.ErrorMessage,
                    AppResources.Account.CouldntSaveYourProfile);
            else
            {
                var msg = $"{AppResources.Account.ProfileInformationSaved}.";
                model.ErrorDisplay.ShowSuccess(msg);

                SetAppUserFromCustomer(customer); // update AppUser settings

                if (!string.IsNullOrEmpty(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);
            }

            return View(model);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        ///
        [Route("account/orderhistory")]
        [HttpGet, HttpPost]
        public async Task<ActionResult> OrderHistory(string displayMode = null)
        {
            var userId = UserState.UserId;
            if (string.IsNullOrEmpty(userId))
                return Redirect("~/");

            var model = CreateViewModel<OrderHistoryViewModel>();
            model.DisplayMode = displayMode;

            var invoiceBusiness = BusinessFactory.GetInvoiceBusiness();


            if (model.DisplayMode == "All")
                model.InvoiceList = await invoiceBusiness.GetRecentInvoices(userId, 999);
            else if (model.DisplayMode == "Open")
            {
                model.InvoiceList = await invoiceBusiness.GetOpenInvoices(userId);
            }
            else
                // recent
                model.InvoiceList = await invoiceBusiness.GetRecentInvoices(userId, 5);

            var customerBusiness = BusinessFactory.GetCustomerBusiness();
            customerBusiness.Context = invoiceBusiness.Context;
            model.Customer = await customerBusiness.LoadAsync(userId);

            return View(model);
        }

        #endregion


        #region Account and Email Validation and Recovery

        [AllowAnonymous]
        public ActionResult PasswordRecovery()
        {
            var model = CreateViewModel<PasswordRecoveryModel>();
            return View(model);
        }

        [Route("/account/recover")]
        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        public ActionResult PasswordRecoverySendEmail(string email)
        {
            var model = CreateViewModel<PasswordRecoveryModel>();

            if (Request.Method == "GET")
                return View(model);

            if (string.IsNullOrEmpty(email))
            {
                model.ErrorDisplay.ShowError("Please specify an email address");
                return View(model);
            }

            var customerBusiness = BusinessFactory.GetCustomerBusiness();
            var validationId = customerBusiness.CreateValidationKeyForUser(email);
            if (string.IsNullOrEmpty(validationId))
            {
                model.ErrorDisplay.ShowError("Couldn't create a Validation Id.");
                return View(model);
            }

            var url = wsApp.Configuration.ApplicationHomeUrl.TrimEnd('/') + $"/account/recover/verify/{validationId}";

            string title = $"{wsApp.Configuration.ApplicationName} Password Recovery";
            string body = $@"Hi,

This is your **password recovery email** for your {wsApp.Configuration.ApplicationName} account.

Please visit the [following link]({url}) to recover your password and create a new password for your account.

[Reset your password]({url})

Regards,

**The {wsApp.Configuration.ApplicationCompany} Team**
";

            bool success = AppUtils.SendEmail(
                email, title,
                AppUtils.GetEmbeddedHtmlDocument(AppUtils.MarkdownToHtml(body)),
                out string error, noCCs: true);

            if (!success)
                model.ErrorDisplay.ShowError("Unable to send email: " + error);
            else
            {
                model.ErrorDisplay.ShowSuccess(
                    $"Verification email has been sent. Please check your account for a message from '{wsApp.Configuration.Email.SenderEmail}'.");

                // always log out user and force them to log back in

                ClearAppUser();

                Response.Headers["Refresh"] =  "15;url=" + Url.Action("Signin", "Account");
            }

            return View(model);
        }

        [Route("/account/recover/verify/{validationId}")]
        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        public ActionResult PasswordRecovery(string validationId, PasswordRecoveryModel model)
        {
            InitializeViewModel(model);

            if (string.IsNullOrEmpty(validationId))
                return RedirectToAction("Index", "Home");

            var customerBusiness = BusinessFactory.GetCustomerBusiness();
            var user = customerBusiness.GetCustomerByValidationKey(validationId);

            // invalid id most likely - don't show a message or fail just redirect
            // to provide minimal user feedback on failure. Password won't be changed.
            if (user == null)
            {
                model.ErrorDisplay.ShowError("Invalid validation key.");
                return View(model);
            }

            // display the entry form
            if (Request.Method == "GET")
                return View(model);

            // validate and save new password
            if (model.Password != model.PasswordRecovery)
            {
                model.ErrorDisplay.ShowError("Please make sure your new passwords entered match.");
                return View(model);
            }

            if (!customerBusiness.RecoverPassword(validationId, model.Password))
            {
                model.ErrorDisplay.AddMessages(customerBusiness.ValidationErrors);
                model.ErrorDisplay.ShowError("Couldn't save new password");
            }
            else
            {
                Response.Headers["Refresh"] = "2;url=" + Url.Action("Signin", "Account");
                model.ErrorDisplay.ShowSuccess("Password updated. Sign in with your new password.");

                ClearAppUser();
            }

            return View(model);
        }

        #endregion


        #region Email Validation

        /// <summary>
        /// Keep track of
        /// </summary>
        private static List<EmailValidationLogEntry> EmailValidationLog = new();

        public class EmailValidationLogEntry
        {
            public string Vk { get; set; }
            public DateTime Timestamp { get; set; }

            public override string ToString() => Vk + " " + Timestamp.ToString("HH:mm:ss");
        }


        public class EmailValidationRequest
        {
            public string Email { get; set; }
            public string Vk { get; set; }
        }

        /// <summary>
        /// https://localhost:5001/api/account/validate/send?email=rickstrahl@west-wind.com
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("api/account/validate/send")]
        public ActionResult SendValidation([FromBody] EmailValidationRequest request)
        {
            if (!Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrEmpty(request?.Email) ||
                !CheckEmailRequestForFraud(request.Vk) )
            {
                Response.StatusCode = 404;
                return Json(new { isError = true, message = "No email address was provided." });
            }

            string email = request.Email;

            if (string.IsNullOrEmpty(email))
            {
                return Json(new { isError = true, message = "No email address was provided." });
            }

            var validator = new EmailAddressValidator();
            string validationKey = validator.GenerateCode(email);
            if (string.IsNullOrEmpty(validationKey))
            {
                return Json(new { isError = true, message = "Invalid email address." });
            }

            string title = $"{wsApp.Configuration.ApplicationName} Email Validation";
            string body = $@"Hi,

This email is to confirm your email address **{email}** for the **{wsApp.Configuration.ApplicationName}**.

Your email validation code is:

<div style='background: #eee padding: 1em; margin: 0 auto'>

## {validationKey}

</div>

Please copy the code, return to the Web browser and enter the validation code to verify your email address.

Regards,

The {wsApp.Configuration.ApplicationCompany} Team
";
            var html = AppUtils.GetEmbeddedHtmlDocument(AppUtils.MarkdownToHtml(body));
            bool result = AppUtils.SendEmail(email, title, html, out string errorMsg);

            return Json(new { isError = !result, message = errorMsg });
        }

        [Route("api/account/validate/{validationId}")]
        [AllowAnonymous]
        public ActionResult ValidateEmail(string validationId, [FromQuery] string email, [FromQuery] string id)
        {
            var validator = new EmailAddressValidator();
            bool result = validator.ValidateCode(validationId, email);

            if (!result || string.IsNullOrEmpty(id))
                return Json(new { isValidated = result, message = validator.ErrorMessage });

            // also check to see if the email address already exists
            var customerBus = BusinessFactory.GetCustomerBusiness();
            var customer = customerBus.Load(id);
            if (customer == null)
            {
                customer = customerBus.CreateEmpty();
                customer.Email = email;
            }

            result = customerBus.ValidateEmailAddress(customer.IsNew, customer);
            return Json(new { isValidated = result, message = customerBus.ValidationErrors.ToString()});
        }


        /// <summary>
        /// Check for fraud by storing to a dictionary and checking for repeated use
        /// and too fast processing.
        /// </summary>
        /// <param name="vkKey"></param>
        /// <returns></returns>
        private bool CheckEmailRequestForFraud(string vkKey, int listSize = 2000, double minuteTimeout = 5F)
        {
            if (string.IsNullOrEmpty(vkKey) || vkKey != UserState.SecurityToken)
                return false;

            var list = EmailValidationLog.Where(l => l.Timestamp > DateTime.UtcNow.AddMinutes( minuteTimeout * -1))
                .OrderByDescending(t => t.Timestamp)
                .ToList();

            // Console.WriteLine(list.Count);

            if (list.Count > 0)
            {
                if (list.Count > 5)
                {
                    //Console.WriteLine("More than 5!");
                    return false;
                }

                var le = list.FirstOrDefault(l => l.Vk == vkKey);

                if (le != null && le.Timestamp > DateTime.UtcNow.AddSeconds(-35))
                {
                    //Console.WriteLine("Time out! " + le);
                    return false;
                }
            }

            // Trim list to size
            if (EmailValidationLog.Count > listSize)
            {
                lock (EmailValidationLog)
                {
                    if (list.Count > listSize)
                        EmailValidationLog.RemoveRange(0, 500);
                }
            }

            // add new entry for each successful attempt
            EmailValidationLog.Add(new EmailValidationLogEntry()
            {
                Vk = vkKey,
                Timestamp = DateTime.UtcNow
            });

            //Console.WriteLine("No Fraud");
            return true;
        }

        #endregion



    }

    public class UserTokenModel : WebStoreBaseViewModel
    {
        public string Token { get; set; }
        public string Scope { get; set;  }

        public string TokenId { get; set; }
    }
}
