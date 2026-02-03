using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Implementations
{
    public class ResignationTypeService : IResignationTypeService
    {
        private readonly HRMSContext _context;

        public ResignationTypeService(HRMSContext context)
        {
            _context = context;
        }

        public List<ResignationTypeDto> GetResignationTypes()
        {
            return _context.ResignationTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.ResignationTypeName)
                .Select(x => new ResignationTypeDto
                {
                    ResignationTypeId = x.ResignationTypeId,
                    ResignationTypeName = x.ResignationTypeName,
                    NoticePeriod = x.NoticePeriod
                })
                .ToList();
        }

        public void SubmitResignation(EmployeeResignationDto dto)
        {
            var entity = new EmployeeResignation
            {
                EmployeeId = dto.EmployeeId,
                ResignationType = dto.ResignationType,
                NoticePeriod = dto.NoticePeriod,

                LastWorkingDay = dto.LastWorkingDay.HasValue
                    ? DateOnly.FromDateTime(dto.LastWorkingDay.Value)
                    : null,

                ResignationReason = dto.ResignationReason,
                Status = "Submitted",
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                UserId = dto.UserId,
                RoleId = dto.RoleId,
                CreatedAt = DateTime.Now
            };

            _context.EmployeeResignations.Add(entity);
            _context.SaveChanges();
        }

        public List<EmployeeResignationDto> GetEmployeeResignations(string employeeId)
        {
            return _context.EmployeeResignations
                .Where(x => x.UserId == int.Parse(employeeId))
                .Select(x => new EmployeeResignationDto
                {
                    EmployeeId = x.EmployeeId,
                    ResignationType = x.ResignationType,
                    NoticePeriod = x.NoticePeriod,
                    LastWorkingDay = x.LastWorkingDay.HasValue
                        ? x.LastWorkingDay.Value.ToDateTime(TimeOnly.MinValue)
                        : null,
                    ResignationReason = x.ResignationReason,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    UserId = x.UserId,
                    RoleId = x.RoleId
                })
                .ToList();
        }


    }
}
