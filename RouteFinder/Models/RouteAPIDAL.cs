using Newtonsoft.Json.Linq;
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
        public static string DisplayMap(string startPoint, string endPoint, string avoidTopLeftCoordinates = null, string avoidBottomRightCoordinates = null)
        {
            List<string> routeCoordinates = GetRoute(startPoint, endPoint, avoidTopLeftCoordinates, avoidBottomRightCoordinates);

            string mapImage = GetMap(routeCoordinates);

            return mapImage;
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
        public static List<string> GetRoute(string startPoint, string endPoint, string avoidTopLeftCoordinates = null, string avoidBottomRightCoordinates = null)
        {
            string AppCode = ConfigReaderDAL.ReadSetting("app_code");
            string AppId = ConfigReaderDAL.ReadSetting("app_id");

            string URL = $"https://route.api.here.com/routing/7.2/calculateroute.json?app_id={AppId}&app_code={AppCode}&waypoint0=geo!{startPoint}&waypoint1=geo!{endPoint}&mode=fastest;car;traffic:disabled&avoidareas={avoidTopLeftCoordinates};{avoidBottomRightCoordinates}&routeattributes=shape";

            string APIText = APICall(URL);

            List<string> routeCoordinates = GetCoordinates(APIText);

            return routeCoordinates;
        }

        /// <summary>
        /// This method returns a image url by taking in a list of coordinates and calling map image api from here.com
        /// </summary>
        /// <param name="routeCoordinates">a list of string coordinates that is passed to APICall method</param>
        /// <returns></returns>
        public static string GetMap(List<string> routeCoordinates)
        {
            string routeCoords = "";

            string AppCode = ConfigReaderDAL.ReadSetting("app_code");
            string AppId = ConfigReaderDAL.ReadSetting("app_id");

            foreach (string coordinate in routeCoordinates)
            {
                routeCoords += coordinate + ",";
            }

            return $"https://image.maps.api.here.com/mia/1.6/route?r0={routeCoords}&w=500&app_id={AppId}&app_code={AppCode}";            
        }
        /// <summary>
        /// Parses Json to get coordinates from APIText String
        /// </summary>
        /// <param ="APIText">string APICall Response</param>
        /// <returns> returns a List of strings containing route coordinates </returns>
        public static List<string> GetCoordinates(string APIText)
        {
            List<string> routeCoordinates = new List<string>();

            JToken json = JToken.Parse(APIText);
            List<JToken> jsonTokens = json["response"]["route"][0]["shape"].ToList();

            foreach (JToken token in jsonTokens)
            {
                routeCoordinates.Add(token.ToString());
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