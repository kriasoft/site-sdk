// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthController.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Web.Controllers
{
    using App.Data;
    using App.Security;
    using DotNetOpenAuth.AspNet;
    using Microsoft.AspNet.Membership.OpenAuth;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;

    public class AuthController : ApiController
    {
        public readonly DatabaseContext db;

        public AuthController(DatabaseContext db)
        {
            this.db = db;
        }

        // GET: /api/auth/{provider}
        public HttpResponseMessage Get(string provider)
        {
            HttpResponseMessage response;
            var result = OpenAuth.VerifyAuthentication(VirtualPathUtility.ToAbsolute("~/api/auth/" + provider));

            if (!result.IsSuccessful)
            {
                response = this.Request.CreateResponse(HttpStatusCode.Redirect);
                response.Headers.Location = new Uri(VirtualPathUtility.ToAbsolute("~/login?auth=failed"), UriKind.Relative);
                return response;
            }

            var openAuthAccount = this.db.UserOpenAuthAccounts.SingleOrDefault(a => a.ProviderName == result.Provider && a.ProviderUserID == result.ProviderUserId);

            if (openAuthAccount != null)
            {
                var manager = new OpenAuthSecurityManager(new HttpContextWrapper(HttpContext.Current), OpenAuth.AuthenticationClients.GetByProviderName(result.Provider), new OpenAuthDataProvider(this.db));

                if (manager.Login(result.ProviderUserId, createPersistentCookie: true))
                {
                    openAuthAccount.LastUsedDate = DateTime.UtcNow;
                    this.db.SaveChanges();

                    response = this.Request.CreateResponse(HttpStatusCode.Redirect);
                    response.Headers.Location = new Uri(VirtualPathUtility.ToAbsolute("~/"), UriKind.Relative);
                    return response;
                }
            }

            if (this.User.Identity.IsAuthenticated)
            {
                var user = this.db.Users.SingleOrDefault(u => u.UserName == this.User.Identity.Name);

                if (user == null)
                {
                    throw new InvalidOperationException(string.Format("Cannot find a user with username '{0}'.", this.User.Identity.Name));
                }

                var dateNow = DateTime.UtcNow;

                this.db.UserOpenAuthAccounts.Add(new UserOpenAuthAccount
                {
                    UserID = user.UserID,
                    ProviderName = result.Provider,
                    ProviderUserID = result.ProviderUserId,
                    ProviderUserName = result.UserName,
                    LastUsedDate = dateNow
                });

                user.LastLoginDate = dateNow;
                user.LastActivityDate = dateNow;
                this.db.SaveChanges();

                response = this.Request.CreateResponse(HttpStatusCode.Redirect);
                response.Headers.Location = new Uri(VirtualPathUtility.ToAbsolute("~/"), UriKind.Relative);
                return response;
            }

            // User is new, ask for their desired membership name
            var loginData = CryptoUtility.Serialize("oauth", result.Provider, result.ProviderUserId, result.UserName);
            var url = "~/login?providerName=" + OpenAuth.GetProviderDisplayName(result.Provider) +
                      "&userName=" + (result.UserName.Contains("@") ? result.UserName.Substring(0, result.UserName.IndexOf("@")) : result.UserName) +
                      "&email=" + (result.UserName.Contains("@") ? result.UserName : string.Empty) +
                      "&externalLoginData=" + loginData;

            response = this.Request.CreateResponse(HttpStatusCode.Redirect);
            response.Headers.Location = new Uri(VirtualPathUtility.ToAbsolute(url), UriKind.Relative);
            return response;
        }

        // POST: /api/auth/{provider}
        public void Post(string provider)
        {
            OpenAuth.RequestAuthentication(provider, VirtualPathUtility.ToAbsolute("~/api/auth/" + provider));
        }
    }
}
