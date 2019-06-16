using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RouteFinder.Models
{
    public class Sensor : MapPoint
    {
        public override string Latitude { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Longitude { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string SensorType { get; set; }

        public Sensor(string latitude, string longitude, string name, string sensorType) : base(latitude, longitude, name)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Name = name;
            this.SensorType = sensorType;
        }
    }
}