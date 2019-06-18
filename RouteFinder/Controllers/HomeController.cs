using RouteFinder.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity.Migrations;

namespace RouteFinder.Controllers
{
    public class HomeController : Controller
    {
        SensorDbContext db = new SensorDbContext();

        public ActionResult Index()
        {
            GetAQI();
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

            //Pull list of sensors directly from database.
            List<Sensor> sensors = db.Sensors.ToList();

            // This section builds a string, which is passed to the view and used by the JS script to display the sensors
            //ToDo - Find a way to display the name of the sensor / the AQI on the map without having to hover over the marker
            // https://forums.asp.net/t/2120631.aspx?Using+Razor+in+javascript+to+create+Google+map <= Citation
            string markers = "[";
            for (int i = 0; i < sensors.Count; i++)
            {
                markers += "{";
                markers += string.Format("'name': '{0}',", sensors[i].Name);
                markers += string.Format("'aqi': '{0}',", sensors[i].AQI);
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
                route += $"{{ lat: {routeCoordinates[i].Latitude}, lng: { routeCoordinates[i].Longitude} }},";
            }
            // This allows to put last coordinate without a ending comma and closes the array
            route += $"{{ lat: {routeCoordinates[routeCoordinates.Count() - 1].Latitude}, lng: {routeCoordinates[routeCoordinates.Count() - 1].Longitude}}}];";

            //Finds center of map. Probably need to find more elogant solution.
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

        public List<SensorsData> GetLastSixtyMinutesSensorData(string sensorName)
        {
            DateTime oneMonthOneHourAgo = DateTime.Today.AddMonths(-1).AddHours(-1);
            DateTime oneMonthAgo = DateTime.Today.AddMonths(-1);
            List<SensorsData> sensorsData = db.SensorsData.Where(x => x.Time >= oneMonthOneHourAgo && x.Time <= oneMonthAgo && x.Sensor.Name == sensorName).ToList();

            return sensorsData;

        }
        public void GetAQI()
        {
            List<Sensor> sensors = db.Sensors.ToList();
            foreach (Sensor sensor in sensors)
            {
                List<SensorsData> sensorData = GetLastSixtyMinutesSensorData(sensor.Name);
                List<int>AQIs =GetHourlyAvg(sensorData);
                if(AQIs[0] > AQIs[1])
                {
                    sensor.AQI = AQIs[0];
                }
                else
                {
                    sensor.AQI = AQIs[1];
                }
                db.Sensors.AddOrUpdate();
                db.SaveChanges();
            }

            //return 1;
        }

        public List<int> GetHourlyAvg(List<SensorsData> sensorData)
        {
            List<int> AQIs = new List<int>();
            int datarows = sensorData.Count();
            int runningTotalO3 = 0;
            int runningTotalPM25 = 0;

            foreach (SensorsData s in sensorData)
            {
                runningTotalO3 += (int)(s.O3_PPB / 1000);
                runningTotalPM25 += (int)s.PM25_MicroGramPerCubicMeter;
            }

            double avgO3 = runningTotalO3 / datarows;
            double avgPM25 = runningTotalPM25 / datarows;
            
            int O3AQI= FindBreakpointsO3(avgO3);
            AQIs.Add(O3AQI);
            int PM25AQI= FindBreakpointsPM25(avgPM25);
            AQIs.Add(PM25AQI);

            return AQIs;
        }
        public int FindBreakpointsO3(double avgO3)
        {
            double O3Min=0;
            double O3Max=0;
            int AQIMin=0;
            int AQIMax=0;

            if (avgO3 >= 0 && avgO3 < 0.059)
            {
                O3Min = 0;
                O3Max = 0.059;
                AQIMin = 0;
                AQIMax = 50;
                //double EquationResult = ((AQIMax - AQIMin) / (O3Max - O3Min)) * (avgO3 - O3Min) + AQIMin;
                //int AQI = Convert.ToInt32(Math.Round(EquationResult));
            }
            if (avgO3 >= 0.060 && avgO3 < 0.075)
            {
                O3Min = 0.060;
                O3Max = 0.075;
                AQIMin = 51;
                AQIMax = 100;

            }
            if (avgO3 >= 0.076 && avgO3 < 0.095)
            {
                O3Min = 0.076;
                O3Max = 0.095;
                AQIMin = 101;
                AQIMax = 150;

            }
            if (avgO3 >= 0.096 && avgO3 < 0.115)
            {
                O3Min = 0.096;
                O3Max = 0.115;
                AQIMin = 151;
                AQIMax = 200;
            }
            if (avgO3 >= 0.116 && avgO3 < 0.374)
            {
                O3Min = 0.116;
                O3Max = 0.374;
                AQIMin = 201;
                AQIMax = 300;
            }
            if (avgO3 >= 0.405)// this is a combination of two hazardous reading, might need to change
            {
                O3Min = 0.405;
                O3Max = 0.604;
                AQIMin = 301;
                AQIMax = 500;
            }
            int AQI=AQIEquation(AQIMax, AQIMin, O3Max, O3Min, avgO3);
            return AQI;
        }

        public int FindBreakpointsPM25(double avgPM25)
        {

            double PM25Min=0;
            double PM25Max=0;
            int AQIMin = 0;
            int AQIMax = 0;

            if (avgPM25 >= 0 && avgPM25 < 15.4)
            {
                PM25Min = 0;
                PM25Max = 15.4;
                AQIMin = 0;
                AQIMax = 50;
            }
            if (avgPM25 >= 15.5 && avgPM25 < 40.4)
            {
                PM25Min = 15.5;
                PM25Max = 40.4;
                AQIMin = 51;
                AQIMax = 100;
            }
            if (avgPM25 >= 40.5 && avgPM25 < 65.4)
            {
                PM25Min = 40.5;
                PM25Max = 65.4;
                AQIMin = 101;
                AQIMax = 150;
            }
            if (avgPM25 >= 65.5 && avgPM25 < 150.4)
            {
                PM25Min = 65.5;
                PM25Max = 150.4;
                AQIMin = 151;
                AQIMax = 200;
            }
            if (avgPM25 >= 150.5 && avgPM25 < 250.4)
            {
                PM25Min = 150.5;
                PM25Max = 250.4;
                AQIMin = 201;
                AQIMax = 300;
            }
            if (avgPM25 >= 250.5 && avgPM25 < 350.4)
            {
                PM25Min = 250.5;
                PM25Max = 350.4;
                AQIMin = 301;
                AQIMax = 400;
            }
            if (avgPM25 >= 350.5)
            {
                PM25Min = 350.5;
                PM25Max = 500.4;
                AQIMin = 401;
                AQIMax = 500;
            }
            int AQI =AQIEquation(AQIMax, AQIMin, PM25Max, PM25Min, avgPM25);
            return AQI;
        }

        public int AQIEquation(int aqiMax, int aquMin, double pollutantMax, double pollutantMin, double pollutantreading)
        {
            int AQI = Convert.ToInt32((((aqiMax - aquMin) / (pollutantMax - pollutantMin)) * (pollutantreading - pollutantMin)) + aquMin);
            return AQI;
        }

        public int CaloriesBurnedWalked(int weight,int mile)
        {
            int caloriesBurned = 0;
            if(weight >= 180)
            {
                caloriesBurned = 100 * mile;
            }
            else if(weight<180 && weight >= 120)
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
