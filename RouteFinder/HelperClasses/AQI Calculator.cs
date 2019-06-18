using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;


namespace RouteFinder.Models
{
    public class AQI_Calculator
    {
        public enum Pollutant

        {
            PM25, PM10, O3,
        }

        public static int GetBreakPoint(double O3, int PM10, double PM25, DateTime time)
        {
            double O3Min = 0;
            double O3Max = 0;
            int PM10Min = 0;
            int PM10Max = 0;
            double PM25Min = 0.0;
            double PM25Max = 0.0;
            int AQIMin = 0;
            int AQIMax = 0;

            if (O3 >= 0 || O3 < 0.059)
            {
                O3Min = 0;
                O3Max = 0.059;
                PM10Min = 0;
                PM10Max = 54;
                PM25Min = 0;
                PM25Max = 15.4;
                AQIMin = 0;
                AQIMax = 50;
                double EquationResult = ((AQIMax - AQIMin) / (O3Max - O3Min)) * (O3 - O3Min) + AQIMin;
                int AQI = Convert.ToInt32(Math.Round(EquationResult));
            }
            if (O3 >= 0.060 || O3 < 0.075)
            {
                O3Min = 0.060;
                O3Max = 0.075;
                PM10Min = 55;
                PM10Max = 154;
                PM25Min = 15.5;
                PM25Max = 40.4;
                AQIMin = 51;
                AQIMax = 100;
                double EquationResult = ((AQIMax - AQIMin) / (O3Max - O3Min)) * (O3 - O3Min) + AQIMin;
                int AQI = Convert.ToInt32(Math.Round(EquationResult));
            }
            if (O3 >= 0.076 || O3 < 0.095)
            {
                O3Min = 0.076;
                O3Max = 0.095;
                PM10Min = 155;
                PM10Max = 254;
                PM25Min = 40.5;
                PM25Max = 65.4;
                AQIMin = 101;
                AQIMax = 150;
                double EquationResult = ((AQIMax - AQIMin) / (O3Max - O3Min)) * (O3 - O3Min) + AQIMin;
                int AQI = Convert.ToInt32(Math.Round(EquationResult));
            }
            if (O3 >= 0.096 || O3 < 0.115)
            {
                O3Min = 0.096;
                O3Max = 0.115;
                PM10Min = 255;
                PM10Max = 354;
                PM25Min = Math.Pow(65.5, 3);
                PM25Max = Math.Pow(150.4, 3);
                AQIMin = 151;
                AQIMax = 200;
                double EquationResult = ((AQIMax - AQIMin) / (O3Max - O3Min)) * (O3 - O3Min) + AQIMin;
                int AQI = Convert.ToInt32(Math.Round(EquationResult));
            }






            //public  Pollutant(Pollutant pollutant, double concentration)
            //{
            //    if (Pollutant.PM10.equals(pollutant) || Pollutant.SO2.equals(pollutant) || Pollutant.NO2.equals(pollutant))
            //    {
            //        return Math.round(concentration);
            //    }
            //}
        }
    }
