using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Common;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface ICertificationTypeService
    {
        Task<ApiResponse<IEnumerable<CertificationTypeDto>>> GetAllAsync(
            int companyId, int regionId);

        Task<ApiResponse<CertificationTypeDto?>> GetByIdAsync(int id);
        Task<ApiResponse<CertificationTypeDto>> CreateAsync(CreateUpdateCertificationTypeDto dto, int createdBy);
        Task<ApiResponse<CertificationTypeDto>> UpdateAsync(int id, CreateUpdateCertificationTypeDto dto, int modifiedBy);
        Task<ApiResponse<object>> DeleteAsync(int id);

        Task<ApiResponse<(int inserted, int duplicates, int failed)>> BulkInsertAsync(
            IEnumerable<CreateUpdateCertificationTypeDto> items, int createdBy);

    }
}
