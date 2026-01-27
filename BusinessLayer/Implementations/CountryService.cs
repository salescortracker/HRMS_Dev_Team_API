using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class CountryService : ICountryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CountryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Get Methods

        // Get all countries excluding deleted ones
        public async Task<IEnumerable<CountryMaster>> GetAllCountriesAsync()
        {
            var list = await _unitOfWork.Repository<CountryMaster>().GetAllAsync();
            return list.Where(c => !c.IsDeleted).ToList();
        }

        // Get single country by ID (excluding deleted)
        public async Task<CountryMaster?> GetCountryByIdAsync(int id)
        {
            var list = await _unitOfWork.Repository<CountryMaster>().GetAllAsync();
            return list.FirstOrDefault(c => c.CountryId == id && !c.IsDeleted);
        }

        #endregion

        #region Save / Update / Delete

        // Save new country
        public async Task<bool> SaveCountryAsync(CountryDto dto)
        {
            var entity = new CountryMaster
            {
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                CountryName = dto.CountryName,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Repository<CountryMaster>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // Update existing country
        public async Task<bool> UpdateCountryAsync(CountryDto dto)
        {
            var list = await _unitOfWork.Repository<CountryMaster>().GetAllAsync();
            var entity = list.FirstOrDefault(c => c.CountryId == dto.CountryId && !c.IsDeleted);

            if (entity == null) return false;

            entity.CompanyId = dto.CompanyId;
            entity.RegionId = dto.RegionId;
            entity.CountryName = dto.CountryName;
            entity.IsActive = dto.IsActive;
            entity.ModifiedBy = dto.CreatedBy;
            entity.ModifiedAt = DateTime.Now;

            _unitOfWork.Repository<CountryMaster>().Update(entity);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // Soft delete
        public async Task<bool> DeleteCountryAsync(int id)
        {
            var list = await _unitOfWork.Repository<CountryMaster>().GetAllAsync();
            var entity = list.FirstOrDefault(c => c.CountryId == id && !c.IsDeleted);

            if (entity == null) return false;

            entity.IsDeleted = true;
            _unitOfWork.Repository<CountryMaster>().Update(entity);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        #endregion
    }
}