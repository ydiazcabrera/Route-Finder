using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace RouteFinder.Models
{
    public class RouteAPIDAL
    {
        /// <summary>
        /// Method to get a map to display
        /// </summary>
        /// <param name="startPoint">Long/Lat of start point</param>
        /// <param name="endPoint">Long/Lat of end point</param>
        /// <param name="avoidTopLeftCoordinates">Long/Lat of top left corner of square to avoid</param>
        /// <param name="avoidBottomRightCoordinates">Long/Lat of bottom right corner of square to avoid</param>
        /// <returns>Image URL</returns>
        public static Route DisplayMap(string startPoint, string endPoint, List<Sensor> sensorsToAvoid = null, string mode = "pedestrian")
        {
            Route route = GetRoute(startPoint, endPoint, sensorsToAvoid, mode);

            return route;
            //string mapImage = GetMap(routeCoordinates);
            //return mapImage;
        }

        internal static List<string> GetCoordinates()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method to Call Routing API from here.com that generates route
        /// from users start and endpoint while avoiding areas of bad AQI
        /// </summary>
        /// <param name="startPoint">Long/Lat of start point</param>
        /// <param name="endPoint">Long/Lat of end point</param>
        /// <param name="avoidTopLeftCoordinates">Long/Lat of top left corner of square to avoid</param>
        /// <param name="avoidBottomRightCoordinates">Long/Lat of bottom right corner of square to avoid</param>
        /// <returns> A list of coordinates for route</returns>
        public static Route GetRoute(string startPoint, string endPoint, List<Sensor> sensorsToAvoid = null, string mode = "pedestrian")
        {
            Route route = new Route();
            route.ModeOfTransportation = mode;
            string AppCode = ConfigReaderDAL.ReadSetting("app_code");
            string AppId = ConfigReaderDAL.ReadSetting("app_id");

            string URL = $"https://route.api.here.com/routing/7.2/calculateroute.json?app_id={AppId}&app_code={AppCode}&waypoint0=geo!{startPoint}&waypoint1=geo!{endPoint}&mode=fastest;{mode};traffic:disabled";

            if (sensorsToAvoid != null)
            {
                URL += "&avoidareas=";
                for (int i = 0; i < sensorsToAvoid.Count; i++)
                {
                    string sensor = sensorsToAvoid[i].BoundingBox.NorthWest.Latitude;
                    sensor += "," + sensorsToAvoid[i].BoundingBox.NorthWest.Longitude;

                    sensor += ";";

                    sensor += sensorsToAvoid[i].BoundingBox.SouthEast.Latitude;
                    sensor += "," + sensorsToAvoid[i].BoundingBox.SouthEast.Longitude;

                    if (i != sensorsToAvoid.Count - 1)
                    {
                        sensor += "!";
                    }

                    URL += sensor;
                }
            }

            URL += "&routeattributes=shape";

            string APIText = APICall(URL);

            route.RouteCoordinates = GetCoordinates(APIText);
            route.TotalTravelTime = GetTotalTravelTime(APIText);
            route.TotalDistance = GetTotalDistance(APIText);
            route.Maneuvers = GetManeuvers(APIText);

            return route;
        }

        /// <summary>
        /// Parses Json to get coordinates from APIText String
        /// </summary>
        /// <param ="APIText">string APICall Response</param>
        /// <returns> returns a List of strings containing route coordinates </returns>
        public static List<RouteCoordinate> GetCoordinates(string APIText)
        {
            List<RouteCoordinate> routeCoordinates = new List<RouteCoordinate>();

            JToken json = JToken.Parse(APIText);
            List<JToken> jsonTokens = json["response"]["route"][0]["shape"].ToList();

            foreach (JToken token in jsonTokens)
            {
                string coordToken = token.ToString();
                string[] coord = coordToken.Split(',');
                RouteCoordinate rc = new RouteCoordinate(coord[0], coord[1], "Route Coordinate");
                routeCoordinates.Add(rc);
            }

            return routeCoordinates;
        }

        public static List<Maneuver> GetManeuvers(string APIText)
        {
            List<Maneuver> maneuvers = new List<Maneuver>();
            JToken json = JToken.Parse(APIText);
            List<JToken> jsonTokens = json["response"]["route"][0]["leg"][0]["maneuver"].ToList();
            for (int i = 0; i < jsonTokens.Count; i++)
            {

                string instruction = jsonTokens[i]["instruction"].ToString();
                double travelTime = double.Parse(jsonTokens[i]["travelTime"].ToString());
                double distance = double.Parse(jsonTokens[i]["length"].ToString());
                Maneuver man = new Maneuver(travelTime, distance, instruction);
                maneuvers.Add(man);
            }
            return maneuvers;
        }


        public static double GetTotalTravelTime(string APIText)
        {
            List<RouteCoordinate> routeCoordinates = new List<RouteCoordinate>();

            JToken json = JToken.Parse(APIText);

            return double.Parse(json["response"]["route"][0]["leg"][0]["travelTime"].ToString());
        }

        public static double GetTotalDistance(string APIText)
        {
            List<RouteCoordinate> routeCoordinates = new List<RouteCoordinate>();

            JToken json = JToken.Parse(APIText);

            return double.Parse(json["response"]["route"][0]["leg"][0]["length"].ToString());
        }

        /// <summary>
        /// This method calls the APIs
        /// </summary>
        /// <param name="URL">The url of the API endpoint</param>
        /// <returns>returns a string of json</returns>
        public static string APICall(string URL)
        {
            HttpWebRequest request = WebRequest.CreateHttp(URL);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader rd = new StreamReader(response.GetResponseStream());

            string APIText = rd.ReadToEnd();

            return APIText;
        }
    }
}