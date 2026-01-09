using BusinessLayer.DTOs;
using BusinessLayer.Implementations;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDataController : ControllerBase
    {
        private readonly IDepartmentService _service;
        private readonly IGenderService _genderService;
        private readonly ILogger<MasterDataController> _logger;
        private readonly IDesignationService _designationService;
        //private readonly IBloodGroupService _bloodGroupservice;
        private readonly IKpiCategoryService _kpiCategoryService;
        private readonly IExpenseStatusService _expenseStatusService;

        private readonly ILeaveStatusService _leaveStatusService;   // ✅ NEW
        private readonly IAttendanceStatusService _attendanceStatusService;



        public MasterDataController(IDepartmentService service, IDesignationService designationService, IGenderService genderService,  ILogger<MasterDataController> logger, IKpiCategoryService kpiCategoryService,IExpenseStatusService expenseStatusService,ILeaveStatusService leaveStatusService, IAttendanceStatusService attendanceStatusService)
        {
            _service = service;
            _designationService = designationService;
            _genderService = genderService;
           
            _logger = logger;
            _kpiCategoryService = kpiCategoryService;
            _expenseStatusService = expenseStatusService;
            _leaveStatusService = leaveStatusService;
            _attendanceStatusService = attendanceStatusService;

        }
        #region Departments
        // ✅ GET ALL (with optional filters later)
        [HttpGet("GetDepartments")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();

                if (result == null )
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

                if (result == null )
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
                var modifiedBy =1; // 🔒 TODO: Replace with logged-in user later
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
        #region Gender
        /// <summary>
        /// Gender Detail Retrieve
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetGenderAll")]
        public async Task<IActionResult> GetGenderAll()
        {
            return Ok(await _genderService.GetAllGendersAsync());
        }
        /// <summary>
        /// Retrieve Gender details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("GetGenderById/{id}")]
        public async Task<IActionResult> GetGenderById(int id)
        {
            var gender = await _genderService.GetGenderByIdAsync(id);
            if (gender == null) return NotFound("Gender not found");
            return Ok(gender);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>

        [HttpPost("GetGendersearch")]
        public async Task<IActionResult> Search([FromBody] object filter)
        {
            return Ok(await _genderService.SearchGenderAsync(filter));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("CreateGender")]
        public async Task<IActionResult> CreateGender([FromBody] GenderDto dto)
        {
            var result = await _genderService.AddGenderAsync(dto);
            return Ok(new { message = "Gender created successfully", data = result });
        }

        [HttpPost("UpdateGender/{id}")]
        public async Task<IActionResult> UpdateGender(int id, [FromBody] GenderDto dto)
        {
            var result = await _genderService.UpdateGenderAsync(id, dto);
            return Ok(new { message = "Gender updated successfully", data = result });
        }

        [HttpPost("DeleteGender/{id}")]
        public async Task<IActionResult> DeleteGender(int id)
        {
            bool success = await _genderService.DeleteGenderAsync(id);
            if (!success) return NotFound("Gender not found");

            return Ok(new { message = "Gender deleted successfully" });
        }
        #endregion
        #region kpicategory
        // =====================================================
        // KPI CATEGORY
        // =====================================================

        // GET ALL KPI CATEGORIES
        [HttpGet("kpi-categories")]
        public async Task<IActionResult> GetKpiCategories()
        {
            var result = await _kpiCategoryService.GetAll();
            return Ok(result);
        }


        // GET KPI CATEGORY BY ID
        [HttpGet("kpi-categories/{id:int}")]
        public async Task<IActionResult> GetKpiCategoryById(int id)
        {
            var result = await _kpiCategoryService.GetByIdAsync(id);
            return Ok(result);
        }

        // CREATE KPI CATEGORY
        [HttpPost("kpi-categories")]
        public async Task<IActionResult> CreateKpiCategory([FromBody] CreateUpdateKpiCategoryDto dto)
        {
            var result = await _kpiCategoryService.CreateAsync(dto);
            return Ok(result);
        }

        // UPDATE KPI CATEGORY
        [HttpPut("kpi-categories")]
        public async Task<IActionResult> UpdateKpiCategory([FromBody] CreateUpdateKpiCategoryDto dto)
        {
            var result = await _kpiCategoryService.UpdateAsync(dto);
            return Ok(result);
        }

        // DELETE KPI CATEGORY
        [HttpDelete("kpi-categories/{id:int}")]
        public async Task<IActionResult> DeleteKpiCategory(int id)
        {
            var result = await _kpiCategoryService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
#endregion
        #region Expense Status
        // =====================================================
        // EXPENSE STATUS
        // =====================================================

        // GET ALL EXPENSE STATUS
        [HttpGet("expense-status")]
        public async Task<IActionResult> GetExpenseStatus()
        {
            var result = await _expenseStatusService.GetAllAsync();
            return Ok(result);
        }

        // GET EXPENSE STATUS BY ID
        [HttpGet("expense-status/{id:int}")]
        public async Task<IActionResult> GetExpenseStatusById(int id)
        {
            var result = await _expenseStatusService.GetByIdAsync(id);
            return Ok(result);
        }

        // CREATE + UPDATE (POST)
        [HttpPost("expense-status")]
        public async Task<IActionResult> SaveExpenseStatus([FromBody] CreateUpdateExpenseStatusDto dto)
        {
            var result = await _expenseStatusService.SaveAsync(dto);
            return Ok(result);
        }

        // DELETE EXPENSE STATUS
        [HttpDelete("expense-status/{id:int}")]
        public async Task<IActionResult> DeleteExpenseStatus(int id)
        {
            var result = await _expenseStatusService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        #endregion

        #region leavestatus

        // =====================================================
        // LEAVE STATUS
        // =====================================================

        /// <summary>
        /// Retrieve all Leave Status records
        /// </summary>
        /// <returns>List of Leave Status</returns>
        [HttpGet("leave-status")]
        public async Task<IActionResult> GetLeaveStatus()
        {
            var result = await _leaveStatusService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieve Leave Status by ID
        /// </summary>
        /// <param name="id">Leave Status ID</param>
        /// <returns>Leave Status details</returns>
        [HttpGet("leave-status/{id:int}")]
        public async Task<IActionResult> GetLeaveStatusById(int id)
        {
            var result = await _leaveStatusService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Create a new Leave Status
        /// </summary>
        /// <param name="dto">Leave Status data</param>
        /// <returns>Success or failure message</returns>
        [HttpPost("leave-status")]
        public async Task<IActionResult> CreateLeaveStatus([FromBody] CreateUpdateLeaveStatusDto dto)
        {
            var result = await _leaveStatusService.CreateAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Update an existing Leave Status
        /// </summary>
        /// <param name="dto">Updated Leave Status data</param>
        /// <returns>Success or failure message</returns>
        [HttpPut("leave-status")]
        public async Task<IActionResult> UpdateLeaveStatus([FromBody] CreateUpdateLeaveStatusDto dto)
        {
            var result = await _leaveStatusService.UpdateAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Delete Leave Status by ID
        /// </summary>
        /// <param name="id">Leave Status ID</param>
        /// <returns>Success or not found message</returns>
        [HttpDelete("leave-status/{id:int}")]
        public async Task<IActionResult> DeleteLeaveStatus(int id)
        {
            var result = await _leaveStatusService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
        #endregion

        #region Attendance Status

        /// <summary>
        /// Retrieve all Attendance Status records
        /// </summary>
        /// <returns>List of Attendance Status</returns>
        [HttpGet("GetAttendanceStatus")]
        public async Task<IActionResult> GetAttendanceStatus()
        {
            var result = await _attendanceStatusService.GetAllAsync();
            return Ok(result);
        }
        /// <summary>
        /// Retrieve Attendance Status by ID
        /// </summary>
        /// <param name="id">Attendance Status ID</param>
        /// <returns>Attendance Status details</returns>
        [HttpGet("GetAttendanceStatusById/{id}")]
        public async Task<IActionResult> GetAttendanceStatusById(int id)
        {
            var result = await _attendanceStatusService.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
        /// <summary>
        /// Create new Attendance Status
        /// </summary>
        /// <param name="dto">Attendance Status details</param>
        /// <returns>Success message</returns>
        [HttpPost("CreateAttendanceStatus")]
        public async Task<IActionResult> CreateAttendanceStatus([FromBody] CreateUpdateAttendanceStatusDto dto)
        {
            var result = await _attendanceStatusService.CreateAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// Update existing Attendance Status
        /// </summary>
        /// <param name="dto">Attendance Status details</param>
        /// <returns>Success message</returns>
        [HttpPut("UpdateAttendanceStatus")]
        public async Task<IActionResult> UpdateAttendanceStatus([FromBody] CreateUpdateAttendanceStatusDto dto)
        {
            var result = await _attendanceStatusService.UpdateAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// Delete Attendance Status by ID
        /// </summary>
        /// <param name="id">Attendance Status ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("DeleteAttendanceStatus/{id}")]
        public async Task<IActionResult> DeleteAttendanceStatus(int id)
        {
            var result = await _attendanceStatusService.DeleteAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        #endregion

    }
}
