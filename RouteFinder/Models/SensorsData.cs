using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RouteFinder.Models
{
    public class SensorsData
    {
        public int Id { get; set; }
        public int SensorId { get; set; }
        public DateTime? Time { get; set; }
        public int? O3_PPB { get; set; }
        public int? PM10Avg_MicroGramPerCubicMeter { get; set; }
        public int? PM1Avg_MicroGramPerCubicMeter { get; set; }
        public int? PM25_MicroGramPerCubicMeter { get; set; }
        public int? PM25Avg_MicroGramPerCubicMeter { get; set; }
        public int? PM25Count_MicroGramPerCubicMeter { get; set; }
        public int? PM25Max_MicroGramPerCubicMeter { get; set; }
        public int? PM25Min_MicroGramPerCubicMeter { get; set; }
        public int? PMStatus { get; set; }
        public int? Port { get; set; }
        public int? SpecHumid { get; set; }
        public int? SpecO3Avg_MicroGramPerCubicMeter { get; set; }
        public int? SpecO3Count_MicroGramPerCubicMeter { get; set; }
        public int? SpecO3Max_MicroGramPerCubicMeter { get; set; }
        public int? SpecO3Min_MicroGramPerCubicMeter { get; set; }
        public int? SpecStatus { get; set; }
        public int? SpecTemo_Celcius { get;set; }

        public int? CO { get; set; }
        public int? Humidity { get; set; }
        public int? NO2_PPB { get; set; }
        public int? NO2_O3_PPB { get; set; }
        public int? SO2 { get; set; }
        public int? Status { get; set; }
        public int? Temperature { get; set; }
        public int? Vin { get; set; }
    }
}