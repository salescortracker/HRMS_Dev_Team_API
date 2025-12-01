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
    public class EmployeeW4Service : IEmployeeW4Service
    {
        private readonly HRMSContext _context;

        public EmployeeW4Service(HRMSContext context)
        {
            _context = context;
        }

        public async Task<List<EmployeeW4Dto>> GetAllAsync()
        {
            return await _context.EmployeeW4s
                .Select(w => new EmployeeW4Dto
                {
                    W4Id = w.W4Id,
                    EmployeeId = w.EmployeeId,
                    FirstName = w.FirstName,
                    MiddleInitial = w.MiddleInitial,
                    LastName = w.LastName,
                    Ssn = w.Ssn,
                    Address = w.Address,
                    City = w.City,
                    State = w.State,
                    ZipCode = w.ZipCode,
                    FilingStatus = w.FilingStatus,
                    MultipleJobsOrSpouse = w.MultipleJobsOrSpouse,
                    TotalDependents = w.TotalDependents,
                    DependentAmounts = w.DependentAmounts,
                    OtherIncome = w.OtherIncome,
                    Deductions = w.Deductions,
                    ExtraWithholding = w.ExtraWithholding,
                    EmployeeSignature = w.EmployeeSignature,
                    FormDate = w.FormDate,
                    RegionId = w.RegionId,
                    UserId = w.UserId,
                    CompanyId = w.CompanyId
                })
                .ToListAsync();
        }

        public async Task<EmployeeW4Dto?> GetByIdAsync(int id)
        {
            var w4 = await _context.EmployeeW4s.FindAsync(id);
            if (w4 == null) return null;
            return new EmployeeW4Dto
            {
                W4Id = w4.W4Id,
                EmployeeId = w4.EmployeeId,
                FirstName = w4.FirstName,
                MiddleInitial = w4.MiddleInitial,
                LastName = w4.LastName,
                Ssn = w4.Ssn,
                Address = w4.Address,
                City = w4.City,
                State = w4.State,
                ZipCode = w4.ZipCode,
                FilingStatus = w4.FilingStatus,
                MultipleJobsOrSpouse = w4.MultipleJobsOrSpouse,
                TotalDependents = w4.TotalDependents,
                DependentAmounts = w4.DependentAmounts,
                OtherIncome = w4.OtherIncome,
                Deductions = w4.Deductions,
                ExtraWithholding = w4.ExtraWithholding,
                EmployeeSignature = w4.EmployeeSignature,
                FormDate = w4.FormDate,
                RegionId = w4.RegionId,
                UserId = w4.UserId,
                CompanyId = w4.CompanyId
            };
        }

        public async Task<bool> AddAsync(EmployeeW4Dto dto)
        {
            var entity = new EmployeeW4
            {
                EmployeeId = dto.EmployeeId,
                FirstName = dto.FirstName,
                MiddleInitial = dto.MiddleInitial,
                LastName = dto.LastName,
                Ssn = dto.Ssn,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                ZipCode = dto.ZipCode,
                FilingStatus = dto.FilingStatus,
                MultipleJobsOrSpouse = dto.MultipleJobsOrSpouse,
                TotalDependents = dto.TotalDependents,
                DependentAmounts = dto.DependentAmounts,
                OtherIncome = dto.OtherIncome,
                Deductions = dto.Deductions,
                ExtraWithholding = dto.ExtraWithholding,
                EmployeeSignature = dto.EmployeeSignature,
                FormDate = dto.FormDate,
                RegionId = dto.RegionId,
                UserId = dto.UserId,
                CompanyId = dto.CompanyId
            };
            _context.EmployeeW4s.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(EmployeeW4Dto dto)
        {
            var entity = await _context.EmployeeW4s.FindAsync(dto.W4Id);
            if (entity == null) return false;

            entity.EmployeeId = dto.EmployeeId;
            entity.FirstName = dto.FirstName;
            entity.MiddleInitial = dto.MiddleInitial;
            entity.LastName = dto.LastName;
            entity.Ssn = dto.Ssn;
            entity.Address = dto.Address;
            entity.City = dto.City;
            entity.State = dto.State;
            entity.ZipCode = dto.ZipCode;
            entity.FilingStatus = dto.FilingStatus;
            entity.MultipleJobsOrSpouse = dto.MultipleJobsOrSpouse;
            entity.TotalDependents = dto.TotalDependents;
            entity.DependentAmounts = dto.DependentAmounts;
            entity.OtherIncome = dto.OtherIncome;
            entity.Deductions = dto.Deductions;
            entity.ExtraWithholding = dto.ExtraWithholding;
            entity.EmployeeSignature = dto.EmployeeSignature;
            entity.FormDate = dto.FormDate;
            entity.RegionId = dto.RegionId;
            entity.UserId = dto.UserId;
            entity.CompanyId = dto.CompanyId;

            _context.EmployeeW4s.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.EmployeeW4s.FindAsync(id);
            if (entity == null) return false;
            _context.EmployeeW4s.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}