// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountController.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using System.Web.Mvc;
    using System.Web.Security;

    using App.Security;
    using App.Services;
    using App.Web.Models;

    using Microsoft.AspNet.Membership.OpenAuth;
    using Microsoft.Web.WebPages.OAuth;

    using WebMatrix.WebData;

    [Authorize]
    public class AccountController : Controller
    {
        private readonly IMembershipService membership;

        private readonly IFormsAuthentication formsAuthentication;

        public AccountController(IMembershipService membershipService, IFormsAuthentication formsAuthentication)
        {
            this.membership = membershipService;
            this.formsAuthentication = formsAuthentication;
            this.membership.RegisterModelState(this.ModelState);
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        //// GET: /login

        [AllowAnonymous]
        public ActionResult LogIn(string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;
            return this.View();
        }

        //// POST: /login

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult LogIn(LoginModel model, string returnUrl)
        {
            if (this.ModelState.IsValid && this.membership.ValidateUser(model.UserName, model.Password))
            {
                this.formsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                return this.RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        //// POST: /logout

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            this.formsAuthentication.SignOut();
            return this.RedirectToAction("Index", "Home");
        }

        //// GET: /register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return this.View();
        }

        //// POST: /register

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            App.Data.User user;

            // Attempt to register the user
            if (this.ModelState.IsValid && (user = this.membership.CreateUser(model.UserName, model.Email, model.Password)) != null)
            {
                this.formsAuthentication.SetAuthCookie(user.UserName, true);
                return this.RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        //// POST: /account/disassociate

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            var message = this.membership.RemoveOpenAuthAccount(User.Identity.Name, provider, providerUserId) ?
                          ManageMessageId.RemoveLoginSuccess : (ManageMessageId?)null;

            return this.RedirectToAction("Manage", new { Message = message });
        }

        //// GET: /account/manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : string.Empty;
            ViewBag.HasLocalPassword = this.membership.HasPassword(User.Identity.Name);
            ViewBag.ReturnUrl = Url.Action("Manage");
            return this.View();
        }

        //// POST: /account/manage

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            ViewBag.HasLocalPassword = this.membership.HasPassword(User.Identity.Name);
            ViewBag.ReturnUrl = Url.Action("Manage");

            if (ViewBag.HasLocalPassword)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = this.membership.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return this.RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    this.membership.SetPassword(User.Identity.Name, model.NewPassword);
                    return this.RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                }
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        //// POST: /Account/ExternalLogin

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //// GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            var result = OpenAuth.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));

            if (!result.IsSuccessful)
            {
                return this.RedirectToAction("ExternalLoginFailure");
            }

            if (this.membership.ValidateOpenAuthUser(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return this.RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                this.membership.AddOpenAuthAccount(User.Identity.Name, result.Provider, result.ProviderUserId, result.UserName);
                return this.RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                var loginData = CryptoUtility.Serialize("oauth", result.Provider, result.ProviderUserId, result.UserName);
                ViewBag.ProviderDisplayName = OpenAuth.GetProviderDisplayName(result.Provider);
                ViewBag.ReturnUrl = returnUrl;

                return this.View(
                    "ExternalLoginConfirmation",
                    new RegisterExternalLoginModel
                    {
                        UserName = result.UserName.Contains("@") ? result.UserName.Substring(0, result.UserName.IndexOf("@")) : result.UserName,
                        Email = result.UserName.Contains("@") ? result.UserName : string.Empty,
                        ExternalLoginData = loginData
                    });
            }
        }

        //// POST: /Account/ExternalLoginConfirmation

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string[] loginData;

            if (User.Identity.IsAuthenticated || (loginData = CryptoUtility.TryDeserialize("oauth", model.ExternalLoginData)) == null)
            {
                return this.RedirectToAction("Manage");
            }

            string provider = loginData[0];
            string providerUserId = loginData[1];
            string providerUserName = loginData[2];

            App.Data.User user;

            if (ModelState.IsValid && (user = this.membership.CreateUser(
                        userName: model.UserName,
                        email: model.Email,
                        password: null,
                        providerName: provider,
                        providerUserID: providerUserId,
                        providerUserName: providerUserName)) != null)
            {
                this.formsAuthentication.SetAuthCookie(user.UserName, createPersistentCookie: false);
                return this.RedirectToLocal(returnUrl);
            }

            ViewBag.ProviderDisplayName = OpenAuth.GetProviderDisplayName(provider);
            ViewBag.ReturnUrl = returnUrl;
            return this.View(model);
        }

        //// GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return this.View();
        }

        [AllowAnonymous, ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return this.PartialView("_ExternalLoginsListPartial", OpenAuth.AuthenticationClients.GetAll());
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            var externalLogins = this.membership.GetOpenAuthAccounts(User.Identity.Name).Select(account => new ExternalLogin
            {
                Provider = account.ProviderName,
                ProviderDisplayName = OpenAuth.AuthenticationClients.GetDisplayName(account.ProviderName),
                ProviderUserId = account.ProviderUserID,
            }).ToList();

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || this.membership.HasPassword(User.Identity.Name);
            return this.PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                this.Provider = provider;
                this.ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }

            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OpenAuth.RequestAuthentication(this.Provider, this.ReturnUrl);
            }
        }
        #endregion
    }
}
