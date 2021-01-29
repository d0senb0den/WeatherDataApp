using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherDataApp.Data;
using WeatherDataApp.Models;
using WeatherDataApp.Entities;
using Microsoft.AspNetCore.Components;

namespace WeatherDataApp.Services
{
    public class FetchDataService : IFetchDataService
    {
        private readonly EfContext _EfContext;
        public FetchDataService(EfContext efContext)
        {
            _EfContext = efContext;
        }


        public double GetDateAvgTemp(string e, string l) // Metod för medeltemperatur för ett specifikt datum, returnerar summan av datan delat på antalet.
        {
            var date = DateTime.Parse(e);
            var dataFromDay = _EfContext.TemperatureReadings
                //.Where(d => d.Date.ToShortDateString() == (string)e.Value && d.Location == l)
                .Where(d => d.Date.Year == date.Year && d.Date.Month == date.Month && d.Date.Day == date.Day && d.Location == l)
                .Select(d => d.Temperature)
                .ToList();

            return Math.Round(dataFromDay.Sum() / dataFromDay.Count(),1);
        }
        
        public List<TemperatureReadingModel> GetData() // Gör uträkning och skapar en modell som skickas till listan ListToAdd.
        {
            List<TemperatureReadingModel> readingModels = new List<TemperatureReadingModel>();

            var dates = _EfContext.TemperatureReadings
                .AsEnumerable()
                .GroupBy(t => t.Date.Date)
                .ToList();

            foreach(var date in dates)
            {
                var dateData = _EfContext.TemperatureReadings
                    .Where(t => t.Date.Date == date.Key);
                CalcAndAddToList(dateData.Where(t => t.Location == "Inne"), readingModels, "Inne", date.Key);
                CalcAndAddToList(dateData.Where(t => t.Location == "Ute"), readingModels, "Ute", date.Key);
            }

            return readingModels;

        }
        private void CalcAndAddToList(IQueryable<TemperatureReading> listToCalc, List<TemperatureReadingModel> listToAdd, string location, DateTime date)
        {
            // Moldrisk. Om uträkningen för moldrisk är mindre än 0 skriver den ut 0, större än 100 skriver den ut 100.

            double moldRisk, avgTemp = 0, avgHumid = 0;

            foreach (var tempReading in listToCalc)
            {
                avgTemp += tempReading.Temperature;
                avgHumid += tempReading.Humidity;
            }
            avgTemp /= listToCalc.Count();
            avgHumid /= listToCalc.Count();
            if((((avgHumid - 78) * (avgTemp / 15)) / 0.22 < 0))
            {
                moldRisk = 0;
            }
            else if ((((avgHumid - 78) * (avgTemp / 15)) / 0.22 > 100))
            {
                moldRisk = 100;
            }
            else
            { 
            moldRisk = ((avgHumid - 78) * (avgTemp / 15)) / 0.22;
            }

            TemperatureReadingModel readingModel = new TemperatureReadingModel()
            {
                AverageTemperature = Math.Round(avgTemp, 1),
                AverageHumidity = (int)avgHumid,
                Date = date,
                Location = location,
                MoldRisk = (int)moldRisk
            };

            listToAdd.Add(readingModel);

        }
    }
}
