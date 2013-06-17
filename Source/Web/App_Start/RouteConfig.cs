// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteConfig.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Web
{
    using System.Web.Mvc;
    using System.Web.Routing;

    using App.Web.Controllers;
    using App.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.LowercaseUrls = true;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapWebPageRoute(
                name: "Default",
                url: "{*url}",
                path: "~/Index.cshtml");
        }
    }
}