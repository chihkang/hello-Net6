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



## Using Duende IdentityServer to protect API endpoint

https://demo.duendesoftware.com

https://docs.duendesoftware.com/identityserver/v6/apis/aspnetcore/jwt/

### Add Package

```txt
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />
```

### Program.cs

https://docs.duendesoftware.com/identityserver/v6/apis/aspnetcore/jwt/

```c#
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // base-address of your identityserver
                options.Authority = "https://demo.duendesoftware.com";

                // audience is optional, make sure you read the following paragraphs
                // to understand your options
                options.TokenValidationParameters.ValidateAudience = false;
                //options.Audience = "api1";

                // it's recommended to check the type header to avoid "JWT confusion" attacks
                options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
            });
builder.Services.AddAuthorization(options =>
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "api");
    })
);
```

### CountryController.cs

Add `[Authorize(policy: "ApiScope")]`to protect endpoint

```c#
// /api/country/all        
        [HttpGet("all")]
        [Authorize(policy: "ApiScope")]
        public async Task<IActionResult> getAllAsync()
        {
            IEnumerable<Country> response = new List<Country>();
            try
            {
                response = await _restCountriesClient.GetAll();
            }
            catch (ApiException ex)
            {
                _logger.LogError("===============================================");
                _logger.LogError(ex.RequestMessage.ToString());
                _logger.LogError(ex.StatusCode.ToString());
                _logger.LogError(ex.ToString());
                _logger.LogError("===============================================");
                return NotFound(response);
            }
                
            return Ok(response);
        }
```

### TokenController

#### Demo Clients

- **client id: m2m** 
  grant type: client credentials 
  client secret: secret 
  access token lifetime: 60 minutes
  allowed scopes: api
- **client id: m2m.jwt** 
  grant type: client credentials 
  client secret: private key JWT 
  access token lifetime: 60 minutes
  allowed scopes: api

Use demo DuendeSoftware server for demo.

https://demo.duendesoftware.com

use postman to retrieve token

