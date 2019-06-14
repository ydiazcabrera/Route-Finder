using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RouteFinder.Models;

namespace RouteFinder.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Route()
        {            
            return View();
        }

        public ActionResult Index()
        {
            ViewBag.APIText = RouteAPIDAL.DisplayMap("42.955485,-85.627450","42.956420,-85.696832","42.957991,-85.660483","42.953907,-85.652974");

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}