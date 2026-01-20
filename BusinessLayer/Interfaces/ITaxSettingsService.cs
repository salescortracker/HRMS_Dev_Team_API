using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ITaxSettingsService
    {
        Task<List<TaxTypeDto>> GetTaxTypesAsync();
        Task<bool> SaveTaxSettingsAsync(TaxSettingsDto dto);

        // Full CRUD
        Task<List<TaxSettingsDto>> GetAllTaxSettingsAsync();
        Task<TaxSettingsDto?> UpdateTaxSettingsAsync(int id, TaxSettingsDto dto);
        Task<bool> DeleteTaxSettingsAsync(int id);
    }
}