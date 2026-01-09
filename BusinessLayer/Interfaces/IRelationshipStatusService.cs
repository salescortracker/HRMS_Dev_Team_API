using BusinessLayer.Common;
using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IRelationshipService
    {
        Task<ApiResponse<IEnumerable<RelationshipDto>>> GetAllAsync();
        Task<ApiResponse<RelationshipDto?>> GetByIdAsync(int id);
        Task<ApiResponse<RelationshipDto>> CreateAsync(RelationshipDto dto, string createdBy);
        Task<ApiResponse<RelationshipDto>> UpdateAsync(int id, RelationshipDto dto, string modifiedBy);
        Task<ApiResponse<object>> SoftDeleteAsync(int id, string modifiedBy);
    }
}