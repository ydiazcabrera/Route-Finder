using RouteFinder.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

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
            //This is the old API set up with HERE called twice
            //ViewBag.APIText = RouteAPIDAL.DisplayMap("42.955485,-85.627450","42.956420,-85.696832","42.957991,-85.660483","42.953907,-85.652974");

            return View();
        }

        [HttpPost]
        public ActionResult Index(string startLong, string startLat, string endLong, string endLat)
        {
            string startPoint = $"{startLong},{startLat}";
            string endPoint = $"{endLong},{endLat}";
            ViewBag.APIText = RouteAPIDAL.DisplayMap(startPoint, endPoint, "42.957991,-85.660483", "42.953907,-85.652974");

            return View();
        }

        [HttpPost]
        public ActionResult RouteMap(string startLong, string startLat, string endLong, string endLat)
        {
            // Makes sure data is entered in form, but doesn't account for invalid data.
            // Need to add validation in action or in the api call. I would assume we could make sure
            // it is a valid number between. Might want to write a validation method for Longitude and Latitude
            if (startLong == "" || startLat == "" || endLong == "" || endLat == "")
            {
                return RedirectToAction("Index");
            }
            //Combine long and lat into single string
            string startPoint = $"{startLong},{startLat}";
            string endPoint = $"{endLong},{endLat}";

            // Hard-coded start/end points and a square to avoid. This will eventually pull in values from the user and sensor AQIs
            //List<RouteCoordinate> routeCoordinates = RouteAPIDAL.DisplayMap("42.906722,-85.725006", "42.960974,-85.605329", "42.969954,-85.639754", "42.927074,-85.609183");
            List<RouteCoordinate> routeCoordinates = RouteAPIDAL.DisplayMap(startPoint, endPoint, "42.969954,-85.639754", "42.927074,-85.609183");

            //hard-coded for testing purposes
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

            // This section builds a string, which is passed to the view and used by the JS script to display the sensors
            //ToDo - Find a way to display the name of the sensor / the AQI on the map without having to hover over the marker
            // https://forums.asp.net/t/2120631.aspx?Using+Razor+in+javascript+to+create+Google+map <= Citation
            string markers = "[";
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


            // This section builds a string, which is passed to the view and used by the JS script to display the route
            string route = "[";
            for (int i = 0; i < routeCoordinates.Count() - 1; i++)
            {
                route += "{ lat: " + routeCoordinates[i].Latitude + ", lng: " + routeCoordinates[i].Longitude + " },";
            }
            route += "{ lat: " + routeCoordinates[routeCoordinates.Count() - 1].Latitude + ", lng: " + routeCoordinates[routeCoordinates.Count() - 1].Longitude + " }];";

            RouteCoordinate centerCoordinate = routeCoordinates[(routeCoordinates.Count() / 2)];
            string mapCenter = "{ lat: " + centerCoordinate.Latitude + ", lng: " + centerCoordinate.Longitude + " }";

            //Map center is imperfect because the middle coordinate isn't necessarily the middle of the map.
            // It also doesn't address the zoom level. We could probably use a C# or .NET geography library to find the 
            // distance between the two furthest points to set distance and zoom.
            ViewBag.MapCenter = mapCenter;
            ViewBag.Sensors = markers;
            ViewBag.Route = route;
            
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ConcentrationTable()
        {
            DataTable concetration = new DataTable("Table");
            concetration.Columns.Add(new DataColumn()
            {
                ColumnName = "O3-8hr",
                DataType = typeof(string)
            });
            concetration.Columns.Add(new DataColumn()
            {
                ColumnName = "03-1hr",
                DataType = typeof(string)
            });
            concetration.Columns.Add(new DataColumn()
            {
                ColumnName = "PM10",
                DataType = typeof(string)
            });
            concetration.Columns.Add(new DataColumn()
            {
                ColumnName = "PM25",
                DataType = typeof(string)
            });
            concetration.Columns.Add(new DataColumn()
            {
                ColumnName = "AQI",
                DataType = typeof(string)
            });
            concetration.Columns.Add(new DataColumn()
            {
                ColumnName = "Category",
                DataType = typeof(string)
            });

            concetration.Rows.Add(new object[] { "0.000 - 0.059", "-", "0-54", "0.0-15.4", "0-50", "Good" });
            concetration.Rows.Add(new object[] { "0.060 - 0.075", "-", "55-154", "15.5 -40.4", "51-100", "Moderate" });
            concetration.Rows.Add(new object[] { "0.076 - 0.095", "0.125 - 0.164", "155-254", "40.5-65.4", "101-150", "Unhealthy for Sensitive Groups" });
            concetration.Rows.Add(new object[] { "0.096 - 0.115", "0.165 - 0.204", "255-354", "(65.5 - 150.4)3", "151-200", "Unhealthy" });
            concetration.Rows.Add(new object[] { "0.116 - 0.374", "0.205 - 0.404", "355-424", "(150.5 - (250.4)3", "201-300", "Very unhealthy" });
            concetration.Rows.Add(new object[] { "()2", "0.405 - 0.504", "425 - 504", "(250.5-(350.4)3", "301-400", "Hazardous" });
            concetration.Rows.Add(new object[] { "()2", "0.505 - 0.604", "505 - 604", "(350.5 - 500.4)3", "401-500", "Hazardous" });

            return View(concetration);
        }
    }

   
}