using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RouteFinder.Models
{
    public class RouteCoordinate : MapPoint
    {
        public override string Latitude { get; set; }
        public override string Longitude { get; set; }
        public override string Name { get; set; }

        public RouteCoordinate(string latitude, string longitude, string name) : base(latitude, longitude, name)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Name = name;
        }
    }
}