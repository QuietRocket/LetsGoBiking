using System.Text.Json.Serialization;
using System.Text.Json;

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
            return JsonSerializer.Deserialize<Contract>(json);
        }

        public static List<Contract>? LoadListFromJson(string json)
        {
            return JsonSerializer.Deserialize<List<Contract>>(json);
        }
    }
}
