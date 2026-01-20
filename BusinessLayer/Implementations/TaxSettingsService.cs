using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class TaxSettingsService : ITaxSettingsService
    {
        private readonly HRMSContext _context;

        public TaxSettingsService(HRMSContext context)
        {
            _context = context;
        }

        // ============================
        // Dropdown - Tax Types
        // ============================
        public async Task<List<TaxTypeDto>> GetTaxTypesAsync()
        {
            return await _context.TaxTypes
                .Where(x => x.IsActive)
                .Select(x => new TaxTypeDto
                {
                    TaxTypeId = x.TaxTypeId,
                    TaxTypeName = x.TaxTypeName
                })
                .ToListAsync();
        }

        // ============================
        // Save Tax Settings
        // ============================
        public async Task<bool> SaveTaxSettingsAsync(TaxSettingsDto dto)
        {
            var entity = new TaxSetting
            {
                TaxName = dto.TaxName,
                TaxTypeId = dto.TaxTypeId,
                Rate = dto.Rate,
                EffectiveDate = DateOnly.FromDateTime(dto.EffectiveDate),
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now
            };

            _context.TaxSettings.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        // ============================
        // Get All Tax Settings
        // ============================
        public async Task<List<TaxSettingsDto>> GetAllTaxSettingsAsync()
        {
            return await _context.TaxSettings
                .Include(x => x.TaxType)
                .Select(x => new TaxSettingsDto
                {
                    TaxId = x.TaxId,
                    TaxName = x.TaxName,
                    TaxTypeId = x.TaxTypeId,
                    Rate = x.Rate,
                    EffectiveDate = x.EffectiveDate.ToDateTime(TimeOnly.MinValue),
                    IsActive = x.IsActive
                })
                .ToListAsync();
        }

        // ============================
        // Update Tax Settings
        // ============================
        public async Task<TaxSettingsDto?> UpdateTaxSettingsAsync(int id, TaxSettingsDto dto)
        {
            var entity = await _context.TaxSettings.FindAsync(id);
            if (entity == null) return null;

            entity.TaxName = dto.TaxName;
            entity.TaxTypeId = dto.TaxTypeId;
            entity.Rate = dto.Rate;
            entity.EffectiveDate = DateOnly.FromDateTime(dto.EffectiveDate);
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new TaxSettingsDto
            {
                TaxId = entity.TaxId,
                TaxName = entity.TaxName,
                TaxTypeId = entity.TaxTypeId,
                Rate = entity.Rate,
                EffectiveDate = entity.EffectiveDate.ToDateTime(TimeOnly.MinValue),
                IsActive = entity.IsActive
            };
        }

        // ============================
        // Delete Tax Settings
        // ============================
        public async Task<bool> DeleteTaxSettingsAsync(int id)
        {
            var entity = await _context.TaxSettings.FindAsync(id);
            if (entity == null) return false;

            _context.TaxSettings.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}