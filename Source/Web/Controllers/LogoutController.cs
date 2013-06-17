// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogoutController.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Web.Controllers
{
    using System.Web.Http;

    using App.Security;

    public class LogoutController : ApiController
    {
        private readonly IFormsAuthentication formsAuth;

        public LogoutController(IFormsAuthentication formsAuth)
        {
            this.formsAuth = formsAuth;
        }

        // POST: /api/logout
        public void Post()
        {
            this.formsAuth.SignOut();
        }
    }
}
