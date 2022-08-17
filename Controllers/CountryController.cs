using System;
using Microsoft.AspNetCore.Mvc;
using webapi.Dto;
using webapi.Interface;

namespace webapi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CountryController: ControllerBase
    {
        private readonly IRestCountryApi _restCountriesClient;
        public CountryController(IRestCountryApi restCountryApi)
        {
            _restCountriesClient = restCountryApi;
        }

        // /api/country/all
        [HttpGet("all")]        
        public async Task<IEnumerable<Country>> getAll()
        {
            return (IEnumerable<Country>)await _restCountriesClient.GetAll();
        }

        // /api/country/{countryName}
        [HttpGet("{countryName}")]
        public async Task<IEnumerable<Country>> getByName(string countryName)
        {
            return await _restCountriesClient.GetByCountryName(countryName);
        }
    }
}

