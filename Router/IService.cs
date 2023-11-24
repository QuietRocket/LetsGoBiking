using CoreWCF;
using System;
using System.Runtime.Serialization;

namespace Router
{
    [ServiceContract(Namespace = "http://wsdl.letsgobiking.com")]
    public interface IService
    {
        [OperationContract]
        RouteResponse GetBikeRoute(string origin, string destination);
    }

    public class Service : IService
    {
        public RouteResponse GetBikeRoute(string origin, string destination)
        {
            // Implement the logic to compute the bike route
            // This may involve API calls to mapping services or internal algorithms

            // Example return (implement actual logic)
            return new RouteResponse
            {
                Origin = origin,
                Destination = destination,
                TotalDistance = 10.0,
                TotalDuration = 1000,
                DirectionSegments = new DirectionSegment[]
                {
                    new DirectionSegment
                    {
                        Instruction = "Turn left",
                        Distance = 1.0,
                        Duration = 100
                    },
                    new DirectionSegment
                    {
                        Instruction = "Turn right",
                        Distance = 2.0,
                        Duration = 200
                    },
                    new DirectionSegment
                    {
                        Instruction = "Turn left",
                        Distance = 3.0,
                        Duration = 300
                    },
                    new DirectionSegment
                    {
                        Instruction = "Turn right",
                        Distance = 4.0,
                        Duration = 400
                    },
                    new DirectionSegment
                    {
                        Instruction = "Turn left",
                        Distance = 5.0,
                        Duration = 500
                    },
                    new DirectionSegment
                    {
                        Instruction = "Turn right",
                        Distance = 6.0,
                        Duration = 600
                    },
                    new DirectionSegment
                    {
                        Instruction = "Turn left",
                        Distance = 7.0,
                        Duration = 700
                    },
                    new DirectionSegment
                    {
                        Instruction = "Turn right",
                        Distance = 8.0,
                        Duration = 800
                    },
                    new DirectionSegment
                    {
                        Instruction = "Turn left",
                        Distance = 9.0,
                        Duration = 900
                    },
                    new DirectionSegment
                    {
                        Instruction = "Turn right",
                        Distance = 10.0,
                        Duration = 1000
                    }
                }
                // Populate other required fields
            };
        }
    }

    [DataContract]
    public class RouteResponse
    {
        [DataMember]
        public string Origin { get; set; }

        [DataMember]
        public string Destination { get; set; }

        [DataMember]
        public double TotalDistance { get; set; }

        [DataMember]
        public long TotalDuration { get; set; }

        [DataMember]
        public DirectionSegment[] DirectionSegments { get; set; }
    }

    [DataContract]
    public class DirectionSegment
    {
        [DataMember]
        public string Instruction { get; set; }

        [DataMember]
        public double Distance { get; set; }

        [DataMember]
        public long Duration { get; set; }
    }
}
