using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using static HRMS_Backend.Controllers.UserManagementController;

namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeFullInfoController : ControllerBase
    {
        private readonly IEmployeeResignationService _resignationService;

        public EmployeeFullInfoController(IEmployeeResignationService resignationService)
        {
            _resignationService = resignationService;
        }

        #region Employee Resignation Details

        #region GetResignations
        /// <summary>
        /// Retrieves a list of all employee resignations.
        /// </summary>
        /// <remarks>
        /// Fetches all resignation records from the database using the service layer.
        /// </remarks>
        /// <returns>
        /// Returns HTTP 200 with list of resignation data.
        /// </returns>
        [HttpGet("GetResignations")]
        public async Task<IActionResult> GetAll(int companyId, int regionId, int roleId)
        {
            var data = await _resignationService
                .GetResignationsByCompanyRegionAsync(companyId, regionId);

            return Ok(data);
        }
        #endregion

        #region GetResignationById
        /// <summary>
        /// Retrieves a resignation record by its unique identifier.
        /// </summary>
        /// <param name="id">Resignation ID to fetch.</param>
        /// <returns>
        /// HTTP 200 with resignation record, or 404 if not found.
        /// </returns>
        [HttpGet("GetResignationById")]
        public async Task<IActionResult> GetById(int id, int companyId, int regionId)
        {
            var data = await _resignationService
                .GetResignationByIdFilteredAsync(id, companyId, regionId);

            if (data == null) return NotFound("Record not found or does not belong to this company/region.");

            return Ok(data);
        }
        #endregion

        #region GetResignationSearch
        /// <summary>
        /// Searches resignation records based on dynamic filter.
        /// </summary>
        /// <remarks>
        /// The filter object contains fields used to match records.
        /// </remarks>
        [HttpPost("GetResignationSearch")]
        public async Task<IActionResult> Search([FromBody] object filter)
        {
            var result = await _resignationService.SearchResignationsAsync(filter);
            return Ok(result);
        }
        #endregion

        #region SaveResignation
        /// <summary>
        /// Creates a new resignation entry.
        /// </summary>
        /// <param name="dto">Resignation DTO with employee data.</param>
        /// <returns>Returns created resignation record.</returns>
        [HttpPost("SaveResignation")]
        public async Task<IActionResult> Create([FromBody] EmployeeResignationDto dto)
        {
            var created = await _resignationService.AddResignationAsync(dto);
            return Ok(created);
        }
        #endregion

        #region UpdateResignation
        /// <summary>
        /// Updates an existing resignation entry.
        /// </summary>
        /// <param name="id">Resignation ID to update.</param>
        /// <param name="dto">Updated resignation details.</param>
        /// <returns>Returns the updated resignation DTO.</returns>
        [HttpPost("UpdateResignation/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeResignationDto dto)
        {
            var updated = await _resignationService.UpdateResignationAsync(id, dto);
            return Ok(updated);
        }
        #endregion

        #region DeleteResignation
        /// <summary>
        /// Deletes a resignation entry.
        /// </summary>
        /// <param name="id">Resignation ID to delete.</param>
        /// <returns>HTTP 204 No Content if deleted, 404 if not found.</returns>
        [HttpDelete("DeleteResignation/{id}")]
        public async Task<IActionResult> Delete(int id, int companyId, int regionId, int roleId)
        {
            var deleted = await _resignationService.DeleteResignationAsync(id, companyId, regionId);

            if (!deleted) return NotFound("Record not found or does not belong to this company/region.");

            return NoContent();
        }

        #endregion

        #endregion
    }
}
