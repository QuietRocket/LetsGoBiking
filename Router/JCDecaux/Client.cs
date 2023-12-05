using System.Net;

using JCDecaux.Models;

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
            var cachingHandler = new RedisCaching.Handler(innerHandler, "localhost", "LGB");
            httpClient = new HttpClient(cachingHandler);
        }

        // Static method to access the singleton instance
        public static Client GetInstance(string apiKey)
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    instance ??= new Client(apiKey);
                }
            }

            return instance;
        }

        public async Task<List<Contract>> GetContracts()
        {
            string url = $"https://api.jcdecaux.com/vls/v3/contracts?apiKey={apiKey}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cache-TTL", CacheTTLYear);
            HttpResponseMessage response = await httpClient.SendAsync(request);
            string json = await response.Content.ReadAsStringAsync();
            List<Contract>? contracts = Contract.LoadListFromJson(json);
            return contracts ?? new List<Contract>();
        }

        public async Task<Station?> GetStation(int stationNumber, string contractName)
        {
            string url = $"https://api.jcdecaux.com/vls/v3/stations/{stationNumber}?contract={contractName}&apiKey={apiKey}";
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
            string url = $"https://api.jcdecaux.com/vls/v3/stations?apiKey={apiKey}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cache-TTL", CacheTTLYear);
            HttpResponseMessage response = await httpClient.SendAsync(request);
            string json = await response.Content.ReadAsStringAsync();
            List<Station>? stations = Station.LoadListFromJson(json);
            return stations ?? new List<Station>();
        }

        public async Task<List<Station>> GetStationsByContract(string contractName)
        {
            string url = $"https://api.jcdecaux.com/vls/v3/stations?contract={contractName}&apiKey={apiKey}";
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
    }
}