using RouteFinder.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace RouteFinder.Controllers
{
    public class HomeController : Controller
    {
        SensorDbContext db = new SensorDbContext();

        //Action for Index view
        public ActionResult Index()
        {          
            //reset sessions for new route
            Session["ModeOfTransportation"] = null;
            Session["rvm"] = null;

            return View();
        }

        //Action for Index view
        public ActionResult About()
        {
            return View();
        }

        public ActionResult RouteMap()
        {
            return RedirectToAction("Index");
        }

        //Displays RouteMap from a form post
        [HttpPost]
        public ActionResult RouteMap(string startLong, string startLat, string endLong, string endLat, string modeOfT)
        {
            // Makes sure data is entered in form, but doesn't account for invalid data.
            // Need to add validation in action or in the api call. I would assume we could make sure
            // it is a valid number between. Might want to write a validation method for Longitude and Latitude
            if ((startLong == "" || startLat == "" || endLong == "" || endLat == "" || modeOfT == "" ) && Session["rvm"] == null)
            {
                return RedirectToAction("Index");
            }

            if (Session["rvm"] == null)
            {

                //Combine long and lat into single string
                string startPoint = $"{startLat},{startLong}";
                string endPoint = $"{endLat},{endLong}";

                //Pull list of sensors directly from database.
                List<Sensor> sensors = GetSensors();

                //Call AvoidSensor Method and get a list of SensorboundingBox to avoid 
                List<Sensor> sensorsAboveAQIThreshold = GetSensorsAboveAQIThreshold(sensors);

                Route safeWalkRoute = RouteAPIDAL.DisplayMap(startPoint, endPoint, sensorsAboveAQIThreshold, "pedestrian");
                safeWalkRoute.RouteCoordinatesString = BuildJsRouteCoordinates(safeWalkRoute.RouteCoordinates);
                safeWalkRoute.RouteCoordinatesString = BuildJsRouteCoordinates(safeWalkRoute.RouteCoordinates);


                Route safeBikeRoute = RouteAPIDAL.DisplayMap(startPoint, endPoint, sensorsAboveAQIThreshold, "bicycle");
                safeBikeRoute.RouteCoordinatesString = BuildJsRouteCoordinates(safeWalkRoute.RouteCoordinates);
                safeBikeRoute.RouteCoordinatesString = BuildJsRouteCoordinates(safeBikeRoute.RouteCoordinates);

                Route fastWalkRoute = RouteAPIDAL.DisplayMap(startPoint, endPoint, null, "pedestrian");
                fastWalkRoute.RouteCoordinatesString = BuildJsRouteCoordinates(fastWalkRoute.RouteCoordinates);
                fastWalkRoute.RouteCoordinatesString = BuildJsRouteCoordinates(fastWalkRoute.RouteCoordinates);


                Route fastBikeRoute = RouteAPIDAL.DisplayMap(startPoint, endPoint, null, "bicycle");
                fastBikeRoute.RouteCoordinatesString = BuildJsRouteCoordinates(fastBikeRoute.RouteCoordinates);
                fastBikeRoute.RouteCoordinatesString = BuildJsRouteCoordinates(fastBikeRoute.RouteCoordinates);

                //build map marker string for sensors on Google MAP API
                string sensorMarkers = BuildJsSensors(sensors);

                // Map center is imperfect because the middle coordinate isn't necessarily the middle of the map.
                // It also doesn't address the zoom level. We could probably use a C# or .NET geography library to find the 
                // distance between the two furthest points to set distance and zoom.

                //Sets center of map
                ViewBag.MapCenter = GetMapCenter(safeWalkRoute.RouteCoordinates);

                //Builds Javascript object string to display markers on google API map.
                ViewBag.Sensors = sensorMarkers;

                // Store currently selected mode of transportation walk/bike
                Session["ModeOfTransportation"] = modeOfT;

                // Routes for all paths and sensors data
                RouteViewModel rvm = new RouteViewModel(safeBikeRoute, fastBikeRoute, safeWalkRoute, fastWalkRoute, sensors);

                // Store route view model in session for use on final route choice page
                Session["rvm"] = rvm;
                return View(rvm);
            } else
            {
                RouteViewModel rvm = (RouteViewModel)Session["rvm"];

                //Sets center of map
                ViewBag.MapCenter = GetMapCenter(rvm.SafeWalkRoute.RouteCoordinates);

                //Builds Javascript object string to display markers on google API map.
                ViewBag.Sensors = BuildJsSensors(rvm.Sensors);

                // Store currently selected mode of transportation walk/bike
                Session["ModeOfTransportation"] = modeOfT;
                return View(rvm);
            }
            // Pass Route View Model to view
            
        }

        //public ActionResult FinalMap(int id)
        //public ActionResult FinalMap(RouteViewModel finalMap)
        //public ActionResult FinalMap(string startLong, string startLat, string endLong, string endLat, string modeOfT, string routeSelected)
        public ActionResult FinalMap(string modeOfTransportation, string safeOrFast)
        {
            if (String.IsNullOrEmpty(modeOfTransportation) || String.IsNullOrEmpty(safeOrFast))
            {
                return RedirectToAction("RouteMap");
            }
            Route route = new Route();

            // Get Route View Model from session
            RouteViewModel rvm = (RouteViewModel)Session["rvm"];
            if(modeOfTransportation == "pedestrian")
            {
                if(safeOrFast == "safe")
                {
                    route = rvm.SafeWalkRoute;
                }
                else
                {
                    route = rvm.FastWalkRoute;
                }
            }
            else
            {
                if(safeOrFast == "safe")
                {
                    route = rvm.SafeBikeRoute;
                }
                else
                {
                    route = rvm.FastBikeRoute;
                }
            }

            ViewBag.MapCenter = GetMapCenter(route.RouteCoordinates);

            return View(route);
        }

    public string GetMapCenter(List<RouteCoordinate> routeCoordinates)
        {
            //Finds center of map. Probably need to find more elogant solution.
            RouteCoordinate centerCoordinate = routeCoordinates[(routeCoordinates.Count() / 2)];

            return "{ lat: " + centerCoordinate.Latitude + ", lng: " + centerCoordinate.Longitude + " }";
        }

        public string BuildJsSensors(List<Sensor> sensors)
        {
            // This section builds a string, which is passed to the view and used by the JS script to display the sensors
            // https://forums.asp.net/t/2120631.aspx?Using+Razor+in+javascript+to+create+Google+map <= Citation
            string sensorMarkers = "[";
            for (int i = 0; i < sensors.Count; i++)
            {
                sensorMarkers += "{";
                sensorMarkers += string.Format("'name': '{0}',", sensors[i].Name);
                sensorMarkers += string.Format("'aqi': '{0}',", sensors[i].AQI);
                sensorMarkers += string.Format("'lat': '{0}',", sensors[i].Latitude);
                sensorMarkers += string.Format("'lng': '{0}',", sensors[i].Longitude);
                sensorMarkers += string.Format("'north': '{0}',", sensors[i].BoundingBox.NorthEast.Longitude);
                sensorMarkers += string.Format("'south': '{0}',", sensors[i].BoundingBox.SouthEast.Longitude);
                sensorMarkers += string.Format("'east': '{0}',", sensors[i].BoundingBox.NorthEast.Latitude);
                sensorMarkers += string.Format("'west': '{0}',", sensors[i].BoundingBox.NorthWest.Latitude);
                //markers += string.Format("'description': '{0}'", "AQI: 50"); // This doesn't seem to be working
                sensorMarkers += "},";
            }
            sensorMarkers += "];";

            return sensorMarkers;
        }

        public string BuildJsRouteCoordinates(List<RouteCoordinate> routeCoordinates)
        {
            // This section builds a string, which is passed to the view and used by the JS script to display the route
            string route = "[";
            for (int i = 0; i < routeCoordinates.Count() - 1; i++)
            {
                route += $"{{ lat: {routeCoordinates[i].Latitude}, lng: { routeCoordinates[i].Longitude} }}";

                // This allows to put last coordinate without a ending comma and closes the array
                if (i != routeCoordinates.Count - 1)
                {
                    route += ",";
                }
            }
            route += $"];";

            return route;
        }

        public List<SensorsData> GetLastSixtyMinutesSensorData(string sensorName)
        {
            DateTime oneMonthOneHourAgo = DateTime.Today.AddMonths(-1).AddHours(-1);
            DateTime oneMonthAgo = DateTime.Today.AddMonths(-1);
            List<SensorsData> sensorsData = db.SensorsData.Where(x => x.Time >= oneMonthOneHourAgo && x.Time <= oneMonthAgo && x.Sensor.Name == sensorName).ToList();

            return sensorsData;
        }

        public int GetSensorAQI(Sensor sensor)
        {
            int aqi = 0;
            //Pull an hour of data from a sensor
            List<SensorsData> sixtyMinSensorData = GetLastSixtyMinutesSensorData(sensor.Name);

            if (sixtyMinSensorData.Count() == 0) //There was no data collected by the sensor for the time called
            {
                return 0;
            }

            // find the hourly average for ozone
            double hourlyO3Avg = AQI_Calculator.GetHourlyAvg(sixtyMinSensorData, "O3_PPB");
            int aqiO3 = AQI_Calculator.CalcluateO3AQI(hourlyO3Avg);

            // find the hourly average for particulate matter
            double hourlyPM25Avg = AQI_Calculator.GetHourlyAvg(sixtyMinSensorData, "PM25_MicroGramPerCubicMeter");
            int aqiPM25 = AQI_Calculator.CalcluatePM25AQI(hourlyPM25Avg);

            if (aqiO3 > aqiPM25)
            {
                aqi = aqiO3;
            }
            else
            {
                aqi = aqiPM25;
            }

            return aqi;
        }

        public List<Sensor> GetSensors()
        {
            List<Sensor> sensors = GetSensorsFromDatabase();

            for (int i = 0; i < sensors.Count; i++)
            {
                sensors[i].BoundingBox = GetSensorBoundingBox(sensors[i]);
                sensors[i].AQI = GetSensorAQI(sensors[i]);

            }

            return sensors;
        }

        //public List<Sensor> GetListSensors(List<int> aqis)
        public List<Sensor> GetSensorsFromDatabase()
        {
            try
            {
                return db.Sensors.ToList();
            }
            catch (Exception)
            {
                return null;
            }

        }

        public List<Sensor> GetSensorsAboveAQIThreshold(List<Sensor> sensors)
        {
            List<Sensor> sensorsAboveAQIThreshold = new List<Sensor>();

            foreach (Sensor sensor in sensors)
            {
                if (sensor.AQI > 35)
                {
                    sensorsAboveAQIThreshold.Add(sensor);
                }

            }

            if (sensorsAboveAQIThreshold.Count == 0)
            {
                sensorsAboveAQIThreshold = null;
            }

            return sensorsAboveAQIThreshold;
        }

        public SensorBoundingBox GetSensorBoundingBox(Sensor sensor)
        {

            double lat = double.Parse(sensor.Latitude);
            double lon = double.Parse(sensor.Longitude);
            double earthRadius = 6378137;
            double n = 300;
            double e = 300;
            double s = -300;
            double w = -300;

            double uLat = n / earthRadius;
            double uLon = e / (earthRadius * Math.Cos(Math.PI * lat / 180));
            double dLat = s / earthRadius;
            double dLon = w / (earthRadius * Math.Cos(Math.PI * lat / 180));

            double nwLatPoint = lat + uLat * 180 / Math.PI;
            double nwLonPoint = lon + uLon * 180 / Math.PI;

            double seLatPoint = lat + dLat * 180 / Math.PI;
            double seLonPoint = lon + dLon * 180 / Math.PI;

            MapPoint NorthWest = new MapPoint(nwLatPoint.ToString("N6"), nwLonPoint.ToString("N6"), sensor.Name + "_NorthWest");
            MapPoint SouthEast = new MapPoint(seLatPoint.ToString("N6"), seLonPoint.ToString("N6"), sensor.Name + "_SouthEast");
            MapPoint NorthEast = new MapPoint(seLatPoint.ToString("N6"), nwLonPoint.ToString("N6"), sensor.Name + "_NorthEast");
            MapPoint SouthWest = new MapPoint(nwLatPoint.ToString("N6"), seLonPoint.ToString("N6"), sensor.Name + "_SouthWest");

            SensorBoundingBox sbb = new SensorBoundingBox(sensor, NorthWest, SouthEast, NorthEast, SouthWest);

            return sbb;
        }

        public int CaloriesBurnedWalked(int weight, int mile)
        {
            int caloriesBurned = 0;
            if (weight >= 180)
            {
                caloriesBurned = 100 * mile;
            }
            else if (weight < 180 && weight >= 120)
            {
                caloriesBurned = 65 * mile;
            }
            else
            {
                caloriesBurned = 53 * mile;
            }
            return caloriesBurned;
        }
    }
}
