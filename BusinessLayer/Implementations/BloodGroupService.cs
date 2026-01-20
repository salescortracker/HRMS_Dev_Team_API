using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;

namespace BusinessLayer.Implementations
{
    public class BloodGroupService : IBloodGroupService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BloodGroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ================================
        // GET ALL
        // ================================
        public async Task<IEnumerable<BloodGroupDto>> GetAllAsync()
        {
            var entities = await _unitOfWork
                .Repository<BloodGroup>()
                .GetAllAsync();

            return entities.Select(bg => new BloodGroupDto
            {
                BloodGroupId = bg.BloodGroupId,
                BloodGroupName = bg.BloodGroupName,
                CompanyId = bg.CompanyId,
                RegionId = bg.RegionId,
                IsActive = bg.IsActive
            });
        }

        // ================================
        // GET BY ID
        // ================================
        public async Task<BloodGroupDto?> GetByIdAsync(int id)
        {
            var bg = await _unitOfWork
                .Repository<BloodGroup>()
                .GetByIdAsync(id);

            if (bg == null)
                return null;

            return new BloodGroupDto
            {
                BloodGroupId = bg.BloodGroupId,
                BloodGroupName = bg.BloodGroupName,
                CompanyId = bg.CompanyId,
                RegionId = bg.RegionId,
                IsActive = bg.IsActive
            };
        }

        // ================================
        // CREATE
        // ================================
        public async Task<string> CreateAsync(BloodGroupDto dto)
        {
            var entity = new BloodGroup
            {
                BloodGroupName = dto.BloodGroupName,
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Repository<BloodGroup>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return "Blood group created successfully";
        }

        // ================================
        // UPDATE
        // ================================
        public async Task<string> UpdateAsync(BloodGroupDto dto)
        {
            var entity = await _unitOfWork
                .Repository<BloodGroup>()
                .GetByIdAsync(dto.BloodGroupId);

            if (entity == null)
                return "Blood group not found";

            entity.BloodGroupName = dto.BloodGroupName;
            entity.CompanyId = dto.CompanyId;
            entity.RegionId = dto.RegionId;
            entity.IsActive = dto.IsActive;
            entity.ModifiedAt = DateTime.Now;

            _unitOfWork.Repository<BloodGroup>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return "Blood group updated successfully";
        }

        // ================================
        // DELETE
        // ================================
        public async Task<string> DeleteAsync(int id)
        {
            var entity = await _unitOfWork
                .Repository<BloodGroup>()
                .GetByIdAsync(id);

            if (entity == null)
                return "Blood group not found";

            _unitOfWork.Repository<BloodGroup>().Remove(entity);
            await _unitOfWork.CompleteAsync();

            return "Blood group deleted successfully";
        }

    }
}
