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

        public static DataTable ConcentrateTable()
        {
            DataTable concetration = new DataTable();
            concetration.Columns.Add(new DataColumn()
            {
                ColumnName = "O3-8hr",
                DataType = typeof(string)
            });
            concetration.Columns.Add(new DataColumn()
            {
                ColumnName = "03-1hr",
                DataType = typeof(string)
            });
            concetration.Columns.Add(new DataColumn()
            {
                ColumnName = "PM10",
                DataType = typeof(string)
            });
            concetration.Columns.Add(new DataColumn()
            {
                ColumnName = "PM25",
                DataType = typeof(string)
            });
            concetration.Columns.Add(new DataColumn()
            {
                ColumnName = "AQI",
                DataType = typeof(string)
            });
            concetration.Columns.Add(new DataColumn()
            {
                ColumnName = "Category",
                DataType = typeof(string)
            });


            concetration.Rows.Add(new object[] { "0.000 - 0.059", "-", "0-54", "0.0-15.4", "0-50", "Good" });
            concetration.Rows.Add(new object[] { "0.060 - 0.075", "-","55-154", "15.5 -40.4", "51-100", "Moderate" });
            concetration.Rows.Add(new object[] { "0.076 - 0.095", "0.125 - 0.164", "155-254", "40.5-65.4", "101-150", "Unhealthy for Sensitive Groups" });
            concetration.Rows.Add(new object[] { "0.096 - 0.115", "0.165 - 0.204", "255-354", "(65.5 - 150.4)3", "151-200", "Unhealthy" });
            concetration.Rows.Add(new object[] { "0.116 - 0.374", "0.205 - 0.404", "355-424", "(150.5 - (250.4)3","201-300", "Very unhealthy" });
            concetration.Rows.Add(new object[] { "()2", "0.405 - 0.504", "425 - 504", "(250.5-(350.4)3", "301-400", "Hazardous" });
            concetration.Rows.Add(new object[] { "()2", "0.505 - 0.604", "505 - 604", "(350.5 - 500.4)3", "401-500", "Hazardous" });

            return concetration;
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
