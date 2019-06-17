using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RouteFinder.Models
{
    public class Sensor : MapPoint
    {
        public int Id { get; set; }
        public override string Latitude { get; set; }
        public override string Longitude { get; set; }
        public override string Name { get; set; }
        public string DeviceType { get; set; }
        public int? AQI { get; set; }

        public Sensor(string latitude, string longitude, string name, string deviceType) : base(latitude, longitude, name)
        {
            Latitude = latitude;
            Longitude = longitude;
            Name = name;
            DeviceType = deviceType;
        }
    }
}