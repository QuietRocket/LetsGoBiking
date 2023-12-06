using Apache.NMS;
using Apache.NMS.ActiveMQ;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Maps.Common;
using GoogleApi.Entities.Maps.Common.Enums;
using GoogleApi.Entities.Maps.Directions.Request;
using GoogleApi.Entities.Maps.Directions.Response;
using GoogleApi.Entities.Maps.Geocoding.Address.Request;

namespace Router
{
    [ServiceContract(Namespace = "http://wsdl.letsgobiking.com")]
    public interface IService
    {
        [OperationContract]
        Task<RouteResponse> GetBikeRoute(string origin, string destination);

        [OperationContract]
        Task<RouteResponseWithoutSegments> GetBikeRouteWithQueue(string origin, string destination);
    }

    public class Service : IService
    {
        private readonly JCDecaux.Client _jcDecauxClient;
        private readonly string GoogleMapsApiKey;

        public Service()
        {
            string? apiKey = Environment.GetEnvironmentVariable("JCDECAUX_API_KEY");

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("Missing JCDecaux API key in environment variables");
            }

            _jcDecauxClient = JCDecaux.Client.GetInstance(apiKey);

            apiKey = Environment.GetEnvironmentVariable("GOOGLE_MAPS_API_KEY");

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("Missing Google Maps API key in environment variables");
            }

