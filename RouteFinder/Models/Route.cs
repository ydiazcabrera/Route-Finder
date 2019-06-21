using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RouteFinder.Models
{
    public class Route
    {
        public string ModeOfTransportation { get; set; }
        public double TotalTravelTime { get; set; }
        public double TotalDistance { get; set; }
        public List<Maneuver> Maneuvers { get; set; }
        public List<RouteCoordinate> RouteCoordinates { get; set; }
        public string RouteCoordinatesString { get; set; }
    }
}