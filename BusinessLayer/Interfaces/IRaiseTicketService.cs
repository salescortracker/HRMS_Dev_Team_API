using BusinessLayer.DTOs;
using DataAccessLayer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public  interface IRaiseTicketService
    {
        Task<IEnumerable<RaiseTicket>> GetAllTicketsAsync();
        Task<RaiseTicket?> GetTicketByIdAsync(int id);
        Task<RaiseTicket> CreateTicketAsync(RaiseTicketCreateDto RaiseTicketDto);
        Task<RaiseTicket?> UpdateTicketAsync(int id, RaiseTicketUpdateDto updatedRaiseTicket);
        Task<bool> DeleteRaiseTicketAsync(int id);
        
    }
}
