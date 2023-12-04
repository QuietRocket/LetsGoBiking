using System.Net;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace RedisCaching
{
    public class Handler : DelegatingHandler
    {
        private readonly IDistributedCache _cache;

        public Handler(
           HttpMessageHandler innerHandler,
           String redisConnectionString,
           String instanceName
        ): base(innerHandler)
        {
            var options = new RedisCacheOptions
            {
                Configuration = redisConnectionString,
                InstanceName = $"{instanceName}:"
            };

            var redisCache = new RedisCache(options);

            _cache = redisCache;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var cacheKey = CreateCacheKey(request);

            var cachedResponse = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (cachedResponse != null)
            {
                var responseResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(cachedResponse)
                };
                return responseResponseMessage;
            }

            var response = await base.SendAsync(request, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var ttl = request.Headers.Contains("Cache-TTL") ? new TimeSpan(0, 0, int.Parse(request.Headers.GetValues("Cache-TTL").First())) : new TimeSpan(0, 0, 300); // Default to 300 seconds if no header
                await _cache.SetStringAsync(cacheKey, responseContent, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl }, cancellationToken);
            }

            return response;
        }

        private string CreateCacheKey(HttpRequestMessage request)
        {
            var key = request.RequestUri?.ToString() ?? "DefaultCacheKey";
            return key;
        }
    }
}