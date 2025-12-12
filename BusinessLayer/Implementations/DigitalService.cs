using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class DigitalService : IDigitalService
    {
        private readonly HRMSContext _context;
        public DigitalService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<DigitalCardDto> GetDigitalCardAsync(int userId)
        {
            var data = await (
                from u in _context.Users
                join ep in _context.EmployeePersonalDetails on u.UserId equals ep.UserId into epjoin
                from ep in epjoin.DefaultIfEmpty()
                join c in _context.Companies on u.CompanyId equals c.CompanyId into cJoin
                from c in cJoin.DefaultIfEmpty()
                join rg in _context.Regions on u.RegionId equals rg.RegionId into rgJoin
                from rg in rgJoin.DefaultIfEmpty()
                join rm in _context.RoleMasters on u.RoleId equals rm.RoleId into rmJoin
                from rm in rmJoin.DefaultIfEmpty()
                where u.UserId == userId
                select new DigitalCardDto
                {
                    UserID = userId,
                    FullName = u.FullName,
                    Email = u.Email,
                    EmployeeCode = u.EmployeeCode,
                    RoleName = rm.RoleName,
                    CompanyName = c.CompanyName,
                    RegionName =rg.RegionName,
                    MobileNumber =ep.MobileNumber,
                    Location=rg.Country,
                    PersonalEmail=ep.PersonalEmail,
                    LinkedInProfile=ep.LinkedInProfile,
                    ProfilePictureBase64 =ep.ProfilePictureBase64

                }

                ).FirstOrDefaultAsync();

            return data;
        }
    }
}
