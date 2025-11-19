using BusinessLayer.DTOs;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;

namespace BusinessLayer.Implementations
{
    public class BloodGroupService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BloodGroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BloodGroupDto>> GetAllAsync()
        {
            var data = await _unitOfWork.Repository<BloodGroup>().GetAllAsync();
            return data.Select(MapToDto).ToList();
        }

        public async Task<BloodGroupDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<BloodGroup>().GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<IEnumerable<BloodGroupDto>> SearchAsync(object filter)
        {
            var props = filter.GetType().GetProperties();
            var all = (await _unitOfWork.Repository<BloodGroup>().GetAllAsync()).AsQueryable();

            foreach (var prop in props)
            {
                var name = prop.Name;
                var value = prop.GetValue(filter);

                if (value != null)
                {
                    switch (name)
                    {
                        case nameof(BloodGroup.BloodGroupName):
                            all = all.Where(x => x.BloodGroupName.Contains(value.ToString()!));
                            break;
                        case nameof(BloodGroup.CompanyId):
                            all = all.Where(x => x.CompanyId == (int)value);
                            break;
                        case nameof(BloodGroup.RegionId):
                            all = all.Where(x => x.RegionId == (int)value);
                            break;
                        case nameof(BloodGroup.IsActive):
                            all = all.Where(x => x.IsActive == (bool)value);
                            break;
                    }
                }
            }

            return all.Select(MapToDto).ToList();
        }

        public async Task<BloodGroupDto> AddAsync(BloodGroupDto dto)
        {
            var entity = new BloodGroup
            {
                BloodGroupName = dto.BloodGroupName,
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                IsActive = dto.IsActive,
               
            };

            await _unitOfWork.Repository<BloodGroup>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return MapToDto(entity);
        }

        public async Task<BloodGroupDto> UpdateAsync(BloodGroupDto dto)
        {
            var entity = await _unitOfWork.Repository<BloodGroup>().GetByIdAsync(dto.BloodGroupId);
            if (entity == null) throw new Exception("Blood group not found");

            entity.BloodGroupName = dto.BloodGroupName;
            entity.CompanyId = dto.CompanyId;
            entity.RegionId = dto.RegionId;
            entity.IsActive = dto.IsActive;
           
            _unitOfWork.Repository<BloodGroup>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return MapToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<BloodGroup>().GetByIdAsync(id);
            if (entity == null) return false;

            _unitOfWork.Repository<BloodGroup>().Remove(entity);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        private BloodGroupDto MapToDto(BloodGroup bg)
        {
            return new BloodGroupDto
            {
                BloodGroupId = bg.BloodGroupId,
                BloodGroupName = bg.BloodGroupName,
                CompanyId = bg.CompanyId,
                RegionId = bg.RegionId,
                IsActive = bg.IsActive
            };
        }
    }
}
