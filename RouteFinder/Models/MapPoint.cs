using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RouteFinder.Models
{
    public class MapPoint
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Name { get; set; }

        public MapPoint(string latitude, string longitude, string name)
        {
            Latitude = latitude;
            Longitude = longitude;
            Name = name;
        }

        public MapPoint()
        {

        }
    }
}