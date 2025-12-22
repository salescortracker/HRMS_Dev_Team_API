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

                    // Employee personal details
                join ep in _context.EmployeePersonalDetails
                    on u.UserId equals ep.UserId into epjoin
                from ep in epjoin.DefaultIfEmpty()

                    // Company
                join c in _context.Companies
                    on u.CompanyId equals c.CompanyId into cJoin
                from c in cJoin.DefaultIfEmpty()

                    // Region
                join rg in _context.Regions
                    on u.RegionId equals rg.RegionId into rgJoin
                from rg in rgJoin.DefaultIfEmpty()

                    // Role
                join rm in _context.RoleMasters
                    on u.RoleId equals rm.RoleId into rmJoin
                from rm in rmJoin.DefaultIfEmpty()

                    // 🔥 PROFILE IMAGE JOIN (MOST IMPORTANT)
                join ei in _context.EmployeeImages
                    on u.UserId equals ei.UserId into eiJoin
                from ei in eiJoin
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(1)
                    .DefaultIfEmpty()

                where u.UserId == userId

                select new DigitalCardDto
                {
                    UserID = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    EmployeeCode = u.EmployeeCode,
                    RoleName = rm.RoleName,
                    CompanyName = c.CompanyName,
                    RegionName = rg.RegionName,
                    MobileNumber = ep.MobileNumber,
                    Location = rg.Country,
                    PersonalEmail = ep.PersonalEmail,
                    LinkedInProfile = ep.LinkedInProfile,

                    // ✅ THIS IS WHAT UI NEEDS
                    ProfileImagePath = ei != null ? ei.FilePath : null
                }
            ).FirstOrDefaultAsync();

            return data;
        }


        public async Task<EmployeeImageRequestDto> employeeimage(int userId)
        {
            var image = await _context.EmployeeImages
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.CreatedAt) // optional: get latest
                .Select(e => new EmployeeImageRequestDto
                {
                    FileName = e.FileName,
                    FilePath = e.FilePath
                })
                .FirstOrDefaultAsync();

            return image;
        }



    }
}
