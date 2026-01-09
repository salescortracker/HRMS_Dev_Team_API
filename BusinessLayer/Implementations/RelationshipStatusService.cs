using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using Microsoft.EntityFrameworkCore;
using System;

public class RelationshipService : IRelationshipService
{
    private readonly IUnitOfWork _unitOfWork;

    public RelationshipService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<IEnumerable<RelationshipDto>>> GetAllAsync()
    {
        try
        {
            var list = await _unitOfWork.Repository<Relationship>()
                .FindAsync(r => !r.IsDeleted);

            var dto = list.Select(r => new RelationshipDto
            {
                relationshipId = r.RelationshipId,
                companyId = r.CompanyId,
                regionId = r.RegionId,
                relationshipName = r.RelationshipName,
                isActive = r.IsActive
            });

            return new ApiResponse<IEnumerable<RelationshipDto>>(dto, "Relationships retrieved successfully.");
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<RelationshipDto>>(null!, ex.Message, false);
        }
    }

    public async Task<ApiResponse<RelationshipDto?>> GetByIdAsync(int id)
    {
        var r = await _unitOfWork.Repository<Relationship>().GetByIdAsync(id);
        if (r == null || r.IsDeleted)
            return new ApiResponse<RelationshipDto?>(null, "Not found", false);

        return new ApiResponse<RelationshipDto?>(new RelationshipDto
        {
            relationshipId = r.RelationshipId,
            companyId = r.CompanyId,
            regionId = r.RegionId,
            relationshipName = r.RelationshipName,
            isActive = r.IsActive
        });
    }

    public async Task<ApiResponse<RelationshipDto>> CreateAsync(RelationshipDto dto, string createdBy)
    {
        var exists = (await _unitOfWork.Repository<Relationship>().FindAsync(r =>
            !r.IsDeleted &&
            r.CompanyId == dto.companyId &&
            r.RegionId == dto.regionId &&
            r.RelationshipName.ToLower() == dto.relationshipName.ToLower()
        )).Any();

        if (exists)
            return new ApiResponse<RelationshipDto>(null!, "Duplicate exists", false);

        var entity = new Relationship
        {
            CompanyId = dto.companyId,
            RegionId = dto.regionId,
            RelationshipName = dto.relationshipName,
            IsActive = dto.isActive,
            CreatedBy = int.Parse(createdBy),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _unitOfWork.Repository<Relationship>().AddAsync(entity);
        await _unitOfWork.CompleteAsync();

        dto.relationshipId = entity.RelationshipId;
        return new ApiResponse<RelationshipDto>(dto, "Created successfully");
    }

    public async Task<ApiResponse<RelationshipDto>> UpdateAsync(int id, RelationshipDto dto, string modifiedBy)
    {
        var entity = await _unitOfWork.Repository<Relationship>().GetByIdAsync(id);
        if (entity == null || entity.IsDeleted)
            return new ApiResponse<RelationshipDto>(null!, "Not found", false);

        entity.CompanyId = dto.companyId;
        entity.RegionId = dto.regionId;
        entity.RelationshipName = dto.relationshipName;
        entity.IsActive = dto.isActive;
        entity.ModifiedBy = int.Parse(modifiedBy);
        entity.ModifiedAt = DateTime.UtcNow;

        _unitOfWork.Repository<Relationship>().Update(entity);
        await _unitOfWork.CompleteAsync();

        return new ApiResponse<RelationshipDto>(dto, "Updated successfully");
    }

    public async Task<ApiResponse<object>> SoftDeleteAsync(int id, string modifiedBy)
    {
        var entity = await _unitOfWork.Repository<Relationship>().GetByIdAsync(id);
        if (entity == null || entity.IsDeleted)
            return new ApiResponse<object>(null!, "Not found", false);

        entity.IsDeleted = true;
        entity.ModifiedBy = int.Parse(modifiedBy);
        entity.ModifiedAt = DateTime.UtcNow;

        _unitOfWork.Repository<Relationship>().Update(entity);
        await _unitOfWork.CompleteAsync();

        return new ApiResponse<object>(null!, "Deleted successfully");
    }
}
