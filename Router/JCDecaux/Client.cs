using System.Net;

namespace JCDecaux
{
    public class Client
    {
        private readonly string apiKey;
        private readonly HttpClient httpClient;

        private const string CacheTTLYear = "31536000";
        private const string CacheTTLShort = "300";

        // Singleton instance
        private static Client? instance = null;

        // Lock object for thread safety
        private static readonly object lockObject = new object();

        private Client(string apiKey)
        {
            this.apiKey = apiKey;
            var innerHandler = new HttpClientHandler();
            var cachingHandler = new RedisCaching.Handler(innerHandler);
            httpClient = new HttpClient(cachingHandler);
        }

        // Static method to access the singleton instance
        public static Client GetInstance(string apiKey)
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new Client(apiKey);
                    }
                }
            }

            return instance;
        }

        public async Task<List<Contract>> GetContracts()
        {
            string url = $"https://api.jcdecaux.com/vls/v1/contracts?apiKey={apiKey}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cache-TTL", CacheTTLYear);
            HttpResponseMessage response = await httpClient.SendAsync(request);
            string json = await response.Content.ReadAsStringAsync();
            List<Contract>? contracts = Contract.LoadListFromJson(json);
            return contracts ?? new List<Contract>();
        }

        public async Task<Station?> GetStation(int stationNumber, string contractName)
        {
            string url = $"https://api.jcdecaux.com/vls/v1/stations/{stationNumber}?contract={contractName}&apiKey={apiKey}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cache-TTL", CacheTTLShort);
            HttpResponseMessage response = await httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            string json = await response.Content.ReadAsStringAsync();
            Station? station = Station.LoadSingleFromJson(json);
            return station;
        }

        public async Task<List<Station>> GetStations()
        {
            string url = $"https://api.jcdecaux.com/vls/v1/stations?apiKey={apiKey}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cache-TTL", CacheTTLYear);
            HttpResponseMessage response = await httpClient.SendAsync(request);
            string json = await response.Content.ReadAsStringAsync();
            List<Station>? stations = Station.LoadListFromJson(json);
            return stations ?? new List<Station>();
        }

        public async Task<List<Station>> GetStationsByContract(string contractName)
        {
            string url = $"https://api.jcdecaux.com/vls/v1/stations?contract={contractName}&apiKey={apiKey}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cache-TTL", CacheTTLShort);
            HttpResponseMessage response = await httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new ArgumentException("Invalid contract name", nameof(contractName));
            }

            string json = await response.Content.ReadAsStringAsync();
            List<Station>? stations = Station.LoadListFromJson(json);
            return stations ?? new List<Station>();
        }

        public async Task<List<Park>> GetParksByContract(string contractName)
        {
            string url = $"https://api.jcdecaux.com/parking/v1/contracts/{contractName}/parks?apiKey={apiKey}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cache-TTL", CacheTTLShort);
            HttpResponseMessage response = await httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new ArgumentException("Invalid contract name or contract does not support parks API", nameof(contractName));
            }

            string json = await response.Content.ReadAsStringAsync();
            List<Park>? parks = Park.LoadListFromJson(json);
            return parks ?? new List<Park>();
        }

        public async Task<Park?> GetPark(string contractName, int parkNumber)
        {
            string url = $"https://api.jcdecaux.com/parking/v1/contracts/{contractName}/parks/{parkNumber}?apiKey={apiKey}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cache-TTL", CacheTTLShort);
            HttpResponseMessage response = await httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            string json = await response.Content.ReadAsStringAsync();
            Park? park = Park.LoadSingleFromJson(json);
            return park;
        }
    }
}