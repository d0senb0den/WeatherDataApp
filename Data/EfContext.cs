using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherDataApp.Models;
using WeatherDataApp.Entities;

namespace WeatherDataApp.Data
{
    public class EfContext : DbContext
    {
        public EfContext(DbContextOptions options) : base(options) { }
        public DbSet<TemperatureReading> TemperatureReadings { get; set; }
    }
}
