﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RouteFinder.Models
{
    public class Sensor : MapPoint
    {
        //public string Latitude { get; set; }
        //public string Longitude { get; set; }
        //public string Name { get; set; }
        public override string Latitude { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Longitude { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string SensorType { get; set; }
    }
}