using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace JCDecaux.Models
{
    public class Contract
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("commercial_name")]
        public string CommercialName { get; set; }

        [JsonPropertyName("cities")]
        public List<string> Cities { get; set; }

        [JsonPropertyName("country_code")]
        public string CountryCode { get; set; }

        public static Contract? LoadSingleFromJson(string json)
        {
            return JsonConvert.DeserializeObject<Contract>(json);
        }

        public static List<Contract>? LoadListFromJson(string json)
        {
            return JsonConvert.DeserializeObject<List<Contract>>(json);
        }
    }
}
