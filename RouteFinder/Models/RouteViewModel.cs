using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RouteFinder.Models
{
    public class RouteViewModel
    {
        public Route SafeRoute { get; set; }
        public Route FastRoute { get; set; }
        public List<Sensor> Sensors { get; set; }

        public RouteViewModel(Route safeRoute, Route fastRoute, List<Sensor> sensors)
        {
            SafeRoute = safeRoute;
            FastRoute = fastRoute;
            Sensors = sensors;
        }
    }
}