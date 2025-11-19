using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;

namespace BusinessLayer.Implementations
{
    public class GenderService:IGenderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GenderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<GenderDto>> GetAllGendersAsync()
        {
            var genders = await _unitOfWork.Repository<Gender>().GetAllAsync();
            return genders.Select(g => MapToDto(g));
        }

        public async Task<GenderDto?> GetGenderByIdAsync(int id)
        {
            var gender = await _unitOfWork.Repository<Gender>().GetByIdAsync(id);
            return gender == null ? null : MapToDto(gender);
        }

        public async Task<IEnumerable<GenderDto>> SearchGenderAsync(object filter)
        {
            var props = filter.GetType().GetProperties();
            var allGenders = await _unitOfWork.Repository<Gender>().GetAllAsync();
            var query = allGenders.AsQueryable();

            foreach (var prop in props)
            {
                var name = prop.Name;
                var value = prop.GetValue(filter);

                if (value != null)
                {
                    switch (name)
                    {
                        case nameof(Gender.GenderName):
                            query = query.Where(g => g.GenderName != null &&
                                                     g.GenderName.Contains(value.ToString()!));
                            break;

                        case nameof(Gender.IsActive):
                            bool active = Convert.ToBoolean(value);
                            query = query.Where(g => g.IsActive == active);
                            break;

                        case nameof(Gender.CompanyId):
                            int company = Convert.ToInt32(value);
                            query = query.Where(g => g.CompanyId == company);
                            break;

                        case nameof(Gender.RegionId):
                            int region = Convert.ToInt32(value);
                            query = query.Where(g => g.RegionId == region);
                            break;
                    }
                }
            }

            return query.Select(g => MapToDto(g)).ToList();
        }

        public async Task<GenderDto> AddGenderAsync(GenderDto dto)
        {
            var entity = new Gender
            {
                GenderName = dto.GenderName,
                IsActive = dto.IsActive,
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId                
            };

            await _unitOfWork.Repository<Gender>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return MapToDto(entity);
        }

        public async Task<GenderDto> UpdateGenderAsync(int id, GenderDto dto)
        {
            var entity = await _unitOfWork.Repository<Gender>().GetByIdAsync(id);
            if (entity == null) throw new Exception("Gender not found");

            entity.GenderName = dto.GenderName;
            entity.IsActive = dto.IsActive;
            entity.CompanyId = dto.CompanyId;
            entity.RegionId = dto.RegionId;
            entity.ModifiedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Gender>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return MapToDto(entity);
        }

        public async Task<bool> DeleteGenderAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Gender>().GetByIdAsync(id);
            if (entity == null)
                return false;

            _unitOfWork.Repository<Gender>().Remove(entity);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<Gender>> AddGendersAsync(List<GenderDto> dtos)
        {
            var entities = dtos.Select(dto => new Gender
            {
                GenderName = dto.GenderName,
                IsActive = dto.IsActive,
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId               
            }).ToList();

            await _unitOfWork.Repository<Gender>().AddRangeAsync(entities);
            await _unitOfWork.CompleteAsync();

            return entities;
        }

        private GenderDto MapToDto(Gender g)
        {
            return new GenderDto
            {
                GenderId = g.GenderId,
                GenderName = g.GenderName,
                IsActive = g.IsActive,
                CompanyId = g.CompanyId,
                RegionId = g.RegionId
            };
        }
    }
}
