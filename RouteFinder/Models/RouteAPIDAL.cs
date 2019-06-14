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
        public static string APICall(string URL)
        {
            HttpWebRequest request = WebRequest.CreateHttp(URL);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader rd = new StreamReader(response.GetResponseStream());

            string APIText = rd.ReadToEnd();

            return APIText;
        }

        public static string Cooridnates (string startDestination,string endDestination,string avoidCoorinates)
        {
            string URL = "";
            APICall(URL);

        }
    }
} 