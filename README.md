[![.NET](https://github.com/chihkang/hello-Net6/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/chihkang/hello-Net6/actions/workflows/dotnet.yml)

# hello-Net6

## Endpoint

```
http://localhost:5000/WeatherForecast
```

## Swagger

```
http://localhost:5000/swagger/index.html
```

## Docker build

```
docker build -t chihkang/net6:main .
```

## Docker run

```
docker run --rm -p 5000:5000 chihkang/net6:main
```

## Using Refit to consume restcountries service

### Add package

```
Refit.HttpClientFactory
Refit
```

### Define interface

```
[Headers("Content-Type: application/json")]
    public interface IRestCountryApi
    {
        [Get("/all")]
        Task<IEnumerable<Country>> GetAll();
    }
```

### Builder service setting

```
builder.Services
        .AddRefitClient<IRestCountryApi>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://restcountries.com/v3.1"));
```

### DI inject & using

```
[ApiController]
    [Route("/api/[controller]")]
    public class CountryController: ControllerBase
    {
        private readonly IRestCountryApi _IRestCountryApi;
        public CountryController(IRestCountryApi restCountryApi)
        {
            _IRestCountryApi = restCountryApi;
        }

        // /api/country/all
        [HttpGet("all")]        
        public async Task<IEnumerable<Country>> getAll()
        {
            return (IEnumerable<Country>)await _IRestCountryApi.GetAll();
        }
    }
```
