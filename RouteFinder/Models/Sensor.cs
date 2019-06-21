using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RouteFinder.Models
{
    public class Sensor
    {
        [Key]
        public int SensorId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Name { get; set; }
        public string DeviceType { get; set; }
        public int? AQI { get; set; }
        [NotMapped]
        public SensorBoundingBox BoundingBox { get; set; }

        public Sensor(string latitude, string longitude, string name, string deviceType)
        {
            Latitude = latitude;
            Longitude = longitude;
            Name = name;
            DeviceType = deviceType;
        }

        public Sensor()
        {

        }
    }

    public class SensorDbContext : DbContext
    {
        public SensorDbContext() : base("SeamlessSensorData")
        {

        }

        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<SensorsData> SensorsData { get; set; }
    }
}