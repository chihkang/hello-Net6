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
        [Get("/all")]
        Task<IEnumerable<Country>> GetAll();
    }
}