![](https://i.imgur.com/rK4gGtx.png)

You could get accessToken like below

```txt
eyJhbGciOiJSUzI1NiIsImtpZCI6IjQ5NzM0OUREMDU3QjJCQzcxNkNFRkZBQUJFRTM4Q0QzIiwidHlwIjoiYXQrand0In0.eyJpc3MiOiJodHRwczovL2RlbW8uZHVlbmRlc29mdHdhcmUuY29tIiwibmJmIjoxNjYwNzI2MzI5LCJpYXQiOjE2NjA3MjYzMjksImV4cCI6MTY2MDcyOTkyOSwiYXVkIjoiYXBpIiwic2NvcGUiOlsiYXBpIl0sImNsaWVudF9pZCI6Im0ybSIsImp0aSI6IjM5QzJCMkQ5MEMyMUE3RkFGMUZCQ0Q0NzQ2REY1Mzg3In0.lOdx1_yXPCUE_KcNuPQ2sKx_KL9JhYb1BRSzoeKUqMojEvZx9nDE9c5tbWomps9wWJFhRhGU_GbY4l4dvGg_AuHslSBO_vNnOMR17wb2Ri9EuZ3kf34rZaslz6gc3D_gnr5QtxZvTBAr1k-FZKvrkxo-0kbPLAPvU6Vf_EQ1Gwwn7BKFV6wtnqVI84VOkU_skYtP67r6cW35m9E8JlJzAxibUJ8ZaSRLCCQ-1lhupKChX1vuBYagI4lGT8zz_xAOuvIHn9KgOJ5QoEfn6KJ3P7eSNzsJMZR_cTuUumJ0rJkGKJjemGRi2VjVMf1X9szmx_iWOtVSpY_zePnvrHDcnA
```

Use accessToken as BearToken could access protected API endpoint.

#### use m2m.jwt to access endpoint

reterive access token from below endpoint

http://localhost:5000/api/token/accesstoken

```txt
eyJhbGciOiJSUzI1NiIsImtpZCI6IjQ5NzM0OUREMDU3QjJCQzcxNkNFRkZBQUJFRTM4Q0QzIiwidHlwIjoiYXQrand0In0.eyJpc3MiOiJodHRwczovL2RlbW8uZHVlbmRlc29mdHdhcmUuY29tIiwibmJmIjoxNjYwNzI2ODkxLCJpYXQiOjE2NjA3MjY4OTEsImV4cCI6MTY2MDczMDQ5MSwiYXVkIjoiYXBpIiwic2NvcGUiOlsiYXBpIl0sImNsaWVudF9pZCI6Im0ybS5qd3QiLCJqdGkiOiIwQkY1MUI1MzNGRkRDODdBNEVEMjE5N0FDQkRFNTQ4OSJ9.UCeYuMuxxE_8GHJteGCQIPT40xQN4wIsAgveTgqJN1fKDGSwMm2yvZvrlEOSqVfXakbKs5pADQER2NT3TwtjxKf9odcGZStWeRQ2ocaksOYFo8kJOC8pORrnWA8JhnpxXClP6KtufL-tfE7Ww27Y6iBR-VUA5fcy4omwjjIERkRTcHs2TUe9-Cf5PBTQz3kE2mi_iklzm9epFkz88MKPBWTRf-eynNn_bbCzeltjUpsFQEG4t0kHuXuef0ki2ozHgZ_564f0Dnpb5Tq0hxyAGcW-cnI22GhFO81SIEyu18QLiy0f2mJxkLxEb6TPgcDgMaPjTNTTvJ_0rCBJfi37kg
```

Use accessToken as BearToken could access protected API endpoint.

#### TokenController.cs

```c#
using System;
using IdentityModel;
using IdentityModel.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using webapi.Shared;

namespace webapi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class TokenController: ControllerBase
    {
        private readonly ILogger _logger;
        private static string rsaKey = "{'d':'GmiaucNIzdvsEzGjZjd43SDToy1pz-Ph-shsOUXXh-dsYNGftITGerp8bO1iryXh_zUEo8oDK3r1y4klTonQ6bLsWw4ogjLPmL3yiqsoSjJa1G2Ymh_RY_sFZLLXAcrmpbzdWIAkgkHSZTaliL6g57vA7gxvd8L4s82wgGer_JmURI0ECbaCg98JVS0Srtf9GeTRHoX4foLWKc1Vq6NHthzqRMLZe-aRBNU9IMvXNd7kCcIbHCM3GTD_8cFj135nBPP2HOgC_ZXI1txsEf-djqJj8W5vaM7ViKU28IDv1gZGH3CatoysYx6jv1XJVvb2PH8RbFKbJmeyUm3Wvo-rgQ','dp':'YNjVBTCIwZD65WCht5ve06vnBLP_Po1NtL_4lkholmPzJ5jbLYBU8f5foNp8DVJBdFQW7wcLmx85-NC5Pl1ZeyA-Ecbw4fDraa5Z4wUKlF0LT6VV79rfOF19y8kwf6MigyrDqMLcH_CRnRGg5NfDsijlZXffINGuxg6wWzhiqqE','dq':'LfMDQbvTFNngkZjKkN2CBh5_MBG6Yrmfy4kWA8IC2HQqID5FtreiY2MTAwoDcoINfh3S5CItpuq94tlB2t-VUv8wunhbngHiB5xUprwGAAnwJ3DL39D2m43i_3YP-UO1TgZQUAOh7Jrd4foatpatTvBtY3F1DrCrUKE5Kkn770M','e':'AQAB','kid':'ZzAjSnraU3bkWGnnAqLapYGpTyNfLbjbzgAPbbW2GEA','kty':'RSA','n':'wWwQFtSzeRjjerpEM5Rmqz_DsNaZ9S1Bw6UbZkDLowuuTCjBWUax0vBMMxdy6XjEEK4Oq9lKMvx9JzjmeJf1knoqSNrox3Ka0rnxXpNAz6sATvme8p9mTXyp0cX4lF4U2J54xa2_S9NF5QWvpXvBeC4GAJx7QaSw4zrUkrc6XyaAiFnLhQEwKJCwUw4NOqIuYvYp_IXhw-5Ti_icDlZS-282PcccnBeOcX7vc21pozibIdmZJKqXNsL1Ibx5Nkx1F1jLnekJAmdaACDjYRLL_6n3W4wUp19UvzB1lGtXcJKLLkqB6YDiZNu16OSiSprfmrRXvYmvD8m6Fnl5aetgKw','p':'7enorp9Pm9XSHaCvQyENcvdU99WCPbnp8vc0KnY_0g9UdX4ZDH07JwKu6DQEwfmUA1qspC-e_KFWTl3x0-I2eJRnHjLOoLrTjrVSBRhBMGEH5PvtZTTThnIY2LReH-6EhceGvcsJ_MhNDUEZLykiH1OnKhmRuvSdhi8oiETqtPE','q':'0CBLGi_kRPLqI8yfVkpBbA9zkCAshgrWWn9hsq6a7Zl2LcLaLBRUxH0q1jWnXgeJh9o5v8sYGXwhbrmuypw7kJ0uA3OgEzSsNvX5Ay3R9sNel-3Mqm8Me5OfWWvmTEBOci8RwHstdR-7b9ZT13jk-dsZI7OlV_uBja1ny9Nz9ts','qi':'pG6J4dcUDrDndMxa-ee1yG4KjZqqyCQcmPAfqklI2LmnpRIjcK78scclvpboI3JQyg6RCEKVMwAhVtQM6cBcIO3JrHgqeYDblp5wXHjto70HVW6Z8kBruNx1AH9E8LzNvSRL-JVTFzBkJuNgzKQfD0G77tQRgJ-Ri7qu3_9o1M4'}\n";
        private readonly IHttpClientFactory _httpClientFactory;

        public TokenController(ILogger<TokenController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        // /api/country/privatekey/
        [HttpGet("accessToken")]
        public async Task<IActionResult> getAccessToken()
        {

            var jwk = new JsonWebKey(rsaKey);
            var response = await RequestTokenAsync(new SigningCredentials(jwk, "RS256"));

            return Ok(response.AccessToken);
        }
        private async Task<TokenResponse> RequestTokenAsync(SigningCredentials signingCredentials)
        {
            var client = _httpClientFactory.CreateClient();

            var disco = await client.GetDiscoveryDocumentAsync(Urls.IdentityServer);
            if (disco.IsError) throw new Exception(disco.Error);

            var clientToken = CreateClientToken(signingCredentials, "m2m.jwt", disco.TokenEndpoint);

            _logger.LogInformation($"clientToken is: {clientToken}");

            var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientAssertion =
                {
                    Type = OidcConstants.ClientAssertionTypes.JwtBearer,
                    Value = clientToken
                },

                Scope = "api",                
            });

            if (response.IsError) throw new Exception(response.Error);
            return response;
        }

        private static string CreateClientToken(SigningCredentials credential, string clientId, string audience)
        {
            var now = DateTime.UtcNow;

            var token = new JwtSecurityToken(
                clientId,
                audience,
                new List<Claim>()
                {
                    new Claim(JwtClaimTypes.JwtId, Guid.NewGuid().ToString()),
                    new Claim(JwtClaimTypes.Subject, clientId),
                    new Claim(JwtClaimTypes.IssuedAt, now.ToEpochTime().ToString(), ClaimValueTypes.Integer64)
                },
                now,
                now.AddMinutes(1),
                credential
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}

```

> rsaKey is for demo , do not use in production.