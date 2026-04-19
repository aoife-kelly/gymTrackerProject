using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace gymTracker
{
    internal class BodyPart
    {
        public class Result
        {
            [JsonProperty("id")]
            public string id { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("bodyPart")]
            public string bodyPart { get; set; }

            [JsonProperty("image")]
            public string image { get; set; }
        }

        public class APIResponse
        {
            [JsonProperty("results")]
            public List<Result> results { get; set; }

            [JsonProperty("total")]
            public int total { get; set; }

            [JsonProperty("count")]
            public int count { get; set; }
        }
    }
}
