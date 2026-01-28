using BusinessLayer.DTOs;
using DataAccessLayer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ICityService
    {
        // Get Methods
        Task<IEnumerable<CityMaster>> GetAllCitiesAsync();
        Task<IEnumerable<CityMaster>> GetActiveCitiesAsync();
        Task<CityMaster?> GetCityByIdAsync(int cityId);

        // Save / Update / Delete
        Task<bool> CreateCityAsync(CityDto dto);
        Task<bool> UpdateCityAsync(CityDto dto);
        Task<bool> DeleteCityAsync(int cityId);
    }
}