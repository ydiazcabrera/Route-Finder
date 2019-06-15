using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace RouteFinder.Models
{
    public class RouteAPIDAL
    {
        //instantiate entity db class of sensors here
        //public List<Sensor> sensors = new List<Sensor>();
        public List<Sensor> sensors = new List<Sensor>
        {
            new Sensor{ Location = "42.9420703, -85.6847243", Name = "106", Type = "OST"},
            new Sensor{ Location = "42.9547237, -85.6824347", Name = "107", Type = "OST"},
            new Sensor{ Location = "42.9274400, -85.6604877", Name = "111", Type = "OST"},
            new Sensor{ Location = "42.984136, -85.671280", Name = "101", Type = "OST"},
            new Sensor{ Location = "42.9372291, -85.6669082", Name = "115", Type = "OST"},
            new Sensor{ Location = "42.92732229883891,-85.64665123059183", Name = "24358c", Type = "SIMMS"},
            new Sensor{ Location = "42.904438,-85.5814071", Name = "232915", Type = "SIMMS"},
            new Sensor{ Location = "42.9414937, -85.658029", Name = "23339e", Type = "SIMMS"},
            new Sensor{ Location = "42.9472356, -85.6822996", Name = "105", Type = "OST"},
            new Sensor{ Location = "42.9201462, -85.6476561", Name = "108", Type = "OST"},
            new Sensor{ Location = "42.984136, -85.671280", Name = "23acbc", Type = "SIMMS"},
            new Sensor{ Location = "42.9467373, -85.6843539", Name = "117", Type = "OST"}
        };

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

            foreach(Sensor sensor in sensors)

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