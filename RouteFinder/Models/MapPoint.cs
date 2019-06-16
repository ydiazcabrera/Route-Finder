using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RouteFinder.Models
{
    public abstract class MapPoint
    {
        public abstract string Latitude { get; set; }
        public abstract string Longitude { get; set; }
        public abstract string Name { get; set; }

        public MapPoint(string latitude, string longitude, string name)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Name = name;
        }
    }
}