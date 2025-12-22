using BusinessLayer.DTOs;
using DataAccessLayer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BusinessLayer.Interfaces;


namespace BusinessLayer.Implementations
{
    public class EmployeeProfileService : IEmployeeProfileService
    {
        private readonly HRMSContext _context;

        public EmployeeProfileService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<EmployeeProfileDto> GetEmployeeProfileAsync(int userId)
        {
            var data = await (
                from u in _context.Users

                join ep in _context.EmployeePersonalDetails
                    on u.UserId equals ep.UserId into epJoin
                from ep in epJoin.DefaultIfEmpty()

                join rm in _context.Users
                    on u.ReportingTo equals rm.UserId into rmJoin
                from rm in rmJoin.DefaultIfEmpty()
                join r in _context.RoleMasters
                    on u.RoleId equals r.RoleId into rJoin
                from r in rJoin.DefaultIfEmpty()
                join sm in _context.ShiftMasters
                    on new { u.CompanyId, u.RegionId }
                    equals new { sm.CompanyId, sm.RegionId } into smJoin
                from sm in smJoin.DefaultIfEmpty()

                    // ⭐ NEW JOIN – Region Table
                join reg in _context.Regions
                    on new { u.CompanyId, u.RegionId }
                    equals new { reg.CompanyId, reg.RegionId } into regJoin
                from reg in regJoin.DefaultIfEmpty()


                where u.UserId == userId

                select new EmployeeProfileDto
                {
                    EmployeeCode = u.EmployeeCode,
                    FullName = u.FullName,
                    Email = u.Email,
                    
                    Phone = ep.MobileNumber,

                    BandGrade = ep.BandGrade,
                    EsicNumber = ep.EsicNumber,
                    PFNumber = ep.Pfnumber,
                    UAN = ep.Uan,

                    ReportingManager = rm.FullName,
                    DateOfJoining = ep.DateOfJoining,
                   
                    Rolename =r.RoleName,
                    EmployeeType = ep.EmployeeType,

                    ServiceStatus = u.Status,
                    Location = reg.RegionName,

                    ShiftName = sm.ShiftName,
                    SkypeId = ep.LinkedInProfile,
                    ProfilePicture = _context.EmployeeImages
                        .Where(i => i.UserId == userId)
                        .OrderByDescending(i => i.CreatedAt)
                        .Select(i => i.FilePath)
                        .FirstOrDefault()
                        }
            ).FirstOrDefaultAsync();

            return data;
        }
        public async Task<int> SaveEmployeeImageAsync(EmployeeImageRequestDto dto)
        {
            // Optional: remove old image for same user
            var existing = _context.EmployeeImages
                .FirstOrDefault(x => x.UserId == dto.UserId);

            if (existing != null)
            {
                existing.FileName = dto.FileName!;
                existing.FilePath = dto.FilePath!;
                existing.ModifiedBy = dto.CreatedBy;
                existing.ModifiedAt = DateTime.Now;
            }
            else
            {
                var entity = new EmployeeImage
                {
                    RegionId = dto.RegionId,
                    CompanyId = dto.CompanyId,
                    UserId = dto.UserId,
                    FileName = dto.FileName!,
                    FilePath = dto.FilePath,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.Now
                };

                _context.EmployeeImages.Add(entity);
                await _context.SaveChangesAsync();
                return entity.Id;
            }

            await _context.SaveChangesAsync();
            return existing!.Id;
        }


    }

}



