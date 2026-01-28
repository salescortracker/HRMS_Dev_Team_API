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
    public class StateService : IStateService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

      

        #region Get Methods

        // Get all states excluding deleted
        public async Task<IEnumerable<StateMaster>> GetAllStatesAsync()
        {
            var list = await _unitOfWork.Repository<StateMaster>().GetAllAsync();
            return list.Where(s => !s.IsDeleted).ToList();
        }

        // Get active states only
        public async Task<IEnumerable<StateMaster>> GetActiveStatesAsync()
        {
            var list = await _unitOfWork.Repository<StateMaster>().GetAllAsync();
            return list.Where(s => s.IsActive && !s.IsDeleted).ToList();
        }

        // Get single state by ID
        public async Task<StateMaster?> GetStateByIdAsync(int stateId)
        {
            var list = await _unitOfWork.Repository<StateMaster>().GetAllAsync();
            return list.FirstOrDefault(s => s.StateId == stateId && !s.IsDeleted);
        }

        #endregion

        #region Save / Update / Delete

        // Create new state
        public async Task<bool> CreateStateAsync(StateDto dto)
        {
            var entity = new StateMaster
            {
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                StateName = dto.StateName,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Repository<StateMaster>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // Update existing state
        public async Task<bool> UpdateStateAsync(StateDto dto)
        {
            var list = await _unitOfWork.Repository<StateMaster>().GetAllAsync();
            var entity = list.FirstOrDefault(s => s.StateId == dto.StateId && !s.IsDeleted);

            if (entity == null) return false;

            entity.CompanyId = dto.CompanyId;
            entity.RegionId = dto.RegionId;
            entity.StateName = dto.StateName;
            entity.IsActive = dto.IsActive;
            entity.ModifiedBy = dto.CreatedBy;
            entity.ModifiedAt = DateTime.Now;

            _unitOfWork.Repository<StateMaster>().Update(entity);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // Soft delete state
        public async Task<bool> DeleteStateAsync(int stateId)
        {
            var list = await _unitOfWork.Repository<StateMaster>().GetAllAsync();
            var entity = list.FirstOrDefault(s => s.StateId == stateId && !s.IsDeleted);

            if (entity == null) return false;

            entity.IsDeleted = true;
            _unitOfWork.Repository<StateMaster>().Update(entity);
            await _unitOfWork.CompleteAsync();
            return true;
        }
            #endregion
    }
}   