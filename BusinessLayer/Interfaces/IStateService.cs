using BusinessLayer.DTOs;
using DataAccessLayer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IStateService
    {
        Task<IEnumerable<StateMaster>> GetAllStatesAsync();
        Task<IEnumerable<StateMaster>> GetActiveStatesAsync();
        Task<StateMaster?> GetStateByIdAsync(int stateId);
        Task<bool> CreateStateAsync(StateDto stateDto);
        Task<bool> UpdateStateAsync(StateDto stateDto);
        Task<bool> DeleteStateAsync(int stateId);
    }
}