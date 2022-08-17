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
}

