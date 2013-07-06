// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountController.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Web.Controllers
{
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    using App.Data;
    using App.Identity;
    using App.Web.Models;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;

    [RoutePrefix("api")]
    public class AccountController : ApiController
    {
        private readonly DatabaseContext db;
        private readonly IdentityAuthenticationManager auth;

        public AccountController(DatabaseContext db)
        {
            this.db = db;
            this.auth = new IdentityAuthenticationManager(new IdentityStoreManager(new CustomIdentityStoreContext(this.db)));
        }

        [HttpPost("login")]
        public async Task<HttpResponseMessage> Login(LoginUser loginUser)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            var user = await this.db.Users.GetByUserNameOrEmailAndPassword(loginUser.UserName, loginUser.Password);

            if (user == null)
            {
                this.ModelState.AddModelError(string.Empty, "The username or password provided is incorrect.");
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            await this.auth.SignIn((HttpContextBase)this.Request.Properties["MS_HttpContext"], user.UserID.ToString(CultureInfo.InvariantCulture), isPersistent: true);

            return this.Request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    EmailHash = user.EmailHash,
                    DisplayName = user.DisplayName
                });
        }

        [HttpPost("logout")]
        public void LogOff()
        {
            this.auth.SignOut((HttpContextBase)this.Request.Properties["MS_HttpContext"]);
        }

        [HttpPost("register")]
        public async Task<HttpResponseMessage> Register(RegisterUser user)
        {
            /* Make sure that NewUser entity has all the required DataAnnotation attributes for validation purposes. */

            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            /* Some additional validation */

            var uniquenessResult = await this.db.Users.CheckUniqueness(user.UserName, user.Email);

            if (uniquenessResult.IsUserNameUnique)
            {
                this.ModelState.AddModelError("UserName", string.Format("A user with username '{0}' is already registered.", user.UserName));
            }

            if (uniquenessResult.IsEmailUnique)
            {
                this.ModelState.AddModelError("Email", string.Format("A user with email '{0}' is already registered.", user.Email));
            }

            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            /* Validation passed. Create a new user record */

            var newUser = this.db.Users.Add(user.UserName, user.Email, user.Password, user.DisplayName);
            this.db.SaveChanges();

            await this.auth.SignIn((HttpContextBase)this.Request.Properties["MS_HttpContext"], newUser.UserID.ToString(CultureInfo.InvariantCulture), isPersistent: true);

            return this.Request.CreateResponse(
                HttpStatusCode.Created,
                new
                {
                    UserName = newUser.UserName,
                    Email = newUser.Email,
                    EmailHash = newUser.EmailHash,
                    DisplayName = newUser.DisplayName
                });
        }

        [HttpPost("externallogin/{provider}")]
        public void ExternalLogin([FromUri] string provider)
        {
            this.auth.Challenge((HttpContextBase)this.Request.Properties["MS_HttpContext"], provider, "/api/externallogin");
        }
    }
}
