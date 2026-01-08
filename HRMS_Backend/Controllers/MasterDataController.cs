using BusinessLayer.DTOs;
using BusinessLayer.Implementations;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using BusinessLayer.DTOs; // for GenderDto
using DataAccessLayer.DBContext; // for Gender entity

//using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using System.Runtime.CompilerServices;

namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDataController : ControllerBase
    {
        private readonly IDepartmentService _service;
       
        private readonly ILogger<MasterDataController> _logger;
        private readonly IDesignationService _designationService;
        //private readonly IBloodGroupService _bloodGroupservice;
        private readonly IPolicyCategoryService _policyCategoryService;
        private readonly IExpenseCategoryService _expenseCategoryService;
        private readonly IEventService _eventService;
        public MasterDataController(IDepartmentService service, IDesignationService designationService, IPolicyCategoryService policyCategoryService, 
            IExpenseCategoryService expenseCategoryService , IEventService eventService,ILogger<MasterDataController> logger)
        {
            _service = service;
            _designationService = designationService;
           
            //_bloodGroupservice = bloodGroupservice;
            _logger = logger;
            _policyCategoryService = policyCategoryService;
            _expenseCategoryService = expenseCategoryService;
            _eventService = eventService;

        }
        #region Departments
        // ✅ GET ALL (with optional filters later)
        [HttpGet("GetDepartments")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();

                if (result == null)
                    return NotFound(new { success = false, message = "No departments found." });

                return Ok(new { success = true, message = "Departments retrieved successfully.", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching department list.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred while fetching department list." });
            }
        }

        // ✅ GET BY ID
        [HttpGet("GetDepartmentsById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { success = false, message = $"Department with ID {id} not found." });

                return Ok(new { success = true, message = "Department details retrieved successfully.", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving department with ID {id}.");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching department details." });
            }
        }

        // ✅ CREATE
        [HttpPost("createDepartment")]
        public async Task<IActionResult> Create([FromBody] CreateUpdateDepartmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input data. Please check your fields." });

            try
            {
                var createdBy = "system"; // 🔒 TODO: Replace with JWT user later
                var result = await _service.CreateAsync(dto, createdBy);

                if (!result.Success)
                    return BadRequest(result);

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new department.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred while creating the department." });
            }
        }

        // ✅ UPDATE
        [HttpPost("updateDepartment/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateDepartmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input data." });

            try
            {
                var modifiedBy = "system"; // 🔒 TODO: Replace with JWT user later
                var result = await _service.UpdateAsync(id, dto, modifiedBy);

                if (!result.Success)
                    return NotFound(result);

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating department with ID {id}.");
                return StatusCode(500, new { success = false, message = "An error occurred while updating the department." });
            }
        }

        // ✅ SOFT DELETE
        [HttpDelete("deleteDepartment/{id:int}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var modifiedBy = "system"; // 🔒 TODO: Replace with JWT user later
                var result = await _service.SoftDeleteAsync(id, modifiedBy);

                if (!result.Success)
                    return NotFound(result);

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting department with ID {id}.");
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the department." });
            }
        }

        // ✅ BULK INSERT
        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsert([FromBody] IEnumerable<CreateUpdateDepartmentDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest(new { success = false, message = "No records found to upload." });

            try
            {
                var createdBy = "system"; // 🔒 TODO: Replace with JWT user later
                var result = await _service.BulkInsertAsync(dtos, createdBy);

                return Ok(new { success = true, message = result.Message, insertedCount = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk department upload.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred during bulk upload." });
            }
        }
        #endregion
        #region Designations
        // ✅ GET ALL
        [HttpGet("GetDesignations")]
        public async Task<IActionResult> GetDesignations()
        {
            try
            {
                var result = await _designationService.GetAllAsync();

                if (result == null)
                    return NotFound(new { success = false, message = "No designations found." });

                return Ok(new { success = true, message = "Designations retrieved successfully.", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching designation list.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred while fetching designations." });
            }
        }

        // ✅ GET BY ID
        [HttpGet("GetDesignationById/{id:int}")]
        public async Task<IActionResult> GetDesignationById(int id)
        {
            try
            {
                var result = await _designationService.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { success = false, message = $"Designation with ID {id} not found." });

                return Ok(new { success = true, message = "Designation details retrieved successfully.", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving designation with ID {id}.");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching designation details." });
            }
        }

        // ✅ CREATE
        [HttpPost("CreateDesignation")]
        public async Task<IActionResult> Create([FromBody] CreateUpdateDesignationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input data. Please check your fields." });

            try
            {
                var createdBy = 1; // 🔒 TODO: Replace with logged-in user later
                var result = await _designationService.CreateAsync(dto, createdBy);

                if (!result.Success)
                    return BadRequest(result);

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new designation.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred while creating the designation." });
            }
        }

        // ✅ UPDATE
        [HttpPost("UpdateDesignation/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateDesignationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input data." });

            try
            {
                var modifiedBy = 1; // 🔒 TODO: Replace with logged-in user later
                var result = await _designationService.UpdateAsync(id, dto, modifiedBy);

                if (!result.Success)
                    return NotFound(result);

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating designation with ID {id}.");
                return StatusCode(500, new { success = false, message = "An error occurred while updating the designation." });
            }
        }

        // ✅ SOFT DELETE
        [HttpPost("DeleteDesignation/{id:int}")]
        public async Task<IActionResult> DeleteDesignation(int id)
        {
            try
            {
                var modifiedBy = 1; // 🔒 TODO: Replace with JWT user later
                var result = await _designationService.SoftDeleteAsync(id, modifiedBy);

                if (!result.Success)
                    return NotFound(result);

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting designation with ID {id}.");
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the designation." });
            }
        }
        // ✅ BULK INSERT
        [HttpPost("DesignationBulkInsert")]
        public async Task<IActionResult> BulkInsert([FromBody] IEnumerable<CreateUpdateDesignationDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest(new { success = false, message = "No records found to upload." });

            try
            {
                var createdBy = 1; // TODO: Replace with JWT username later
                var result = await _designationService.BulkInsertAsync(dtos, createdBy);

                return Ok(new
                {
                    success = result.Success,
                    message = result.Message,
                    data = new
                    {
                        inserted = result.Data.inserted,
                        duplicates = result.Data.duplicates,
                        failed = result.Data.failed
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk designation upload.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred during bulk upload." });
            }
        }

        #endregion



     

        #region PolicyCategory



        [HttpGet("PolicyCategories")]
        public async Task<IActionResult> GetPolicy(int companyId, int regionId)
        {
            var data = await _policyCategoryService.GetPolicyAsync(companyId, regionId);
            return Ok(data);
        }

        [HttpPost("PolicyCategories")]
        public async Task<IActionResult> CreatePolicyCategory([FromBody] PolicyCategoryDto dto)
        {
            if (dto == null)
                return BadRequest("Payload is null");

            if (dto.CompanyId <= 0 || dto.RegionId <= 0)
                return BadRequest("Invalid CompanyId or RegionId");

            if (string.IsNullOrWhiteSpace(dto.PolicyCategoryName))
                return BadRequest("PolicyCategoryName is required");

            var result = await _policyCategoryService.CreatePolicyAsync(dto);
            return Ok(result);
        }



        [HttpPut("PolicyCategories/{id}")]
        public async Task<IActionResult> UpdatePolicy(int id, [FromBody] PolicyCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _policyCategoryService.UpdatePolicyAsync(id, dto);
            if (!success) return NotFound();

            return Ok();
        }

        [HttpDelete("PolicyCategories/{id}")]
        public async Task<IActionResult> DeletePolicy(int id)
        {
            int userId = 1;
            var success = await _policyCategoryService.DeletePolicyAsync(id, userId);
            if (!success) return NotFound();

            return Ok();
        }

        [HttpPost("PolicyCategories/BulkUpload")]
        public async Task<IActionResult> BulkUploadPolicyCategories(
             IFormFile file,
             int companyId,
             int regionId,
             int userId)
        {
            if (file == null)
                return BadRequest("File missing");

            var result = await _policyCategoryService
                .BulkUploadAsync(file, companyId, regionId, userId);

            return Ok(result);
        }



        #endregion

        #region ExpencesCategoryType


        [HttpGet("GetAllExpences/{companyId}/{regionId}")]
        public async Task<IActionResult> GetAllExpences(int companyId, int regionId)
        {
            var data = await _expenseCategoryService.GetAllExpencesAsync(companyId, regionId);
            return Ok(new { data });
        }

        // POST: api/ExpenseCategoryType/Create
        [HttpPost("CreateExpences")]
        public async Task<IActionResult> CreateExpences([FromBody] ExpenseCategoryDto dto)
        {
            int userId = 1; // 🔐 replace with logged-in user
            await _expenseCategoryService.CreateExpencesAsync(dto, userId);
            return Ok(new { message = "Created successfully" });
        }

        // PUT: api/ExpenseCategoryType/Update
        [HttpPut("UpdateExpences")]
        public async Task<IActionResult> UpdateExpences([FromBody] ExpenseCategoryDto dto)
        {
            int userId = 1;
            await _expenseCategoryService.UpdateExpencesAsync(dto, userId);
            return Ok(new { message = "Updated successfully" });
        }

        // DELETE: api/ExpenseCategoryType/Delete/5
        [HttpDelete("DeleteExpences/{id}")]
        public async Task<IActionResult> DeleteExpences(int id)
        {
            await _expenseCategoryService.DeleteExpencesAsync(id);
            return Ok(new { message = "Deleted successfully" });
        }
        #endregion

        #region Event
        

        [HttpGet("GetAllEventTypes")]
        public async Task<IActionResult> GetAllEventTypes()
        {
            var data = await _eventService.GetAllEventTypesAsync();
            return Ok(data);
        }

        [HttpGet("GetEventTypes")]
        public async Task<IActionResult> GetEventTypes(
            [FromQuery] int companyId,
            [FromQuery] int regionId)
        {
            var data = await _eventService.GetEventTypesAsync(companyId, regionId);
            return Ok(data);
        }

        [HttpGet("GetEvents")]
        public async Task<IActionResult> GetEvents(
            [FromQuery] int companyId,
            [FromQuery] int regionId)
        {
            var events = await _eventService.GetEventsAsync(companyId, regionId);
            return Ok(events);
        }

        [HttpGet("GetEventById/{eventId}")]
        public async Task<IActionResult> GetEventById(int eventId)
        {
            var data = await _eventService.GetEventByIdAsync(eventId);
            if (data == null)
                return NotFound("Event not found");

            return Ok(data);
        }

        [HttpPost("CreateEvent")]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto dto)
        {
            int userId = 1; // TODO: get from JWT
            var result = await _eventService.CreateEventAsync(dto, userId);
            return Ok(result);
        }

        [HttpPut("UpdateEvent/{eventId}")]
        public async Task<IActionResult> UpdateEvent(int eventId, [FromBody] EventDto dto)
        {
            int userId = dto.CompanyId; // Get from DTO
            var result = await _eventService.UpdateEventAsync(eventId, dto, userId);

            if (!result)
                return NotFound(new { message = "Event not found" });

            return Ok(new { message = "Event updated successfully" });
        }

        [HttpDelete("DeleteEvent/{eventId}")]
        public async Task<IActionResult> DeleteEvent(int eventId, [FromQuery] int userId)
        {
            var result = await _eventService.DeleteEventAsync(eventId, userId);

            if (!result)
                return NotFound(new { message = "Event not found" });

            return Ok(new { message = "Event deleted successfully" });
        }

        #endregion


    }
}
