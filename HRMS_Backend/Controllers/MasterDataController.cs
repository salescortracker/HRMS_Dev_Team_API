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
        private readonly ICompanyNewsService _companyNewsService;
        private readonly IWebHostEnvironment _env;
        private readonly ICategoryServicecs _categoryService;
        private readonly IBloodGroupService _bloodGroupService;


        public MasterDataController(IDepartmentService service, IDesignationService designationService, IGenderService genderService, IBloodGroupService bloodGroupservice, ILogger<MasterDataController> logger, ICompanyNewsService companyNewsService, IWebHostEnvironment env, ICategoryServicecs categoryService,IBloodGroupService bloodGroupService)
        {
            _service = service;
            _designationService = designationService;
            _genderService = genderService;
            _companyNewsService = companyNewsService;
            _logger = logger;
            _env = env;
            _bloodGroupService = bloodGroupservice; // ✅ MISSING LINE (VERY IMPORTANT)
            _categoryService = categoryService;
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

        // ======================================
        // BLOOD GROUP MASTER
        // ======================================

        // GET: api/MasterData/bloodgroups
        [HttpGet("bloodgroups")]
        public async Task<IActionResult> GetBloodGroups()
        {
            var data = await _bloodGroupService.GetAllAsync();
            return Ok(data);
        }
        // GET: api/MasterData/bloodgroups/5
        [HttpGet("bloodgroups/{id:int}")]
        public async Task<IActionResult> GetBloodGroupById(int id)
        {
            var data = await _bloodGroupService.GetByIdAsync(id);
            if (data == null)
                return NotFound(new { message = "Blood group not found" });

            return Ok(data);
        }

        // POST: api/MasterData/bloodgroups
        [HttpPost("bloodgroups")]
        public async Task<IActionResult> CreateBloodGroup([FromBody] BloodGroupDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _bloodGroupService.CreateAsync(dto);
            return Ok(new { message = result });
        }

        // PUT: api/MasterData/bloodgroups/5
        [HttpPut("bloodgroups/{id:int}")]
        public async Task<IActionResult> UpdateBloodGroup(
            int id,
            [FromBody] BloodGroupDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dto.BloodGroupId = id;

            var result = await _bloodGroupService.UpdateAsync(dto);

            if (result == "Blood group not found")
                return NotFound(new { message = result });

            return Ok(new { message = result });
        }

        // DELETE: api/MasterData/bloodgroups/5
        [HttpDelete("bloodgroups/{id:int}")]
        public async Task<IActionResult> DeleteBloodGroup(int id)
        {
            var result = await _bloodGroupService.DeleteAsync(id);

            if (result == "Blood group not found")
                return NotFound(new { message = result });

            return Ok(new { message = result });
        }

        // ================= COMPANY NEWS =================

        [HttpPost("CreateCompanyNews")]
        public async Task<IActionResult> CreateCompanyNews([FromForm] CompanyNewsDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                string root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string path = Path.Combine(root, "Uploads", "CompanyNews");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                // 🔹 File handling
                if (dto.UploadFile != null && dto.UploadFile.Length > 0)
                {
                    string fileName = $"{Guid.NewGuid()}_{dto.UploadFile.FileName}";
                    string fullPath = Path.Combine(path, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await dto.UploadFile.CopyToAsync(stream);

                    dto.AttachmentName = fileName;
                    dto.AttachmentPath = $"Uploads/CompanyNews/{fileName}";
                }

                int userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

                dto.CreatedBy = userId;



                var newsId = await _companyNewsService.CreateAsync(dto);

                return Ok(new { message = "Company news saved successfully", NewsId = newsId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating company news");
                return StatusCode(500, "Error while creating company news");
            }
        }

        [HttpPut("UpdateCompanyNews/{id}")]
        public async Task<IActionResult> UpdateCompanyNews(int id, [FromForm] CompanyNewsDto dto)
        {
            if (id != dto.NewsId)
                return BadRequest("Id mismatch");

            try
            {
                string root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string path = Path.Combine(root, "Uploads", "CompanyNews");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                // 🔹 File handling
                if (dto.UploadFile != null && dto.UploadFile.Length > 0)
                {
                    string fileName = $"{Guid.NewGuid()}_{dto.UploadFile.FileName}";
                    string fullPath = Path.Combine(path, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await dto.UploadFile.CopyToAsync(stream);

                    dto.AttachmentName = fileName;
                    dto.AttachmentPath = $"Uploads/CompanyNews/{fileName}";
                }

                int userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");


                dto.UpdatedBy = userId;


                await _companyNewsService.UpdateAsync(dto);

                return Ok(new { message = "Company news updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating company news");
                return StatusCode(500, "Error while updating company news");
            }
        }

        [HttpGet("GetCompanyNews")]
        public async Task<IActionResult> GetCompanyNews(
            [FromQuery] int companyId,
            [FromQuery] int regionId)
        {
            try
            {
                var result = await _companyNewsService.GetAllAsync(companyId, regionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching company news");
                return StatusCode(500, "Error while fetching company news");
            }
        }

        [HttpDelete("DeleteCompanyNews/{newsId}")]
        public async Task<IActionResult> DeleteCompanyNews(int newsId)
        {
            try
            {
                await _companyNewsService.DeleteAsync(newsId);
                return Ok("Company news deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting company news");
                return StatusCode(500, "Error while deleting company news");
            }
        }
        [HttpGet("GetCompanyNewsForSuperAdmin")]
        public async Task<IActionResult> GetCompanyNewsForSuperAdmin(
     [FromQuery] string category)
        {
            if (string.IsNullOrEmpty(category))
                return BadRequest("Category is required");

            try
            {
                var result = await _companyNewsService.GetForSuperAdminAsync(category);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Super Admin news");
                return StatusCode(500, "Error while fetching company news");
            }
        }



        #region Categories

        [HttpGet]
        [Route("GetActiveCategories")]
        public async Task<IActionResult> GetAllActiveCategories()
        {
            var data = await _categoryService.GetActiveCategoriesAsync();
            return Ok(data);
        }

     


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(int companyId, int regionId)
        {
            return Ok(await _categoryService.GetAllAsync(companyId, regionId));
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetAllById(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] CompanyPolicyDto dto)
        {
            UploadFile(dto);
            var result = await _categoryService.AddAsync(dto);
            return Ok(new { message = "Policy created successfully", data = result });
        }

        [HttpPost("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] CompanyPolicyDto dto)
        {
            UploadFile(dto);
            var result = await _categoryService.UpdateAsync(id, dto);
            return Ok(new { message = "Policy updated successfully", data = result });
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool success = await _categoryService.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Policy deleted successfully" });
        }

        private void UploadFile(CompanyPolicyDto dto)
        {
            if (dto.File == null || dto.File.Length == 0) return;

            string root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string path = Path.Combine(root, "Uploads", "CompanyPolicies");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
            string fullPath = Path.Combine(path, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            dto.File.CopyTo(stream);

            dto.FileName = fileName;
            dto.FilePath = $"Uploads/CompanyPolicies/{fileName}";
        }
        [HttpGet("GetAllPolicies")]
        public async Task<IActionResult> GetAllPolicies()
        {
            var result = await _categoryService.GetAllPoliciesAsync();
            return Ok(result);
        }

        #endregion
    }
}


    