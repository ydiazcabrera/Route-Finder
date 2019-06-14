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
        public static string Coordinates (/*string startPoint,string endPoint,string avoidTopLeftCoordinates,string avoidBottomRightCoordinates*/)
        {
            
            return APICall(URL);

            
        }
    }
} 