using System.Text.Json.Serialization;
using GoogleApi.Entities.Common;

namespace JCDecaux.Models
{
    [DataContract]
    public class Location
    {
        [DataMember]
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [DataMember]
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        public override string ToString()
        {
            return $"latitude: {Latitude}\nlongitude: {Longitude}";
        }

        public Coordinate ToCoordinate()
        {
            return new Coordinate(Latitude, Longitude);
        }
    }
}
