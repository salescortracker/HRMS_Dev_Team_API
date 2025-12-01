using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class EmployeeBankDetailsService : IEmployeeBankDetailsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeBankDetailsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<EmployeeBankDetailsDto>> GetAllAsync()
        {
            var data = await _unitOfWork.Repository<EmployeeBankDetail>().GetAllAsync();

            return data.Select(e => new EmployeeBankDetailsDto
            {
                BankDetailsId = e.BankDetailsId,
                EmployeeId = e.EmployeeId,
                RegionId = e.RegionId,
                UserId = e.UserId,
                CompanyId = e.CompanyId,
                BankName = e.BankName,
                BranchName = e.BranchName,
                AccountHolderName = e.AccountHolderName,
                AccountNumber = e.AccountNumber,
                AccountTypeId = Convert.ToInt32(e.AccountTypeId),
                Ifsccode = e.Ifsccode,
                Micrcode = e.Micrcode,
                Upiid = e.Upiid
            });
        }

        public async Task<EmployeeBankDetailsDto?> GetByIdAsync(int id)
        {
            var e = await _unitOfWork.Repository<EmployeeBankDetail>().GetByIdAsync(id);
            if (e is null) return null;

            return new EmployeeBankDetailsDto
            {
                BankDetailsId = e.BankDetailsId,
                EmployeeId = e.EmployeeId,
                RegionId = e.RegionId,
                UserId = e.UserId,
                CompanyId = e.CompanyId,
                BankName = e.BankName,
                BranchName = e.BranchName,
                AccountHolderName = e.AccountHolderName,
                AccountNumber = e.AccountNumber,
                AccountTypeId = Convert.ToInt32(e.AccountTypeId),
                Ifsccode = e.Ifsccode,
                Micrcode = e.Micrcode,
                Upiid = e.Upiid
            };
        }

        public async Task<bool> AddAsync(EmployeeBankDetailsDto dto)
        {
            var entity = new EmployeeBankDetail
            {
                EmployeeId = dto.EmployeeId,
                RegionId = dto.RegionId,
                UserId = dto.UserId,
                CompanyId = dto.CompanyId,
                BankName = dto.BankName,
                BranchName = dto.BranchName,
                AccountHolderName = dto.AccountHolderName,
                AccountNumber = dto.AccountNumber,
                AccountTypeId = dto.AccountTypeId,
                Ifsccode = dto.Ifsccode,
                Micrcode = dto.Micrcode,
                Upiid = dto.Upiid,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<EmployeeBankDetail>().AddAsync(entity);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateAsync(EmployeeBankDetailsDto dto)
        {
            var repo = _unitOfWork.Repository<EmployeeBankDetail>();
            var entity = await repo.GetByIdAsync(dto.BankDetailsId);

            if (entity is null) return false;

            entity.RegionId = dto.RegionId;
            entity.UserId = dto.UserId;
            entity.CompanyId = dto.CompanyId;
            entity.BankName = dto.BankName;
            entity.BranchName = dto.BranchName;
            entity.AccountHolderName = dto.AccountHolderName;
            entity.AccountNumber = dto.AccountNumber;
            entity.AccountTypeId = dto.AccountTypeId; entity.Ifsccode = dto.Ifsccode;
            entity.Micrcode = dto.Micrcode;
            entity.Upiid = dto.Upiid;
            entity.ModifiedAt = DateTime.UtcNow;

            repo.Update(entity);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var repo = _unitOfWork.Repository<EmployeeBankDetail>();
            var entity = await repo.GetByIdAsync(id);

            if (entity is null) return false;

            repo.Remove(entity);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}




