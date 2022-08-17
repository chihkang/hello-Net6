using System;
using Refit;
using System.Collections.Generic;
using System.Collections;
using webapi.Dto;

namespace webapi.Interface
{
    [Headers("Content-Type: application/json")]
    public interface IRestCountryApi
    {
        // https://restcountries.com/v3.1/all
        [Get("/all")]
        Task<IEnumerable<Country>> GetAll();

        // https://restcountries.com/v3.1/name/{name}
        [Get("/name/{name}")]
        Task<IEnumerable<Country>> GetByCountryName([AliasAs("name")] string countryName);

        // https://restcountries.com/v3.1/currency/{currency}
        [Get("/currency/{name}")]
        Task<IEnumerable<Country>> GetByCurrencyName([AliasAs("name")] string currencyName);
    }
}

