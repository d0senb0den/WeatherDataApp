using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherDataApp.Models;
using WeatherDataApp.Entities;
using Microsoft.AspNetCore.Components;

namespace WeatherDataApp.Services
{
    public interface IFetchDataService
    {
        double GetDateAvgTemp(string e, string l);

        List<TemperatureReadingModel> GetData();
    }
}
