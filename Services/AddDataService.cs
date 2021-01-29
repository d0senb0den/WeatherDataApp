using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WeatherDataApp.Data;
using WeatherDataApp.Models;
using System.Globalization;
using WeatherDataApp.Entities;

namespace WeatherDataApp.Services
{
    public class AddDataService : IAddDataService
    {
        private readonly EfContext _efContext;

        public AddDataService(EfContext efContext) /*Dependency injection*/
        {
            _efContext = efContext;
        }

        public async Task<bool> ConvertAndAdd(IBrowserFile browserFile)
        // Skapar tempfil av filen man skickat in, för att läsa in och sedan ta bort då den inte längre behövs.
        // Kopierar tempfilen till en riktig fil. Ökade värdet på OpenReadStream(20000000) då filen är större än värdet som tillåts.
        {
            var dataList = new List<TemperatureReading>();
            var filePath = Path.GetTempFileName();

            using (var stream = File.Create(filePath))
            {
                await browserFile.OpenReadStream(20000000).CopyToAsync(stream); /**/
            }

            var data = await File.ReadAllLinesAsync(filePath);
            File.Delete(filePath);

            for (int i = 0; i < data.Length; i++) // Inläsning av alla rader från filen. Sparar som entiteter i en lista som vi sedan skickar till databasen och sparar.
            {
                var item = data[i];
                if (item != null)
                {
                    var splittedData = item.Split(",");

                    if (splittedData.Length == 4)
                    {
                        try
                        {
                            TemperatureReading temperatureReading = new TemperatureReading()
                            {
                                Date = DateTime.Parse(splittedData[0].Replace(" ", "T")),
                                Location = splittedData[1],
                                Temperature = double.Parse(splittedData[2], NumberFormatInfo.InvariantInfo),
                                Humidity = int.Parse(splittedData[3])
                            };
                            dataList.Add(temperatureReading);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                            System.Diagnostics.Debug.WriteLine(i + 1);
                            throw;
                        }
                    }
                }
            }
            await _efContext.TemperatureReadings.AddRangeAsync(dataList);
            await _efContext.SaveChangesAsync();
            return true;
        }
    }
}