            GoogleMapsApiKey = apiKey;
        }

        private async Task<Coordinate?> ResolveAddress(string address)
        {
            var request = new AddressGeocodeRequest
            {
                Address = address,
                Key = GoogleMapsApiKey
            };

            var response = await GoogleApi.GoogleMaps.Geocode.AddressGeocode.QueryAsync(request);

            if (response.Status != Status.Ok)
            {
                throw new Exception($"Failed to resolve address {address}");
            }

            var location = response.Results.FirstOrDefault()?.Geometry.Location;

            return location;
        }

        private LocationEx CoordinateToLocationEx(double latitude, double longitude)
        {
            return new LocationEx(new CoordinateEx(latitude, longitude));
        }

        private async Task<DirectionsResponse> ComputePath(Coordinate origin, Coordinate destination, TravelMode travelMode)
        {
            var request = new DirectionsRequest
            {
                Key = GoogleMapsApiKey,
                Origin = CoordinateToLocationEx(origin.Latitude, origin.Longitude),
                Destination = CoordinateToLocationEx(destination.Latitude, destination.Longitude),
                TravelMode = travelMode
            };

            var result = await GoogleApi.GoogleMaps.Directions.QueryAsync(request);

            return result;
        }

        public static double ComputeDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Radius of the earth in km
            var latDistance = ToRadians(lat2 - lat1);
            var lonDistance = ToRadians(lon2 - lon1);
            var a = Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2)
                    + Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2))
                    * Math.Sin(lonDistance / 2) * Math.Sin(lonDistance / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = R * c; // convert to kilometers
            return distance;
        }

        private static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private async Task<JCDecaux.Models.Station?> FindClosestStation(Coordinate coordinate, bool isOrigin)
        {
            var stations = await _jcDecauxClient.GetStations();

            if (stations == null)
            {
                throw new Exception("Failed to get stations");
            }

            var sortedStations = stations.OrderBy(station => ComputeDistance(coordinate.Latitude, coordinate.Longitude, station.Position.Latitude, station.Position.Longitude));
            int limit = 10;
            for (int i = 0; i < Math.Min(sortedStations.Count(), limit); i++)
            {
                var currentStation = await _jcDecauxClient.GetStation(sortedStations.ElementAt(i).Number, sortedStations.ElementAt(i).ContractName);
                if (currentStation == null)
                {
                    throw new Exception("Failed to get station");
                }

                var availability = currentStation.TotalStands.Availability;

                if (isOrigin && availability.Bikes > 0)
                {
                    return currentStation;
                }
                else if (!isOrigin && availability.Stands > 0)
                {
                    return currentStation;
                }
            }

            return null;
        }

        public string ConstructSegmentString(
            string instruction,
            double distance,
            long duration
        )
        {
            return $"{instruction} ({distance} km, {duration} minutes)";
        }

        public async Task<RouteResponse> GetBikeRoute(string origin, string destination)
        {
            var originLocationReq = ResolveAddress(origin);
            var destinationLocationReq = ResolveAddress(destination);

            var originLocation = await originLocationReq;
            var destinationLocation = await destinationLocationReq;

            if (originLocation == null || destinationLocation == null)
            {
                throw new Exception("Failed to resolve origin or destination");
            }

            var originStationReq = FindClosestStation(originLocation, true);
            var destinationStationReq = FindClosestStation(destinationLocation, false);

            var originStation = await originStationReq;
            var destinationStation = await destinationStationReq;

            if (originStation == null || destinationStation == null)
            {
                throw new Exception("Failed to find origin or destination station");
            }

            // Compute path from origin to origin station by walking
            var originToOriginStationReq = ComputePath(originLocation, originStation.Position.ToCoordinate(), TravelMode.Walking);
            // Compute path from origin station to destination station by biking
            var originStationToDestinationStationReq = ComputePath(originStation.Position.ToCoordinate(), destinationStation.Position.ToCoordinate(), TravelMode.Bicycling);
            // Compute path from destination station to destination by walking
            var destinationStationToDestinationReq = ComputePath(destinationStation.Position.ToCoordinate(), destinationLocation, TravelMode.Walking);

            // Compute direct path from origin to destination by walking
            var originToDestinationReq = ComputePath(originLocation, destinationLocation, TravelMode.Walking);


            var originToOriginStation = await originToOriginStationReq;
            var originStationToDestinationStation = await originStationToDestinationStationReq;
            var destinationStationToDestination = await destinationStationToDestinationReq;

            var originToDestination = await originToDestinationReq;

            if (originToOriginStation == null || originStationToDestinationStation == null || destinationStationToDestination == null || originToDestination == null)
            {
                throw new Exception("Failed to compute path");
            }

            var originToOriginStationLeg = originToOriginStation.Routes.FirstOrDefault()?.Legs.FirstOrDefault();
            var originStationToDestinationStationLeg = originStationToDestinationStation.Routes.FirstOrDefault()?.Legs.FirstOrDefault();
            var destinationStationToDestinationLeg = destinationStationToDestination.Routes.FirstOrDefault()?.Legs.FirstOrDefault();
            var originToDestinationLeg = originToDestination.Routes.FirstOrDefault()?.Legs.FirstOrDefault();

            if (originToOriginStationLeg == null || originStationToDestinationStationLeg == null || destinationStationToDestinationLeg == null || originToDestinationLeg == null)
            {
                throw new Exception("Failed to obtain legs");
            }

            var totalDuration = originToOriginStationLeg.Duration.Value + originStationToDestinationStationLeg.Duration.Value + destinationStationToDestinationLeg.Duration.Value;

            if (totalDuration > originToDestinationLeg.Duration.Value)
            {
                var directionSegments = originToDestinationLeg.Steps.Select(step => ConstructSegmentString(step.HtmlInstructions, step.Distance.Value, step.Duration.Value)).ToList();
                directionSegments.Add("Arrived!");

                return new RouteResponse
                {
                    Origin = originToDestinationLeg.StartAddress,
                    Destination = originToDestinationLeg.EndAddress,
                    TotalDistance = originToDestinationLeg.Distance.Value,
                    TotalDuration = originToDestinationLeg.Duration.Value,
                    DirectionSegments = directionSegments.ToArray()
                };
            }
            else
            {
                var directionSegments = new List<string>();

                directionSegments.AddRange(originToOriginStationLeg.Steps.Select(step => ConstructSegmentString(step.HtmlInstructions, step.Distance.Value, step.Duration.Value)));

                directionSegments.Add("Take a bike at " + originStationToDestinationStationLeg.StartAddress);

                directionSegments.AddRange(originStationToDestinationStationLeg.Steps.Select(step => ConstructSegmentString(step.HtmlInstructions, step.Distance.Value, step.Duration.Value)));

                directionSegments.Add("Leave the bike at " + originStationToDestinationStationLeg.EndAddress);

                directionSegments.AddRange(destinationStationToDestinationLeg.Steps.Select(step => ConstructSegmentString(step.HtmlInstructions, step.Distance.Value, step.Duration.Value)));

                directionSegments.Add("Arrived!");

                return new RouteResponse
                {
                    Origin = originToOriginStationLeg.StartAddress,
                    Destination = destinationStationToDestinationLeg.EndAddress,
                    TotalDistance = originToOriginStationLeg.Distance.Value + originStationToDestinationStationLeg.Distance.Value + destinationStationToDestinationLeg.Distance.Value,
                    TotalDuration = originToOriginStationLeg.Duration.Value + originStationToDestinationStationLeg.Duration.Value + destinationStationToDestinationLeg.Duration.Value,
                    DirectionSegments = directionSegments.ToArray()
                };
            }
        }

        public async Task<RouteResponseWithoutSegments> GetBikeRouteWithQueue(string origin, string destination)
        {
            var routeResponse = await GetBikeRoute(origin, destination);
            var routeIdentifier = Guid.NewGuid().ToString();

            // Iterate over each DirectionSegment
            foreach (var segment in routeResponse.DirectionSegments)
            {
                SendToQueue(segment, routeIdentifier);
            }

            return new RouteResponseWithoutSegments
            {
                Origin = routeResponse.Origin,
                Destination = routeResponse.Destination,
                TotalDistance = routeResponse.TotalDistance,
                TotalDuration = routeResponse.TotalDuration,
                RouteIdentifier = routeIdentifier
            };
        }

        // ActiveMQ logic to send messages
        private void SendToQueue(string serializedData, string routeIdentifier)
        {
            IConnectionFactory factory = new ConnectionFactory("activemq:tcp://localhost:61616");
            using (IConnection connection = factory.CreateConnection("artemis", "artemis"))
            using (Apache.NMS.ISession session = connection.CreateSession())
            {
                IDestination destination = session.GetQueue("segments");
                using (IMessageProducer producer = session.CreateProducer(destination))
                {
                    ITextMessage message = session.CreateTextMessage(serializedData);
                    message.NMSCorrelationID = routeIdentifier;
                    producer.Send(message);
                }
            }
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
        public string[] DirectionSegments { get; set; }
    }

    [DataContract]
    public class RouteResponseWithoutSegments
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
        public string RouteIdentifier { get; set; }
    }
}
