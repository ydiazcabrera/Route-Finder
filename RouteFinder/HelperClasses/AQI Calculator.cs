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
        public AQI_Calculator()
        {

        }
        public static double GetHourlyAvg(List<SensorsData> sensorData, string pollutant)
        {
            double dataPoint = 0.00;
            //List<int> AQIs = new List<int>();

            int datarows = sensorData.Count();
            double runningTotal = 0.00;

            foreach (SensorsData s in sensorData)
            {
                if (pollutant == "O3_PPB")
                {
                    dataPoint = (double)s.O3_PPB;
                    runningTotal += (dataPoint / 1000); // convert from ppb to ppm
                }
                else if (pollutant == "PM25_MicroGramPerCubicMeter")
                {
                    dataPoint = (int)s.PM25_MicroGramPerCubicMeter;
                    runningTotal += dataPoint;
                }
            }

            return runningTotal / sensorData.Count();
        }

        public static int CalcluateO3AQI(double avgO3)
        {
            double O3Min = 0;
            double O3Max = 0;
            int AQIMin = 0;
            int AQIMax = 0;

            if (avgO3 >= 0 && avgO3 < 0.059)
            {
                O3Min = 0;
                O3Max = 0.059;
                AQIMin = 0;
                AQIMax = 50;
            }
            else if (avgO3 >= 0.060 && avgO3 < 0.075)
            {
                O3Min = 0.060;
                O3Max = 0.075;
                AQIMin = 51;
                AQIMax = 100;
            }
            else if (avgO3 >= 0.076 && avgO3 < 0.095)
            {
                O3Min = 0.076;
                O3Max = 0.095;
                AQIMin = 101;
                AQIMax = 150;

            }
            else if (avgO3 >= 0.096 && avgO3 < 0.115)
            {
                O3Min = 0.096;
                O3Max = 0.115;
                AQIMin = 151;
                AQIMax = 200;
            }
            else if (avgO3 >= 0.116 && avgO3 < 0.374)
            {
                O3Min = 0.116;
                O3Max = 0.374;
                AQIMin = 201;
                AQIMax = 300;
            }
            else if (avgO3 >= 0.405)// this is a combination of two hazardous reading, might need to change
            {
                O3Min = 0.405;
                O3Max = 0.604;
                AQIMin = 301;
                AQIMax = 500;
            }

            int AQI = CalculateAQI(AQIMax, AQIMin, O3Max, O3Min, avgO3);
            return AQI;
        }

        public static int CalcluatePM25AQI(double avgPM25)
        {
            double PM25Min = 0;
            double PM25Max = 0;
            int AQIMin = 0;
            int AQIMax = 0;

            if (avgPM25 >= 0 && avgPM25 < 15.4)
            {
                PM25Min = 0;
                PM25Max = 15.4;
                AQIMin = 0;
                AQIMax = 50;
            }
            else if (avgPM25 >= 15.5 && avgPM25 < 40.4)
            {
                PM25Min = 15.5;
                PM25Max = 40.4;
                AQIMin = 51;
                AQIMax = 100;
            }
            else if (avgPM25 >= 40.5 && avgPM25 < 65.4)
            {
                PM25Min = 40.5;
                PM25Max = 65.4;
                AQIMin = 101;
                AQIMax = 150;
            }
            else if (avgPM25 >= 65.5 && avgPM25 < 150.4)
            {
                PM25Min = 65.5;
                PM25Max = 150.4;
                AQIMin = 151;
                AQIMax = 200;
            }
            else if (avgPM25 >= 150.5 && avgPM25 < 250.4)
            {
                PM25Min = 150.5;
                PM25Max = 250.4;
                AQIMin = 201;
                AQIMax = 300;
            }
            else if (avgPM25 >= 250.5 && avgPM25 < 350.4)
            {
                PM25Min = 250.5;
                PM25Max = 350.4;
                AQIMin = 301;
                AQIMax = 400;
            }
            else if (avgPM25 >= 350.5)
            {
                PM25Min = 350.5;
                PM25Max = 500.4;
                AQIMin = 401;
                AQIMax = 500;
            }

            int AQI = CalculateAQI(AQIMax, AQIMin, PM25Max, PM25Min, avgPM25);
            return AQI;
        }

        public static int CalculateAQI(int aqiMax, int aquMin, double pollutantMax, double pollutantMin, double pollutantReading)
        {
            int AQI = Convert.ToInt32((((aqiMax - aquMin) / (pollutantMax - pollutantMin)) * (pollutantReading - pollutantMin)) + aquMin);
            return AQI;
        }
    }
}
