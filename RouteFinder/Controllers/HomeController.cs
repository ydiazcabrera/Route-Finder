﻿using System;
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

            //ViewBag.APIText = RouteAPIDAL.DisplayMap("42.955485,-85.627450","42.956420,-85.696832","42.957991,-85.660483","42.953907,-85.652974");
            List<RouteCoordinate> routeCoordinates = RouteAPIDAL.DisplayMap("42.955485,-85.627450", "42.956420,-85.696832", "42.957991,-85.660483", "42.953907,-85.652974");

            return View();
        }

        public ActionResult RouteMap()
        {
            List<string> sensorCoordinates = new List<string>();
            
            //hardcoded for testing purposes
            List<Sensor> sensors = new List<Sensor>
            {
                new Sensor( "42.9420703", "-85.6847243", "106", "OST"),
                new Sensor( "42.9547237", "-85.6824347", "107", "OST"),
                new Sensor( "42.9274400", "-85.6604877", "111", "OST"),
                new Sensor( "42.984136", "-85.671280", "101", "OST"),
                new Sensor( "42.9372291", ":-85.6669082", "115", "OST"),
                new Sensor( "42.92732229883891", "-85.64665123059183", "24358c", "SIMMS"),
                new Sensor( "42.904438", "-85.5814071", "232915", "SIMMS"),
                new Sensor( "42.9414937",  "-85.658029", "23339e", "SIMMS"),
                new Sensor( "42.9472356", "-85.6822996", "105", "OST"),
                new Sensor( "42.9201462", "-85.6476561", "108", "OST"),
                new Sensor( "42.984136", "-85.671280", "23acbc", "SIMMS"),
                new Sensor( "42.9467373", "-85.6843539", "117", "OST")
            };

            List<string> routeCoordinates = RouteAPIDAL.GetCoordinates();

            string markers = "[";

            //ToDo - Find a way to display the name of the sensor / the AQI on the map without having to hover over the marker
            //Build raw Json? Html? String? to get used by the JS script in the view.
            // https://forums.asp.net/t/2120631.aspx?Using+Razor+in+javascript+to+create+Google+map <= Citation
            for (int i = 0; i < sensors.Count; i++)
            {
                markers += "{";
                markers += string.Format("'title': '{0}',", sensors[i].Name); 
                markers += string.Format("'lat': '{0}',", sensors[i].Latitude);
                markers += string.Format("'lng': '{0}',", sensors[i].Longitude);
                //markers += string.Format("'description': '{0}'", "AQI: 50"); // This doesn't seem to be working
                markers += "},";
            }

            markers += "];";

            ViewBag.Sensors = markers;

            return View(sensorCoordinates);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}