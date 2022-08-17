using System;
using Microsoft.AspNetCore.Mvc;
using Refit;
using webapi.Dto;
using webapi.Interface;

namespace webapi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CountryController: ControllerBase
    {
        private readonly IRestCountryApi _restCountriesClient;
        private readonly ILogger _logger;

        public CountryController(IRestCountryApi restCountryApi, ILogger<CountryController> logger)
        {
            _restCountriesClient = restCountryApi;
            _logger = logger;
        }

        // /api/country/all
        [HttpGet("all")]        
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
            }
                
            return Ok(response);
        }

        // /api/country/{countryName}
        [HttpGet("{countryName}")]
        public async Task<IEnumerable<Country>> getByName(string countryName)
        {
            _logger.LogInformation($"countryName is {countryName}");

            IEnumerable<Country> response = new List<Country>();

            try
            {
                response = await _restCountriesClient.GetByCountryName(countryName);
            }
            catch (ApiException ex)
            {
                _logger.LogError("===============================================");
                _logger.LogError(ex.RequestMessage.ToString());
                _logger.LogError(ex.StatusCode.ToString());
                _logger.LogError(ex.ToString());
                _logger.LogError("===============================================");
            }
            return response;
        }

        // /api/country/currency/{currencyName}
        [HttpGet("currency/{currencyName}")]
        public async Task<IEnumerable<Country>> getByCurrencyName(string currencyName)
        {
            _logger.LogInformation($"currencyName is {currencyName}");
            //_logger.LogInformation("currencyName is {0}", currencyName);
            IEnumerable<Country> response = new List<Country>();
            try
            {
                response = await _restCountriesClient.GetByCurrencyName(currencyName);
            }
            catch (ApiException ex)
            {
                _logger.LogError("===============================================");
                _logger.LogError(ex.RequestMessage.ToString());
                _logger.LogError(ex.StatusCode.ToString());
                _logger.LogError(ex.ToString());
                _logger.LogError("===============================================");
            }                        

            return response;
        }
    }
}

