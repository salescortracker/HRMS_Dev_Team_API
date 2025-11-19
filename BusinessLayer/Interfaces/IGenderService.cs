using BusinessLayer.DTOs;
using DataAccessLayer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IGenderService
    {
        Task<IEnumerable<GenderDto>> GetAllGendersAsync();
        Task<GenderDto?> GetGenderByIdAsync(int id);
        Task<IEnumerable<GenderDto>> SearchGenderAsync(object filter);
        Task<GenderDto> AddGenderAsync(GenderDto dto);
        Task<GenderDto> UpdateGenderAsync(int id, GenderDto dto);
        Task<bool> DeleteGenderAsync(int id);
        Task<IEnumerable<Gender>> AddGendersAsync(List<GenderDto> dtos);
    }
}
