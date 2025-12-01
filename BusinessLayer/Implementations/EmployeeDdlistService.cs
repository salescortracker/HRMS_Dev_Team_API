using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class EmployeeDdlistService : IEmployeeDdlistService
    {
        private readonly HRMSContext _context;

        public EmployeeDdlistService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeDdlistDto>> GetAllAsync()
        {
            var data = await _context.EmployeeDdlists.ToListAsync();

            return data.Select(e => new EmployeeDdlistDto
            {
                DdlistId = e.DdlistId,
                EmployeeId = e.EmployeeId,
                RegionId = e.RegionId,
                UserId = e.UserId,
                CompanyId = e.CompanyId,
                Ddnumber = e.Ddnumber,
                Dddate = e.Dddate,
                BankName = e.BankName,
                BranchName = e.BranchName,
                Amount = e.Amount,
                PayeeName = e.PayeeName,
                DdcopyFilePath = e.DdcopyFilePath
            });
        }

        public async Task<EmployeeDdlistDto?> GetByIdAsync(int id)
        {
            var e = await _context.EmployeeDdlists.FindAsync(id);
            if (e == null) return null;

            return new EmployeeDdlistDto
            {
                DdlistId = e.DdlistId,
                EmployeeId = e.EmployeeId,
                RegionId = e.RegionId,
                UserId = e.UserId,
                CompanyId = e.CompanyId,
                Ddnumber = e.Ddnumber,
                Dddate = e.Dddate,
                BankName = e.BankName,
                BranchName = e.BranchName,
                Amount = e.Amount,
                PayeeName = e.PayeeName,
                DdcopyFilePath = e.DdcopyFilePath
            };
        }

        public async Task<bool> AddAsync(EmployeeDdlistDto dto)
        {
            var entity = new EmployeeDdlist
            {
                EmployeeId = dto.EmployeeId,
                RegionId = dto.RegionId,
                UserId = dto.UserId,
                CompanyId = dto.CompanyId,
                Ddnumber = dto.Ddnumber,
                Dddate = dto.Dddate,
                BankName = dto.BankName,
                BranchName = dto.BranchName,
                Amount = dto.Amount,
                PayeeName = dto.PayeeName,
                DdcopyFilePath = dto.DdcopyFilePath,
                CreatedAt = DateTime.UtcNow
            };

            _context.EmployeeDdlists.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(EmployeeDdlistDto dto)
        {
            var entity = await _context.EmployeeDdlists.FindAsync(dto.DdlistId);
            if (entity == null) return false;

            entity.EmployeeId = dto.EmployeeId;
            entity.RegionId = dto.RegionId;
            entity.UserId = dto.UserId;
            entity.CompanyId = dto.CompanyId;
            entity.Ddnumber = dto.Ddnumber;
            entity.Dddate = dto.Dddate;
            entity.BankName = dto.BankName;
            entity.BranchName = dto.BranchName;
            entity.Amount = dto.Amount;
            entity.PayeeName = dto.PayeeName;
            entity.DdcopyFilePath = dto.DdcopyFilePath;
            entity.UpdatedAt = DateTime.UtcNow;

            _context.EmployeeDdlists.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.EmployeeDdlists.FindAsync(id);
            if (entity == null) return false;

            _context.EmployeeDdlists.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}