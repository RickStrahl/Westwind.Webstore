using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Westwind.AspNetCore.Security;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Westwind.Utilities;
using Westwind.Utilities.Data;
using Westwind.WebStore.App;
using Westwind.Webstore.Business;
using Westwind.Webstore.Business.Entities;
using Westwind.Webstore.Web.App;
using Westwind.Webstore.Web.Controllers;
using Westwind.Webstore.Web.Views;


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

        [HttpGet]
        public ActionResult Signin()
        {
            var model = CreateViewModel<SigninViewModel>();
            model.ReturnUrl = Request.Query["ReturnUrl"];
            return View("SignIn", model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult SignIn(SigninViewModel model)
        {
            InitializeViewModel(model);

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

            SetAppUserFromCustomer(customer);

            if (!string.IsNullOrEmpty(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return Redirect("~/");
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

        [AllowAnonymous]
        [HttpGet]
        [Route("api/isAuthenticated")]
        public bool IsAuthenthenticated()
        {
            return User.Identity.IsAuthenticated;
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("api/isAuthenticated/{userToken}")]
        public bool IsAuthenthenticatedToken(string userToken)
        {
            return User.Identity.IsAuthenticated;
        }



        [AllowAnonymous]
        [HttpGet]
        public IActionResult Info()
        {
            var model = CreateViewModel<ProfileViewModel>();
            return View(model);
        }

        #endregion

        #region Profile Editing and Creation

        [Route("account/profile/new")]
        [AllowAnonymous]
        public ActionResult NewProfile()
        {
            return Profile();
        }

        [HttpGet]
        [Route("shoppingcart/orderprofile")]
        [Route("account/profile")]
        public ActionResult Profile([FromQuery] string returnUrl = null)
        {
            var userId = AppUserState.UserId;

            var model = CreateViewModel<ProfileViewModel>();
            model.IsOrderProfile =
                HttpContext.Request.Path.Value.Contains("/orderprofile", StringComparison.OrdinalIgnoreCase);
            model.ReturnUrl = returnUrl;

            var customerBusiness = BusinessFactory.GetCustomerBusiness();
            Customer customer;
            if (!AppUserState.IsAuthenticated())
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
            model.BillingAddress = CustomerBusiness.GetBillingAddress(customer);

            return View(model);
        }

        [HttpPost]
        [Route("shoppingcart/orderprofile")]
        [Route("account/profile")]
        public async Task<ActionResult> Profile(ProfileViewModel model)
        {
            var routeData = ControllerContext.RouteData;

            InitializeViewModel(model);
            if (string.IsNullOrEmpty(model.ReturnUrl))
                model.ReturnUrl = HttpContext.Request.Query["ReturnUrl"].FirstOrDefault();

            var userId = AppUserState.UserId;

            if (string.IsNullOrEmpty(model.Customer.Id) || model.Customer.Id != AppUserState.UserId)
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
                    ErrorDisplay.AddMessage("Your email address has not been validated. Please use the validate button on the email field.", "Email");
                }
            }

            if (!validationResult)
            {
                model.ErrorDisplay.AddMessages(customerBusiness.ValidationErrors, null);
                model.ErrorDisplay.ShowError("Please fix the following");
                return View(model);
            }

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
            var userId = AppUserState.UserId;
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


        #region Email Validation and Recovery

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

                Response.Headers.Add("Refresh", "15;url=" + Url.Action("Signin", "Account"));
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
                Response.Headers.Add("Refresh", "2;url=" + Url.Action("Signin", "Account"));
                model.ErrorDisplay.ShowSuccess("Password updated. Sign in with your new password.");

                ClearAppUser();
            }

            return View(model);
        }

        #endregion

        #region Email Validation

        /// <summary>
        /// https://localhost:5001/api/account/validate/send?email=rickstrahl@west-wind.com
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("api/account/validate/send")]
        public ActionResult SendValidation([FromQuery] string email)
        {
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

        #endregion

    }
}
