using System.Text.Json.Serialization;
using System.Text.Json;

namespace JCDecaux.Models
{
    [DataContract]
    public class Station
    {
        [DataMember]
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [DataMember]
        [JsonPropertyName("status")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public StationStatus Status { get; set; }

        [DataMember]
        [JsonPropertyName("contractName")]
        public string ContractName { get; set; }

        [DataMember]
        [JsonPropertyName("cities")]
        public List<string> Cities { get; set; }

        [DataMember]
        [JsonPropertyName("position")]
        public Location Position { get; set; }

        [DataMember]
        [JsonPropertyName("banking")]
        public bool Banking { get; set; }

        [DataMember]
        [JsonPropertyName("bonus")]
        public bool Bonus { get; set; }

        [DataMember]
        [JsonPropertyName("totalStands")]
        public Stand TotalStands { get; set; }

        public override string ToString()
        {
            return $"number: {Number}\n" +
                $"contactName: {ContractName}\n" +
                $"position: {Position}\n" +
                $"banking: {Banking}\n" +
                $"bonus: {Bonus}\n";
        }

        public static Station? LoadSingleFromJson(string json)
        {
            return JsonSerializer.Deserialize<Station>(json);
        }

        public static List<Station>? LoadListFromJson(string json)
        {
            return JsonSerializer.Deserialize<List<Station>>(json);
        }
    }

    public enum StationStatus
    {
        OPEN,
        CLOSED
    }
}
