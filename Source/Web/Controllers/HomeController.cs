// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Web.Controllers
{
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            this.ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return this.View();
        }

        public ActionResult About()
        {
            this.ViewBag.Message = "Your app description page.";

            return this.View();
        }

        public ActionResult Contact()
        {
            this.ViewBag.Message = "Your contact page.";

            return this.View();
        }
    }
}
