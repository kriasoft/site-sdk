// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiControllerExtensions.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Web.Http
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    public static class ApiControllerExtensions
    {
        public static HttpResponseMessage ErrorResponse(this ApiController controller)
        {
            if (!controller.ModelState.Any())
            {
                controller.ModelState.AddModelError(string.Empty, "An unknown error occured.");
            }

            return controller.Request.CreateErrorResponse(HttpStatusCode.BadRequest, controller.ModelState);
        }
    }
}
