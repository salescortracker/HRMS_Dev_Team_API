using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;

namespace BusinessLayer.Implementations
{
    public class CategoryService : ICategoryServicecs
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
        {
            var data = await _unitOfWork.Repository<Category>().GetAllAsync();

            return data
                .Where(x =>
                    x.IsActive == true


                )
                .Select(x => new CategoryDto
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName
                })
                .ToList();
        }
        public async Task<IEnumerable<CompanyPolicyDto>> GetAllAsync(int companyId, int regionId)
        {
            var policies = await _unitOfWork.Repository<CompanyPolicy>().GetAllAsync();
            var categories = await _unitOfWork.Repository<Category>().GetAllAsync();

            var result =
                from p in policies
                join c in categories on p.CategoryId equals c.CategoryId
                where p.CompanyId == companyId && p.RegionId == regionId
                select new CompanyPolicyDto
                {
                    PolicyId = p.PolicyId,
                    CompanyId = p.CompanyId,
                    RegionId = p.RegionId,
                    Title = p.Title,
                    CategoryId = p.CategoryId,
                    CategoryName = c.CategoryName, // ✅ FIXED
                    EffectiveDate = p.EffectiveDate.ToDateTime(TimeOnly.MinValue),
                    Description = p.Description,
                    FileName = p.FileName,
                    FilePath = p.FilePath
                };

            return result.ToList();
        }



        public async Task<CompanyPolicyDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork
                .Repository<CompanyPolicy>()
                .GetByIdAsync(id);

            return entity == null ? null : MapToDto(entity);
        }

        public async Task<CompanyPolicyDto> AddAsync(CompanyPolicyDto dto)
        {
            var entity = new CompanyPolicy
            {
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                Title = dto.Title,
                CategoryId = dto.CategoryId,
                EffectiveDate = DateOnly.FromDateTime(dto.EffectiveDate),
                FileName = dto.FileName!,
                FilePath = dto.FilePath,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<CompanyPolicy>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return MapToDto(entity);
        }

        public async Task<CompanyPolicyDto> UpdateAsync(int id, CompanyPolicyDto dto)
        {
            var entity = await _unitOfWork.Repository<CompanyPolicy>().GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Policy not found");

            entity.Title = dto.Title;
            entity.CategoryId = dto.CategoryId;
            entity.EffectiveDate = DateOnly.FromDateTime(dto.EffectiveDate);
            entity.Description = dto.Description;
            entity.FileName = dto.FileName ?? entity.FileName;
            entity.FilePath = dto.FilePath ?? entity.FilePath;
            entity.ModifiedAt = DateTime.UtcNow;

            _unitOfWork.Repository<CompanyPolicy>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return MapToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<CompanyPolicy>().GetByIdAsync(id);
            if (entity == null) return false;

            _unitOfWork.Repository<CompanyPolicy>().Remove(entity);
            await _unitOfWork.CompleteAsync();
            return true;
        }
        public async Task<IEnumerable<CompanyPolicyDto>> GetAllPoliciesAsync()
        {
            var policies = await _unitOfWork.Repository<CompanyPolicy>().GetAllAsync();
            var categories = await _unitOfWork.Repository<Category>().GetAllAsync();

            var result =
                from p in policies
                join c in categories on p.CategoryId equals c.CategoryId
                orderby p.CreatedAt descending
                select new CompanyPolicyDto
                {
                    PolicyId = p.PolicyId,
                    CompanyId = p.CompanyId,
                    RegionId = p.RegionId,
                    Title = p.Title,
                    CategoryId = p.CategoryId,
                    CategoryName = c.CategoryName,
                    EffectiveDate = p.EffectiveDate.ToDateTime(TimeOnly.MinValue),
                    Description = p.Description,
                    FileName = p.FileName,
                    FilePath = p.FilePath
                };

            return result.ToList();
        }


        private CompanyPolicyDto MapToDto(CompanyPolicy p)
        {
            return new CompanyPolicyDto
            {
                PolicyId = p.PolicyId,
                CompanyId = p.CompanyId,
                RegionId = p.RegionId,
                Title = p.Title,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.CategoryName,
                EffectiveDate = p.EffectiveDate.ToDateTime(TimeOnly.MinValue),
                Description = p.Description,
                FileName = p.FileName,
                FilePath = p.FilePath
            };
        }

    }
}
