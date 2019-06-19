using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RouteFinder.Models
{
    public class SensorBoundingBox
    {
        public Sensor Sensor { get; set; }
        public MapPoint NorthWest { get; set; }
        public MapPoint SouthEast { get; set; }

        public SensorBoundingBox(Sensor sensor, MapPoint northWest, MapPoint southEast)
        {
            NorthWest = northWest;
            SouthEast = southEast;
            Sensor = sensor;
        }

        public SensorBoundingBox()
        {

        }

    }
}