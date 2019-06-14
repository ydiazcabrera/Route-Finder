using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace RouteFinder.Models
{

    public class RouteAPIDAL
    {
        //API Call returns route as string 
        public static string APICall(string URL)
        {
            HttpWebRequest request = WebRequest.CreateHttp(URL);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader rd = new StreamReader(response.GetResponseStream());

            string APIText = rd.ReadToEnd();

            return APIText;
        }

        //Method to get route coordinates based on user input and a square to avoid
        public static string DisplayMap(string startPoint, string endPoint, string avoidTopLeftCoordinates, string avoidBottomRightCoordinates)
        {
            List<string> routeCoordinates = GetRoute(startPoint, endPoint, avoidTopLeftCoordinates, avoidBottomRightCoordinates);
            string mapImage = GetMap(routeCoordinates);
            return mapImage;
        }

        //Builds URL and calls API with that URL
        public static List<string> GetRoute(string startPoint, string endPoint, string avoidTopLeftCoordinates, string avoidBottomRightCoordinates)
        {
            string AppCode = ConfigReaderDAL.ReadSetting("app_code");
            string AppId = ConfigReaderDAL.ReadSetting("app_id");
            string URL = $"https://route.api.here.com/routing/7.2/calculateroute.json?app_id={AppId}&app_code={AppCode}&waypoint0=geo!{startPoint}&waypoint1=geo!{endPoint}&mode=fastest;car;traffic:disabled&avoidareas={avoidTopLeftCoordinates};{avoidBottomRightCoordinates}&routeattributes=shape";
            string APIText = APICall(URL);
            List<string> routeCoordinates = GetCoordinates(APIText);
            return routeCoordinates;
        }

        /// <summary>
        /// Parses Json to get coordinates from APIText String
        /// </summary>
        /// <param ="APIText">string APICall Response</param>
        /// <returns> returns a List of strings containing route coordinates </returns>
        public static List<string> GetCoordinates(string APIText)
        {
            JToken json = JToken.Parse(APIText);
            List<JToken> jsonTokens = json["route"].ToList();/*["shape"]*/
            List<string> routeCoordinates = new List<string>();
            foreach (JToken token in jsonTokens)
            {
                routeCoordinates.Add(token.ToString());

      
            }
            return routeCoordinates;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeCoordinates"></param>
        /// <returns></returns>
        public static string GetMap(List<string> routeCoordinates)
        {
            string AppCode = ConfigReaderDAL.ReadSetting("app_code");
            string AppId = ConfigReaderDAL.ReadSetting("app_id");
            string routeCoords = "";
            foreach (string coordinate in routeCoordinates)
            {
                routeCoords += coordinate + ",";
            }
            string imgTag = $"https://image.maps.api.here.com/mia/1.6/route?r0={routeCoords}&w=500&app_id={AppId}&app_code={AppCode}";
            return imgTag;
        }
    }
} 