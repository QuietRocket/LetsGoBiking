using Newtonsoft.Json;

namespace JCDecaux
{
    public class Position
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class Availabilities
    {
        public int Bikes { get; set; }
        public int Stands { get; set; }
        public int MechanicalBikes { get; set; }
        public int ElectricalBikes { get; set; }
        public int ElectricalInternalBatteryBikes { get; set; }
        public int ElectricalRemovableBatteryBikes { get; set; }
    }

    public class Stands
    {
        public required Availabilities Availabilities { get; set; }
        public int Capacity { get; set; }
    }

    public class Station
    {
        public int Number { get; set; }
        public required string ContractName { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required Position Position { get; set; }
        public bool Banking { get; set; }
        public bool Bonus { get; set; }
        public required string Status { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool Connected { get; set; }
        public bool Overflow { get; set; }
        public required object Shape { get; set; }
        public required Stands TotalStands { get; set; }
        public required Stands MainStands { get; set; }
        public required Stands OverflowStands { get; set; }

        public static Station? LoadSingleFromJson(string json)
        {
            return JsonConvert.DeserializeObject<Station>(json);
        }

        public static List<Station>? LoadListFromJson(string json)
        {
            return JsonConvert.DeserializeObject<List<Station>>(json);
        }
    }

    public class Contract
    {
        public required string Name { get; set; }
        public required string CommercialName { get; set; }
        public required string CountryCode { get; set; }
        public required List<string> Cities { get; set; }

        public static Contract? LoadSingleFromJson(string json)
        {
            return JsonConvert.DeserializeObject<Contract>(json);
        }

        public static List<Contract>? LoadListFromJson(string json)
        {
            return JsonConvert.DeserializeObject<List<Contract>>(json);
        }
    }

    public class Park
    {
        public required string ContractName { get; set; }
        public required string Name { get; set; }
        public int Number { get; set; }
        public required string Status { get; set; }
        public required Position Position { get; set; }
        public required string AccessType { get; set; }
        public required string LockerType { get; set; }
        public bool HasSurveillance { get; set; }
        public bool IsFree { get; set; }
        public required string Address { get; set; }
        public required string ZipCode { get; set; }
        public required string City { get; set; }
        public bool IsOffStreet { get; set; }
        public bool HasElectricSupport { get; set; }
        public bool HasPhysicalReception { get; set; }

        public static Park? LoadSingleFromJson(string json)
        {
            return JsonConvert.DeserializeObject<Park>(json);
        }

        public static List<Park>? LoadListFromJson(string json)
        {
            return JsonConvert.DeserializeObject<List<Park>>(json);
        }
    }
}