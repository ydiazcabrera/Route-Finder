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
        //private double TravelTime;
        //private double Distance;
        //private string Instruction;

        public Maneuver(double travelTime, double distance, string instruction)/* : base(travelTime, distance, instruction)*/
        {
            this.TravelTime = travelTime;
            this.Distance = distance;
            this.Instruction = instruction;
        }


    }
}