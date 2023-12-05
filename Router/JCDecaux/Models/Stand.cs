using System.Text.Json.Serialization;

namespace JCDecaux.Models
{
    [DataContract]
    public class Stand
    {
        [DataMember]
        [JsonPropertyName("availabilities")]
        public Availability Availability { get; set; }
    }
}
