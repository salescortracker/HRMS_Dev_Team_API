using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class CityService : ICityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Get / Save / Update / Delete Methods

        // Get all cities excluding deleted
        public async Task<IEnumerable<CityMaster>> GetAllCitiesAsync()
        {
            var list = await _unitOfWork.Repository<CityMaster>().GetAllAsync();
            return list.Where(c => !c.IsDeleted).ToList();
        }

        // Get active cities only
        public async Task<IEnumerable<CityMaster>> GetActiveCitiesAsync()
        {
            var list = await _unitOfWork.Repository<CityMaster>().GetAllAsync();
            return list.Where(c => c.IsActive && !c.IsDeleted).ToList();
        }

        // Get single city by ID
        public async Task<CityMaster?> GetCityByIdAsync(int cityId)
        {
            var list = await _unitOfWork.Repository<CityMaster>().GetAllAsync();
            return list.FirstOrDefault(c => c.CityId == cityId && !c.IsDeleted);
        }

        // Create new city
        public async Task<bool> CreateCityAsync(CityDto dto)
        {
            var entity = new CityMaster
            {
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                StateId = dto.StateId,
                CityName = dto.CityName,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Repository<CityMaster>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // Update existing city
        public async Task<bool> UpdateCityAsync(CityDto dto)
        {
            var list = await _unitOfWork.Repository<CityMaster>().GetAllAsync();
            var entity = list.FirstOrDefault(c => c.CityId == dto.CityId && !c.IsDeleted);

            if (entity == null) return false;

            entity.CompanyId = dto.CompanyId;
            entity.RegionId = dto.RegionId;
            entity.StateId = dto.StateId;
            entity.CityName = dto.CityName;
            entity.IsActive = dto.IsActive;
            entity.ModifiedBy = dto.CreatedBy;
            entity.ModifiedAt = DateTime.Now;

            _unitOfWork.Repository<CityMaster>().Update(entity);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // Soft delete city
        public async Task<bool> DeleteCityAsync(int cityId)
        {
            var list = await _unitOfWork.Repository<CityMaster>().GetAllAsync();
            var entity = list.FirstOrDefault(c => c.CityId == cityId && !c.IsDeleted);

            if (entity == null) return false;

            entity.IsDeleted = true;
            _unitOfWork.Repository<CityMaster>().Update(entity);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        #endregion
    }
}