using System.Text.Json.Serialization;

namespace JCDecaux.Models
{
    [DataContract]
    public class Availability
    {
        [DataMember]
        [JsonPropertyName("bikes")]
        public int Bikes { get; set; }

        [DataMember]
        [JsonPropertyName("stands")]
        public int Stands { get; set; }

        [DataMember]
        [JsonPropertyName("mechanicalBikes")]
        public int MechanicalBikes { get; set; }

        [DataMember]
        [JsonPropertyName("electricalBikes")]
        public int ElectricalBikes { get; set; }

        [DataMember]
        [JsonPropertyName("electricalInternalBatteryBikes")]
        public int ElectricalInternalBatteryBikes { get; set; }

        [DataMember]
        [JsonPropertyName("electricalRemovableBatteryBikes")]
        public int ElectricalRemovableBatteryBikes { get; set; }

        public int NumberOfBikesAvailable()
        {
            return Bikes +
               MechanicalBikes +
               ElectricalBikes +
               ElectricalInternalBatteryBikes +
               ElectricalRemovableBatteryBikes;
        }

        public int NumberOfStandsAvailable()
        {
            return Stands - NumberOfBikesAvailable();
        }
    }
}
