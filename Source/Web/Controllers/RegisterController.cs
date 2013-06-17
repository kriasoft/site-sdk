// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisterController.cs" company="KriaSoft LLC">
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

    public class RegisterController : ApiController
    {
        private readonly DatabaseContext db;

        private readonly IFormsAuthentication formsAuth;

        public RegisterController(DatabaseContext db, IFormsAuthentication formsAuth)
        {
            this.db = db;
            this.formsAuth = formsAuth;
        }

        // POST: /api/register
        public HttpResponseMessage Post(RegisterUser user)
        {
            /* Make sure that NewUser entity has all the required DataAnnotation attributes for validation purposes. */

            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            /* Some additional validation */

            bool isUserNameUnique, isEmailUnique;
            this.db.Users.CheckUniqueness(user.UserName, user.Email, out isUserNameUnique, out isEmailUnique);

            if (isUserNameUnique)
            {
                this.ModelState.AddModelError("UserName", string.Format("A user with username '{0}' is already registered.", user.UserName));
            }

            if (isEmailUnique)
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

            this.formsAuth.SetAuthCookie(newUser.UserName, true);

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
    }
}
