﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace RouteFinder.Models
{
    public class RouteAPIDAL
    {
        //instantiate entity db class of sensors instead of hardcoding them in here
        public List<Sensor> sensors = new List<Sensor>
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

        /// <summary>
        /// Method to get a map to display
        /// </summary>
        /// <param name="startPoint">Long/Lat of start point</param>
        /// <param name="endPoint">Long/Lat of end point</param>
        /// <param name="avoidTopLeftCoordinates">Long/Lat of top left corner of square to avoid</param>
        /// <param name="avoidBottomRightCoordinates">Long/Lat of bottom right corner of square to avoid</param>
        /// <returns>Image URL</returns>
        public static List<RouteCoordinate> DisplayMap(string startPoint, string endPoint, List<SensorBoundingBox> sensorsToAvoid = null)
        {
            List<RouteCoordinate> routeCoordinates = GetRoute(startPoint, endPoint, sensorsToAvoid);

            return routeCoordinates;
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
        public static List<RouteCoordinate> GetRoute(string startPoint, string endPoint, List<SensorBoundingBox> sensorsToAvoid = null)
        {
            string AppCode = ConfigReaderDAL.ReadSetting("app_code");
            string AppId = ConfigReaderDAL.ReadSetting("app_id");

            string URL = $"https://route.api.here.com/routing/7.2/calculateroute.json?app_id={AppId}&app_code={AppCode}&waypoint0=geo!{startPoint}&waypoint1=geo!{endPoint}&mode=fastest;car;traffic:disabled&avoidareas=";

            for (int i = 0; i < sensorsToAvoid.Count; i++)
            {
                string sensor = sensorsToAvoid[i].NorthWest.Latitude;
                sensor += "," + sensorsToAvoid[i].NorthWest.Longitude;
               
                sensor += ";";

                sensor += sensorsToAvoid[i].SouthEast.Latitude;
                sensor += "," + sensorsToAvoid[i].SouthEast.Longitude;

                if(i != sensorsToAvoid.Count - 1)
                {
                    sensor += "!";
                }

                URL += sensor;
            }

            URL += "&routeattributes=shape";

            string APIText = APICall(URL);

            List<RouteCoordinate> routeCoordinates = GetCoordinates(APIText);

            return routeCoordinates;
        }
        
        // Old API set up with HERE  being called twice
        /// <summary>
        /// This method returns a image url by taking in a list of coordinates and calling map image api from here.com
        /// </summary>
        /// <param name="routeCoordinates">a list of string coordinates that is passed to APICall method</param>
        /// <returns></returns>
        //public static string GetMap(List<string> routeCoordinates)
        //{
        //    string routeCoords = "";

        //    string AppCode = ConfigReaderDAL.ReadSetting("app_code");
        //    string AppId = ConfigReaderDAL.ReadSetting("app_id");

        //    foreach (string coordinate in routeCoordinates)
        //    {
        //        routeCoords += coordinate + ",";
        //    }

        //    //foreach(Sensor sensor in sensors)

        //    return $"https://image.maps.api.here.com/mia/1.6/route?r0={routeCoords}&w=500&app_id={AppId}&app_code={AppCode}";
        //}
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