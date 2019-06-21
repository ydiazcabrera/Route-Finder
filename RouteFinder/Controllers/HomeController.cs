using RouteFinder.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace RouteFinder.Controllers
{
    public class HomeController : Controller
    {
        SensorDbContext db = new SensorDbContext();

        public ActionResult Index()
        {
            string mapCenter = "{ lat: " + 42.9634 + ", lng: " + -85.6681 + " }";
            ViewBag.MapCenter = mapCenter;

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RouteMap(string startLong, string startLat, string endLong, string endLat, string modeOfT, string finalMap = "no")
        {
            // Makes sure data is entered in form, but doesn't account for invalid data.
            // Need to add validation in action or in the api call. I would assume we could make sure
            // it is a valid number between. Might want to write a validation method for Longitude and Latitude
            if (startLong == "" || startLat == "" || endLong == "" || endLat == "")
            {
                return RedirectToAction("Index");
            }

            //Combine long and lat into single string
            string startPoint = $"{startLat},{startLong}";
            string endPoint = $"{endLat},{endLong}";

            //Pull list of sensors directly from database.
            List<Sensor> sensors = GetSensors();

            //Call AvoidSensor Method and get a list of SensorboundingBox to avoid 
            List<Sensor> sensorsAboveAQIThreshold = GetSensorsAboveAQIThreshold(sensors);

            Route safeRoute = RouteAPIDAL.DisplayMap(startPoint, endPoint, sensorsAboveAQIThreshold, modeOfT);
            safeRoute.RouteCoordinatesString = GetRoute(safeRoute.RouteCoordinates);
            Session["SafeRoute"] = safeRoute;

            Route riskyRoute = RouteAPIDAL.DisplayMap(startPoint, endPoint, null, modeOfT);
            riskyRoute.RouteCoordinatesString = GetRoute(riskyRoute.RouteCoordinates);

            //build map marker string for sensors on Google MAP API
            string sensorMarkers = GetMarkers(sensors);

            //Map center is imperfect because the middle coordinate isn't necessarily the middle of the map.
            // It also doesn't address the zoom level. We could probably use a C# or .NET geography library to find the 
            // distance between the two furthest points to set distance and zoom.

            //Safe Map Route
            ViewBag.MapCenter = GetMapCenter(safeRoute.RouteCoordinates);
            ViewBag.Sensors = sensorMarkers;
            ViewBag.SafeRoute = safeRoute.RouteCoordinatesString;
            //ViewBag.Route = GetRoute(safeRouteCoordinates);

            // Risky Map Route
            ViewBag.RiskyMapCenter = GetMapCenter(riskyRoute.RouteCoordinates);
            ViewBag.RiskyMapSensors = sensorMarkers;
            //ViewBag.RiskyMapRoute = GetRoute(riskyRouteCoordinates);

            RouteViewModel rvm = new RouteViewModel(safeRoute, riskyRoute, sensors);
            
            if(finalMap == "yes")
            {
                return RedirectToAction("FinalMap", new { RVM = rvm });
            }

            return View(rvm);
        }

        public ActionResult FinalMap(RouteViewModel RVM)
        {
            return View(RVM);
        }

        public string GetMapCenter(List<RouteCoordinate> routeCoordinates)
        {
            //Finds center of map. Probably need to find more elogant solution.
            RouteCoordinate centerCoordinate = routeCoordinates[(routeCoordinates.Count() / 2)];

            return "{ lat: " + centerCoordinate.Latitude + ", lng: " + centerCoordinate.Longitude + " }";
        }

        public string GetMarkers(List<Sensor> sensors)
        {
            // This section builds a string, which is passed to the view and used by the JS script to display the sensors
            // https://forums.asp.net/t/2120631.aspx?Using+Razor+in+javascript+to+create+Google+map <= Citation
            string markers = "[";
            for (int i = 0; i < sensors.Count; i++)
            {
                markers += "{";
                markers += string.Format("'name': '{0}',", sensors[i].Name);
                markers += string.Format("'aqi': '{0}',", sensors[i].AQI);
                markers += string.Format("'lat': '{0}',", sensors[i].Latitude);
                markers += string.Format("'lng': '{0}',", sensors[i].Longitude);
                markers += string.Format("'north': '{0}',", sensors[i].BoundingBox.NorthEast.Longitude);
                markers += string.Format("'south': '{0}',", sensors[i].BoundingBox.SouthEast.Longitude);
                markers += string.Format("'east': '{0}',", sensors[i].BoundingBox.NorthEast.Latitude);
                markers += string.Format("'west': '{0}',", sensors[i].BoundingBox.NorthWest.Latitude);
                //markers += string.Format("'description': '{0}'", "AQI: 50"); // This doesn't seem to be working
                markers += "},";
            }
            markers += "];";

            return markers;
        }

        public string GetRoute(List<RouteCoordinate> routeCoordinates)
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
            double hourlyO3Avg = GetHourlyAvg(sixtyMinSensorData, "O3_PPB");
            int aqiO3 = CalcluateO3AQI(hourlyO3Avg);

            // find the hourly average for particulate matter
            double hourlyPM25Avg = GetHourlyAvg(sixtyMinSensorData, "PM25_MicroGramPerCubicMeter");
            int aqiPM25 = CalcluatePM25AQI(hourlyPM25Avg);

            if (aqiO3 > aqiPM25)
            {
                aqi = aqiO3;
            }
            else
            {
                aqi = aqiPM25;
            }

            //return aqi;
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
                if (sensor.AQI > 100)
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

        public double GetHourlyAvg(List<SensorsData> sensorData, string pollutant)
        {
            double dataPoint = 0.00;
            //List<int> AQIs = new List<int>();

            int datarows = sensorData.Count();
            double runningTotal = 0.00;

            foreach (SensorsData s in sensorData)
            {
                if (pollutant == "O3_PPB")
                {
                    dataPoint = (double)s.O3_PPB;
                    runningTotal += (dataPoint / 1000); // convert from ppb to ppm
                }
                else if (pollutant == "PM25_MicroGramPerCubicMeter")
                {
                    dataPoint = (int)s.PM25_MicroGramPerCubicMeter;
                    runningTotal += dataPoint;
                }
            }

            return runningTotal / sensorData.Count();
        }

        public int CalcluateO3AQI(double avgO3)
        {
            double O3Min = 0;
            double O3Max = 0;
            int AQIMin = 0;
            int AQIMax = 0;

            if (avgO3 >= 0 && avgO3 < 0.059)
            {
                O3Min = 0;
                O3Max = 0.059;
                AQIMin = 0;
                AQIMax = 50;
            }
            else if (avgO3 >= 0.060 && avgO3 < 0.075)
            {
                O3Min = 0.060;
                O3Max = 0.075;
                AQIMin = 51;
                AQIMax = 100;
            }
            else if (avgO3 >= 0.076 && avgO3 < 0.095)
            {
                O3Min = 0.076;
                O3Max = 0.095;
                AQIMin = 101;
                AQIMax = 150;

            }
            else if (avgO3 >= 0.096 && avgO3 < 0.115)
            {
                O3Min = 0.096;
                O3Max = 0.115;
                AQIMin = 151;
                AQIMax = 200;
            }
            else if (avgO3 >= 0.116 && avgO3 < 0.374)
            {
                O3Min = 0.116;
                O3Max = 0.374;
                AQIMin = 201;
                AQIMax = 300;
            }
            else if (avgO3 >= 0.405)// this is a combination of two hazardous reading, might need to change
            {
                O3Min = 0.405;
                O3Max = 0.604;
                AQIMin = 301;
                AQIMax = 500;
            }

            int AQI = CalculateAQI(AQIMax, AQIMin, O3Max, O3Min, avgO3);
            return AQI;
        }

        public int CalcluatePM25AQI(double avgPM25)
        {
            double PM25Min = 0;
            double PM25Max = 0;
            int AQIMin = 0;
            int AQIMax = 0;

            if (avgPM25 >= 0 && avgPM25 < 15.4)
            {
                PM25Min = 0;
                PM25Max = 15.4;
                AQIMin = 0;
                AQIMax = 50;
            }
            else if (avgPM25 >= 15.5 && avgPM25 < 40.4)
            {
                PM25Min = 15.5;
                PM25Max = 40.4;
                AQIMin = 51;
                AQIMax = 100;
            }
            else if (avgPM25 >= 40.5 && avgPM25 < 65.4)
            {
                PM25Min = 40.5;
                PM25Max = 65.4;
                AQIMin = 101;
                AQIMax = 150;
            }
            else if (avgPM25 >= 65.5 && avgPM25 < 150.4)
            {
                PM25Min = 65.5;
                PM25Max = 150.4;
                AQIMin = 151;
                AQIMax = 200;
            }
            else if (avgPM25 >= 150.5 && avgPM25 < 250.4)
            {
                PM25Min = 150.5;
                PM25Max = 250.4;
                AQIMin = 201;
                AQIMax = 300;
            }
            else if (avgPM25 >= 250.5 && avgPM25 < 350.4)
            {
                PM25Min = 250.5;
                PM25Max = 350.4;
                AQIMin = 301;
                AQIMax = 400;
            }
            else if (avgPM25 >= 350.5)
            {
                PM25Min = 350.5;
                PM25Max = 500.4;
                AQIMin = 401;
                AQIMax = 500;
            }

            int AQI = CalculateAQI(AQIMax, AQIMin, PM25Max, PM25Min, avgPM25);
            return AQI;
        }

        public int CalculateAQI(int aqiMax, int aquMin, double pollutantMax, double pollutantMin, double pollutantReading)
        {
            int AQI = Convert.ToInt32((((aqiMax - aquMin) / (pollutantMax - pollutantMin)) * (pollutantReading - pollutantMin)) + aquMin);
            return AQI;
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
