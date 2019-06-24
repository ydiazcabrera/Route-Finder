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
            this.Instruction = RemoveDistanceInstruction(instruction);
        }

        public string RemoveDistanceInstruction(string instruction)
        {
            try
            {
                int startRemove = instruction.IndexOf("distance");
                string removedInstruction = instruction.Remove(startRemove); //Remove everything starting at "distance"
                string newInstruction = removedInstruction.Remove(removedInstruction.Length - 13); //Remove the extra <span>
                return newInstruction;
            }
            catch (System.ArgumentOutOfRangeException) //The last instruction doesn't have a distance. 
            {
                return (instruction);              
            }
        }
    }
}