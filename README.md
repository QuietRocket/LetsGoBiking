# LetsGoBiking
Full-stack Bike-focused Navigation App and Service


## Architecture

```
.
├── Client
├── Router
└── docker-compose.yml
```

- Redis is used as a cache.

- ActiveMQ is used as a queue.

- The Router is a CoreWCF API server.
    - The router contains two callable methods from WCF:
        - `RouteResponse GetBikeRoute(string origin, string destination);``
            - Obtain the detailed directions directly from the SOAP request.
        - `RouteResponseWithoutSegments GetBikeRouteWithQueue(string origin, string destination);``
            - Obtain the directions from a queue.
            - An identifier is contained within the response so that the client to poll the queue for the results.
    - The router uses a custom handler for HTTP requests.
        - The handler intercepts a Cache TLL header and caches the response in Redis.

- The Client is a Java heavy client.
    - It connects to the Router to send requests and receive responses.
    - It connects to ActiveMQ to poll for the detailed directions.

## How to run
1. Add JCDecaux API key and Google Maps API key to `Router/Properties/launchSettings.json`
2. Start Redis and ActiveMQ
```
docker compose build
```
then run them
```
docker compose up
```
3. Run the Router
```
dotnet run --project Router/Router.csproj
```
4. Run the Client
```
cd Client && mvn clean package && mvn exec:exec
```