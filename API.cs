using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static gymTracker.BodyPart;

namespace gymTracker
{
    internal class API
    {
        internal class Program
        {
            static async Task Main(string[] args)
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://gym-fit.p.rapidapi.com/v1/exercises/search?limit=50&offset=0&bodyPart=Chest&equipment=Barbell&type=Compound"),
                    Headers =
    {
        { "x-rapidapi-key", "cbbefe63bcmshf31fcbe6324517bp1741a8jsnb2adc49bdcff" },
        { "x-rapidapi-host", "gym-fit.p.rapidapi.com" },
    },
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();

                    APIResponse apiResponse = JsonConvert.DeserializeObject<APIResponse>(body);
                    List<Result> allExercises = apiResponse.results;

                    foreach (Result exercise in allExercises)
                    {
                        Console.WriteLine(exercise.name);
                    }
                    Console.WriteLine(body);
                }
            }
        }
    }
}