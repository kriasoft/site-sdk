// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginController.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Web.Controllers
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using App.Data;
    using App.Security;
    using App.Web.Models;

    public class LoginController : ApiController
    {
        public readonly DatabaseContext db;

        public readonly IFormsAuthentication formsAuth;

        public LoginController(DatabaseContext db, IFormsAuthentication formsAuth)
        {
            this.db = db;
            this.formsAuth = formsAuth;
        }

        // POST: /api/login
        public HttpResponseMessage Post(LoginUser loginUser)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            var user = this.db.Users.GetByUserNameOrEmail(loginUser.UserName, loginUser.Password);

            if (user == null)
            {
                this.ModelState.AddModelError(string.Empty, "The username or password provided is incorrect.");
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            this.formsAuth.SetAuthCookie(user.UserName, true);

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
    }
}
