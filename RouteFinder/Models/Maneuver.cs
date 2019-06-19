using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RouteFinder.Models
{
    public class Maneuver
    {
        public double TravelTime { get; set; }
        public double Distance { get; set; }
        public string Instruction { get; set; }
    }
}