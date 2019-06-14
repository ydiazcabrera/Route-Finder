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
            ViewBag.APIText = RouteAPIDAL.Coordinates();
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