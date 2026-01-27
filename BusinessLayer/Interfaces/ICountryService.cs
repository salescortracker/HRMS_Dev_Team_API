using BusinessLayer.DTOs;
using DataAccessLayer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ICountryService
    {
        Task<IEnumerable<CountryMaster>> GetAllCountriesAsync();
        Task<CountryMaster?> GetCountryByIdAsync(int id);
        Task<bool> SaveCountryAsync(CountryDto dto);
        Task<bool> UpdateCountryAsync(CountryDto dto); // <-- exact signature

        Task<bool> DeleteCountryAsync(int id);
    }
}