using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RouteFinder.Models
{
    public class RouteViewModel
    {
        public Route SafeBikeRoute { get; set; }
        public Route FastBikeRoute { get; set; }
        public Route SafeWalkRoute { get; set; }
        public Route FastWalkRoute { get; set; }
        public List<Sensor> Sensors { get; set; }
        public List<Maneuver> Maneuvers { get; set; }


        public RouteViewModel(Route safeBikeRoute, Route fastBikeRoute, Route safeWalkRoute, Route fastWalkRoute, List<Sensor> sensors)
        {
            SafeBikeRoute = safeBikeRoute;
            FastBikeRoute = fastBikeRoute;
            SafeWalkRoute = safeWalkRoute;
            FastWalkRoute = fastWalkRoute;
            Sensors = sensors;
        }

        public RouteViewModel()
        {

        }
    }
}