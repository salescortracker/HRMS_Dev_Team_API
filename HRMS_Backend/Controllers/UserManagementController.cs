using BusinessLayer.DTOs;
using BusinessLayer.Implementations;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly IRegionService _regionService;
        private readonly IUserService _userService;
        private readonly IMenuMasterService _menuService;
        private readonly IRoleMasterService _roleService;
        private readonly IMenuRoleService _menuRoleService;
        private readonly IEmployeeEducationService _employeeEducationService;
        private readonly IEmployeeCertificationService _employeeCertificationService;
        private readonly IEmployeeJobHistoryService _employeeJobHistoryService;
        private readonly IWebHostEnvironment _env;


        public UserManagementController(ICompanyService companyService, IRegionService regionService, IUserService userService
            , IMenuMasterService menuService, IRoleMasterService roleService, IMenuRoleService menuRoleService, IEmployeeEducationService employeeEducationService, IWebHostEnvironment env,IEmployeeCertificationService employeeCertificationService, IEmployeeJobHistoryService employeeJobHistoryService)
        {
            _companyService = companyService;
            _regionService = regionService;
            _userService = userService;
            _menuService = menuService;
            _roleService = roleService;
            _menuRoleService = menuRoleService;
            _employeeEducationService = employeeEducationService;
            _employeeCertificationService = employeeCertificationService;
            _env = env;
            _employeeJobHistoryService = employeeJobHistoryService;
        }
        public class BulkInsertRequest
        {
            public string EntityName { get; set; }
            public List<object> Data { get; set; }
        }
        public class BulkInsertResult<T>
        {
            public int InsertedCount { get; set; }
            public int DuplicateCount { get; set; }
            public List<T> InsertedRecords { get; set; }
            public List<T> DuplicateRecords { get; set; }
        }

        #region Company Details
        /// <summary>
        /// Retrieves a list of all companies.
        /// </summary>
        /// <remarks>This method performs an asynchronous operation to fetch all companies from the data
        /// source.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing a collection of companies.  Returns an HTTP 200 status code with
        /// the list of companies if successful.</returns>
        [HttpGet]
        [Route("GetCompany")]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _companyService.GetAllCompaniesAsync();
            return Ok(companies);
        }
        /// <summary>
        /// Retrieves a company by its unique identifier.
        /// </summary>
        /// <remarks>This method performs an asynchronous operation to fetch the company details. Ensure
        /// the <paramref name="id"/> corresponds to a valid company record.</remarks>
        /// <param name="id">The unique identifier of the company to retrieve.</param>
        /// <returns>An <see cref="IActionResult"/> containing the company data if found; otherwise, a <see
        /// cref="NotFoundResult"/> if the company does not exist.</returns>
        [HttpGet]
        [Route("GetCompanyById")]
        public async Task<IActionResult> GetById(int id)
        {
            var company = await _companyService.GetCompanyByIdAsync(id);
            if (company == null) return NotFound();
            return Ok(company);
        }
        /// <summary>
        /// Searches for companies based on the specified filter criteria.
        /// </summary>
        /// <remarks>The filter object must be structured according to the requirements of the underlying
        /// search service. Ensure that the filter contains valid criteria to avoid unexpected results.</remarks>
        /// <param name="filter">An object containing the filter criteria for the search. The structure and fields of the filter object
        /// depend on the implementation of the search service.</param>
        /// <returns>An <see cref="IActionResult"/> containing the search results. The result is a collection of companies that
        /// match the specified filter criteria.</returns>
        [HttpPost]
        [Route("GetCompanySearch")]
        public async Task<IActionResult> Search([FromBody] object filter)
        {
            var companies = await _companyService.SearchCompaniesAsync(filter);
            return Ok(companies);
        }
        /// <summary>
        /// Creates a new company and returns the created resource with its location.
        /// </summary>
        /// <remarks>This method uses the HTTP POST verb to create a new company. The created resource's
        /// URI is included in the response.</remarks>
        /// <param name="dto">The data transfer object containing the details of the company to create.</param>
        /// <returns>A <see cref="CreatedAtActionResult"/> containing the details of the created company and the URI of the
        /// resource.</returns>
        [HttpPost]
        [Route("SaveCompany")]
        public async Task<IActionResult> Create([FromBody] CompanyDto dto)
        {
            var company = await _companyService.AddCompanyAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = company.CompanyId }, company);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("UpdateCompany/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CompanyDto dto)
        {
            var updated = await _companyService.UpdateCompanyAsync(id, dto);
            return Ok(updated);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [HttpDelete("DeleteCompany/{id}")]
       
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _companyService.DeleteCompanyAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        [HttpPost]
        [Route("BulkInsert")]
        public async Task<IActionResult> BulkInsert([FromBody] BulkInsertRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.EntityName) || request.Data == null || !request.Data.Any())
                    return BadRequest(new { Success = false, Message = "Invalid request. No data provided." });

                switch (request.EntityName.ToLower())
                {
                    case "company":

                        var companies = new List<CompanyDto>();

                        foreach (var item in request.Data)
                        {
                            // Handle both stringified and object JSON
                            CompanyDto? company = null;

                            switch (item)
                            {
                                case string jsonString:
                                    company = JsonConvert.DeserializeObject<CompanyDto>(jsonString);
                                    break;

                                case JObject jobj:
                                    company = jobj.ToObject<CompanyDto>();
                                    break;

                                case JsonElement jsonElement:
                                    if (jsonElement.ValueKind == JsonValueKind.String)
                                    {
                                        var innerJson = jsonElement.GetString();
                                        if (!string.IsNullOrWhiteSpace(innerJson))
                                            company = JsonConvert.DeserializeObject<CompanyDto>(innerJson);
                                    }
                                    else
                                    {
                                        var json = jsonElement.GetRawText();
                                        company = JsonConvert.DeserializeObject<CompanyDto>(json);
                                    }
                                    break;
                            }

                            if (company != null)
                                companies.Add(company);
                        }

                        if (!companies.Any())
                            return BadRequest(new { Success = false, Message = "Failed to parse company data. Invalid JSON format." });

                        // ✅ Call service layer
                        var result = await _companyService.AddCompaniesAsync(companies);

                        return Ok(new
                        {
                            Success = true,
                            Message = $"{result.Count()} companies inserted successfully. {result.Count()} duplicate(s) skipped.",
                            Summary = result
                        });

                    // Add more entity cases as needed
                    

                    default:
                        return BadRequest(new { Success = false, Message = "Unsupported entity type." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.InnerException?.Message ?? ex.Message ?? "An unexpected error occurred. Please contact IT Administrator."
                });
            }
        }




        #endregion
        #region Region Details
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRegion")]
        public async Task<IActionResult> GetRegion()
        {
            var regions = await _regionService.GetAllRegionsAsync();
            return Ok(regions);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetRegionById")]
        public async Task<IActionResult> GetRegionById(int id)
        {
            var region = await _regionService.GetRegionByIdAsync(id);
            if (region == null) return NotFound();
            return Ok(region);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveRegion")]
        public async Task<IActionResult> SaveRegion([FromBody] object model)
        {
            var region = await _regionService.AddRegionAsync(model);
            return CreatedAtAction(nameof(GetRegionById), new { id = region.RegionID }, region);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateRegion/{id}")]
        public async Task<IActionResult> UpdateRegion(int id, [FromBody] object model)
        {
            var region = await _regionService.UpdateRegionAsync(id, model);
            return Ok(region);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteRegion/{id}")]
        public async Task<IActionResult> DeleteRegion(int id)
        {
            var result = await _regionService.DeleteRegionAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetRegionSearch")]
        public async Task<IActionResult> GetRegionSearch([FromBody] object filter)
        {
            var regions = await _regionService.SearchRegionsAsync(filter);
            return Ok(regions);
        }
        #endregion
        #region User Details
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
        {
            if (userDto == null)
                return BadRequest("Invalid user data");

            var createdUser = await _userService.CreateUserAsync(userDto);

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.UserId }, createdUser);
        }

      

        [HttpPut("UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            var updatedUser = await _userService.UpdateUserAsync(id, user);
            if (updatedUser == null)
                return NotFound();

            return Ok(updatedUser);
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userService.DeleteUserAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            try
            {
                var user = await _userService.VerifyLoginAsync(model.Email, model.Password);

                if (user == null)
                    return Unauthorized(new { message = "Invalid username or password" });

                return Ok(new { message = "Login successful", user });
            }
            catch (Exception ex)
            {
                // Handle unexpected API-level issues
                Console.WriteLine($"Login failed: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while processing your login." });
            }
        }

        #endregion
        #region Menu Master Details
        /// <summary>
        /// Get all menus
        /// </summary>
        [HttpGet("GetAllMenus")]
        public async Task<IActionResult> GetAllMenus()
       {
            var menus = await _menuService.GetAllMenusAsync();
            return Ok(menus);
        }

        /// <summary>
        /// Get menu by ID
        /// </summary>
        [HttpGet("GetMenuById/{id:int}")]
        public async Task<IActionResult> GetMenuById(int id)
        {
            var menu = await _menuService.GetMenuByIdAsync(id);
            if (menu == null)
                return NotFound(new { message = "Menu not found" });

            return Ok(menu);
        }

        /// <summary>
        /// Search menus dynamically by MenuName, ParentMenuID, IsActive, etc.
        /// </summary>
        [HttpPost("SearchMenus")]
        public async Task<IActionResult> SearchMenus([FromBody] object filter)
        {
            var results = await _menuService.SearchMenusAsync(filter);
            return Ok(results);
        }

        /// <summary>
        /// Create a new menu
        /// </summary>
        [HttpPost("CreateMenu")]
        public async Task<IActionResult> CreateMenu([FromBody] MenuMasterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Example: retrieve CreatedBy from token later if you have authentication
            var createdBy = 1; // placeholder for now
            var menu = await _menuService.AddMenuAsync(dto, createdBy);
            return CreatedAtAction(nameof(GetMenuById), new { id = menu.MenuID }, menu);
        }

        /// <summary>
        /// Update an existing menu
        /// </summary>
        [HttpPost("UpdateMenu/{id:int}")]
        public async Task<IActionResult> UpdateMenu(int id, [FromBody] MenuMasterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var modifiedBy = 1; // placeholder for now
            try
            {
                var updatedMenu = await _menuService.UpdateMenuAsync(id, dto, modifiedBy);
                return Ok(updatedMenu);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete menu by ID
        /// </summary>
        [HttpPost("DeleteMenu/{id:int}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var deleted = await _menuService.DeleteMenuAsync(id);
            if (!deleted)
                return NotFound(new { message = "Menu not found" });

            return NoContent();
        }

        /// <summary>
        /// Get all active menus
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveMenus()
        {
            var activeMenus = await _menuService.GetActiveMenusAsync();
            return Ok(activeMenus);
        }
        #endregion
        #region Role Details
        // ✅ GET: api/RoleMaster
        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        // ✅ GET: api/RoleMaster/{id}
        [HttpGet("GetRoleById/{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound(new { message = "Role not found" });

            return Ok(role);
        }

        // ✅ POST: api/RoleMaster
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleMasterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdRole = await _roleService.AddRoleAsync(dto);
            return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.RoleId }, createdRole);
        }

        // ✅ PUT: api/RoleMaster/{id}
        [HttpPost("UpdateRole/{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleMasterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedRole = await _roleService.UpdateRoleAsync(id, dto);
                return Ok(updatedRole);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // ✅ DELETE: api/RoleMaster/{id}
        [HttpPost("DeleteRole/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var deleted = await _roleService.DeleteRoleAsync(id);
            if (!deleted)
                return NotFound(new { message = "Role not found or already deleted" });

            return Ok(new { message = "Role deleted successfully" });
        }

        // ✅ POST: api/RoleMaster/search
        [HttpPost("search")]
        public async Task<IActionResult> SearchRoles(
            [FromBody] object filter,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool isDescending = false)
        {
            var roles = await _roleService.SearchRolesAsync(filter, pageNumber, pageSize, sortBy, isDescending);
            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = roles.Count(),
                Data = roles
            });
        }
        #endregion
        #region menurolemaster
        /// <summary>
        /// Assign permissions for multiple roles.
        /// </summary>
        [HttpPost("AssignMultipleRoles")]
        public async Task<IActionResult> AssignPermissionsToMultipleRoles([FromBody] List<RolePermissionRequestDto> rolePermissions)
        {
            if (rolePermissions == null || !rolePermissions.Any())
                return BadRequest("Role permissions data cannot be empty.");

            var success = await _menuRoleService.AssignPermissionsToMultipleRolesAsync(rolePermissions);
            return success ? Ok(new { Message = "Permissions assigned successfully." }) : StatusCode(500, "Failed to assign permissions.");
        }

        /// <summary>
        /// Get permissions for multiple roles.
        /// </summary>
        [HttpPost("GetPermissionsForMultipleRoles")]
        public async Task<IActionResult> GetPermissionsForMultipleRoles([FromBody] List<int> roleIds)
        {
            if (roleIds == null || !roleIds.Any())
                return BadRequest("Role IDs list cannot be empty.");

            var result = await _menuRoleService.GetPermissionsForMultipleRolesAsync(roleIds);
            return Ok(result);
        }
        /// <summary>
        /// Assign permissions for a single role.
        /// </summary>
        [HttpPost("assign-permissions/{roleId}")]
        public async Task<IActionResult> AssignPermissionsToRole(int roleId, [FromBody] List<MenuRoleDto> permissions)
        {
            if (permissions == null || !permissions.Any())
                return BadRequest("Permissions list cannot be empty.");

            var success = await _menuRoleService.AssignPermissionsToRoleAsync(roleId, permissions);
            if (success)
                return Ok(new { message = "Permissions assigned successfully." });

            return StatusCode(500, "Failed to assign permissions.");
        }

        /// <summary>
        /// Get all assigned permissions for a role.
        /// </summary>
        [HttpGet("get-permissions/{roleId}")]
        public async Task<IActionResult> GetPermissionsByRole(int roleId)
        {
            var result = await _menuRoleService.GetPermissionsByRoleAsync(roleId);
            return Ok(result);
        }
        [HttpGet("GetAllMenusByRoleId/{roleId}")]
        public async Task<IActionResult> GetAllMenusByRoleId(int roleId)
        {
            try
            {
                var permissions = await _menuRoleService.GetAllMenusByRoleId(roleId);

                if (permissions == null || !permissions.Any())
                    return NotFound(new { message = "No permissions found for this role." });

                return Ok(permissions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching permissions: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving permissions." });
            }
        }
        #endregion

        #region Employee Education
        [HttpGet("education")]
        public async Task<IActionResult> GetAllEducation()
        {
            var data = await _employeeEducationService.GetAllAsync();
            return Ok(data);
        }

  
        [HttpGet("user/{userId}/education")]
        public async Task<IActionResult> GetEducationByUserId(int userId)
        {
            var result = await _employeeEducationService.GetByUserIdAsync(userId);

            // Return empty list instead of 404
            if (result == null || !result.Any())
                return Ok(new List<EmployeeEducationDto>());

            return Ok(result);
        }



        [HttpGet("education/{id}")]
        public async Task<IActionResult> GetEducationById(int id)
        {
            var data = await _employeeEducationService.GetByIdAsync(id);
            if (data == null)
                return NotFound(new { message = "Education not found" });

            return Ok(data);
        }

        [HttpPost("education")]
        public async Task<IActionResult> AddEducation([FromForm] EmployeeEducationDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            string path = Path.Combine(root, "Uploads", "EmployeeEducationCertificates");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (model.CertificateFile != null && model.CertificateFile.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}_{model.CertificateFile.FileName}";
                string fullPath = Path.Combine(path, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await model.CertificateFile.CopyToAsync(stream);

                model.CertificateFilePath = $"Uploads/EmployeeEducationCertificates/{fileName}";
            }

            model.CreatedBy = model.UserId;

            var id = await _employeeEducationService.AddAsync(model);

            return Ok(new { message = "Saved successfully", id });
        }

        [HttpPut("education/{id}")]
        public async Task<IActionResult> UpdateEducation(int id, [FromForm] EmployeeEducationDto model)
        {
            if (id != model.EducationId)
                return BadRequest("Id mismatch");

             string root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            string path = Path.Combine(root, "Uploads", "EmployeeEducationCertificates");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (model.CertificateFile != null && model.CertificateFile.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}_{model.CertificateFile.FileName}";
                string fullPath = Path.Combine(path, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await model.CertificateFile.CopyToAsync(stream);

                model.CertificateFilePath = $"Uploads/EmployeeEducationCertificates/{fileName}";
            }

            model.CreatedBy = model.UserId;
            model.ModifiedBy = model.UserId;

            var result = await _employeeEducationService.UpdateAsync(model);
            if (!result)
                return NotFound(new { message = "Record not found" });

            return Ok(new { message = "Updated successfully" });
        }

        /// <summary>
        /// Delete a family record
        /// </summary>
        [HttpDelete("family/{id}")]
        public async Task<IActionResult> DeleteFamily(int id)
        {
            var result = await _employeeFamilyService.DeleteAsync(id);
        [HttpDelete("education/{id}")]
        public async Task<IActionResult> DeleteEducation(int id)
        {
            var result = await _employeeEducationService.DeleteAsync(id);

            if (!result)
                return NotFound(new { message = "Record not found" });

            return Ok(new { message = "Deleted successfully" });
        }

        /// <summary>
        /// Get relationship list for dropdowns
        /// </summary>
        [HttpGet("relationships")]
        public async Task<IActionResult> GetRelationships()
        {
            var list = await _employeeFamilyService.GetRelationshipListAsync();
            return Ok(list);
        }

        /// <summary>
        /// Get gender list for dropdown
        /// </summary>
        [HttpGet("genders")]
        public async Task<IActionResult> GetGenders()
        {
            var list = await _employeeFamilyService.GetGenderListAsync();
            return Ok(list);
        }


        #endregion

        #region Employee Emergency Contact Details

        /// <summary>
        /// Get all emergency contact records
        /// </summary>
        [HttpGet("emergency-contact")]
        public async Task<IActionResult> GetAllEmergencyContacts()
        {
            var data = await _employeeEmergencyContactService.GetAllAsync();
        [HttpGet("modeofstudy")]
        public async Task<IActionResult> GetModeOfStudy()
        {
            var data = await _employeeEducationService.GetModeOfStudyListAsync();
            return Ok(data);
        }

        #endregion

        #region Employee Certifications

        /// <summary>
        /// Retrieves all employee certifications.
        /// </summary>
        /// <returns>A list of EmployeeCertificationDto objects.</returns>
        [HttpGet("certifications")]
        public async Task<IActionResult> GetAllCertifications()
        {
            var data = await _employeeCertificationService.GetAllAsync();
            return Ok(data);
        }

        /// <summary>
        /// Get emergency contacts for a specific user
        /// </summary>
        [HttpGet("user/{userId}/emergency-contact")]
        public async Task<IActionResult> GetUserEmergencyContacts(int userId)
        /// Retrieves certifications for a specific user.
        /// </summary>
        /// <param name="userId">The user ID to filter certifications.</param>
        /// <returns>A list of EmployeeCertificationDto objects for the given user.</returns>
        [HttpGet("user/{userId}/certifications")]
        public async Task<IActionResult> GetUserCertifications(int userId)
        {
            if (userId <= 0)
                return BadRequest(new { message = "Invalid userId" });

            var data = await _employeeEmergencyContactService.GetByUserIdAsync(userId);

            if (data == null || !data.Any())
                return NotFound(new { message = "No emergency contact details found" });
            var data = await _employeeCertificationService.GetByUserIdAsync(userId);

            if (data == null || !data.Any())
                return NotFound(new { message = "No certifications found" });

            return Ok(data);
        }

        /// <summary>
        /// Get a single emergency contact record by ID
        /// </summary>
        [HttpGet("emergency-contact/{id}")]
        public async Task<IActionResult> GetEmergencyContactById(int id)
        {
            var data = await _employeeEmergencyContactService.GetByIdAsync(id);

            if (data == null)
                return NotFound(new { message = "Emergency contact record not found" });
        /// Retrieves a certification by its ID.
        /// </summary>
        /// <param name="id">The certification ID.</param>
        /// <returns>An EmployeeCertificationDto object if found.</returns>
        [HttpGet("certifications/{id}")]
        public async Task<IActionResult> GetCertificationById(int id)
        {
            var data = await _employeeCertificationService.GetByIdAsync(id);
            if (data == null)
                return NotFound(new { message = "Certification not found" });

            return Ok(data);
        }

        /// <summary>
        /// Add a new emergency contact entry
        /// </summary>
        [HttpPost("emergency-contact")]
        public async Task<IActionResult> AddEmergencyContact([FromBody] EmployeeEmergencyContactDto model)
        /// Adds a new employee certification.
        /// </summary>
        /// <param name="model">The EmployeeCertificationDto object containing certification details and file.</param>
        /// <returns>The ID of the newly created certification.</returns>
        [HttpPost("certifications")]
        public async Task<IActionResult> AddCertification([FromForm] EmployeeCertificationDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Audit - createdBy
            if (string.IsNullOrEmpty(model.CreatedBy) && model.UserId != 0)
                model.CreatedBy = model.UserId.ToString();

            var id = await _employeeEmergencyContactService.AddAsync(model);

            string root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string path = Path.Combine(root, "Uploads", "EmployeeCertifications");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (model.DocumentFile != null && model.DocumentFile.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}_{model.DocumentFile.FileName}";
                string fullPath = Path.Combine(path, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await model.DocumentFile.CopyToAsync(stream);

                model.DocumentPath = $"Uploads/EmployeeCertifications/{fileName}";
            }
            model.CreatedBy = model.UserId;

            var id = await _employeeCertificationService.AddAsync(model);
            return Ok(new { message = "Saved successfully", id });
        }

        /// <summary>
        /// Update an existing emergency contact entry
        /// </summary>
        [HttpPut("emergency-contact/{id}")]
        public async Task<IActionResult> UpdateEmergencyContact(int id, [FromBody] EmployeeEmergencyContactDto model)
        {
            if (id != model.EmergencyContactId)
                return BadRequest(new { message = "Id mismatch" });

            // Audit - modifiedBy
            if (string.IsNullOrEmpty(model.ModifiedBy) && model.UserId != 0)
                model.ModifiedBy = model.UserId.ToString();

            var result = await _employeeEmergencyContactService.UpdateAsync(model);
        /// Updates an existing employee certification.
        /// </summary>
        /// <param name="id">The certification ID.</param>
        /// <param name="model">The EmployeeCertificationDto object containing updated details and file.</param>
        /// <returns>A success message if updated.</returns>
        [HttpPut("certifications/{id}")]
        public async Task<IActionResult> UpdateCertification(int id, [FromForm] EmployeeCertificationDto model)
        {
            if (id != model.CertificationId)
                return BadRequest("Id mismatch");

            string root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string path = Path.Combine(root, "Uploads", "EmployeeCertifications");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (model.DocumentFile != null && model.DocumentFile.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}_{model.DocumentFile.FileName}";
                string fullPath = Path.Combine(path, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await model.DocumentFile.CopyToAsync(stream);

                model.DocumentPath = $"Uploads/EmployeeCertifications/{fileName}";
            }

            model.ModifiedBy = model.UserId;

            var result = await _employeeCertificationService.UpdateAsync(model);

            if (!result)
                return NotFound(new { message = "Record not found" });

            return Ok(new { message = "Updated successfully" });
        }

        /// <summary>
        /// Delete an emergency contact record
        /// </summary>
        [HttpDelete("emergency-contact/{id}")]
        public async Task<IActionResult> DeleteEmergencyContact(int id)
        {
            var result = await _employeeEmergencyContactService.DeleteAsync(id);
        /// Deletes an employee certification by ID.
        /// </summary>
        /// <param name="id">The certification ID.</param>
        /// <returns>A success message if deleted.</returns>
        [HttpDelete("certifications/{id}")]
        public async Task<IActionResult> DeleteCertification(int id)
        {
            var result = await _employeeCertificationService.DeleteAsync(id);

            if (!result)
                return NotFound(new { message = "Record not found" });

            return Ok(new { message = "Deleted successfully" });
        }

        /// <summary>
        /// Get relationship list for emergency contact dropdowns
        /// </summary>
        [HttpGet("emergency-contact/relationships")]
        public async Task<IActionResult> GetEmergencyContactRelationships()
        {
            var list = await _employeeEmergencyContactService.GetRelationshipListAsync();
            return Ok(list);
        /// Retrieves all active certification types.
        /// </summary>
        /// <returns>A list of CertificationTypeDto objects.</returns>
        [HttpGet("certificationtypes")]
        public async Task<IActionResult> GetCertificationTypes()
        {
            var data = await _employeeCertificationService.GetCertificationTypesAsync();
            return Ok(data);
        }

        #endregion

        #region Employee References
      

        /// <summary>
        /// Get all employee references
        /// </summary>
        [HttpGet("references")]
        public async Task<IActionResult> GetAllReferences()
        {
            var data = await _employeeReferenceService.GetAllAsync();

        #region Employee Job History

        /// <summary>
        /// Get all employee job history records
        /// </summary>
        [HttpGet("jobhistory")]
        public async Task<IActionResult> GetAllJobHistory()
        {
            var data = await _employeeJobHistoryService.GetAllAsync();
            return Ok(data);
        }

        /// <summary>
        /// Get employee references for a specific user
        /// </summary>
        [HttpGet("user/{userId}/references")]
        public async Task<IActionResult> GetReferencesByUser(int userId)
        /// Get job history records for a specific user
        /// </summary>
        [HttpGet("user/{userId}/jobhistory")]
        public async Task<IActionResult> GetUserJobHistory(int userId)
        {
            if (userId <= 0)
                return BadRequest(new { message = "Invalid userId" });

            var data = await _employeeReferenceService.GetByUserIdAsync(userId);

            if (data == null || !data.Any())
                return NotFound(new { message = "No reference records found" });
            var data = await _employeeJobHistoryService.GetByUserIdAsync(userId);

            if (data == null || !data.Any())
                return NotFound(new { message = "No job history found" });

            return Ok(data);
        }

        /// <summary>
        /// Get a single employee reference by ID
        /// </summary>
        [HttpGet("references/{id}")]
        public async Task<IActionResult> GetReferenceById(int id)
        {
            var data = await _employeeReferenceService.GetByIdAsync(id);

            if (data == null)
                return NotFound(new { message = "Reference record not found" });
        /// Get a single job history record by ID
        /// </summary>
        [HttpGet("jobhistory/{id}")]
        public async Task<IActionResult> GetJobHistoryById(int id)
        {
            var data = await _employeeJobHistoryService.GetByIdAsync(id);

            if (data == null)
                return NotFound(new { message = "Job history not found" });

            return Ok(data);
        }

        /// <summary>
        /// Create a new employee reference record
        /// </summary>
        [HttpPost("references")]
        public async Task<IActionResult> AddReference([FromBody] EmployeeReferenceDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            model.CreatedBy = (model.CreatedBy == null || model.CreatedBy == 0)
                ? model.UserId
                : model.CreatedBy;

            var id = await _employeeReferenceService.AddAsync(model);

            return Ok(new { message = "Saved successfully", id });
        }

        /// <summary>
        /// Update an existing employee reference
        /// </summary>
        [HttpPut("references/{id}")]
        public async Task<IActionResult> UpdateReference(int id, [FromBody] EmployeeReferenceDto model)
        {
            if (id != model.ReferenceId)
                return BadRequest(new { message = "Id mismatch" });

            if (!ModelState.IsValid) return BadRequest(ModelState);

            model.ModifiedBy = (model.ModifiedBy == null || model.ModifiedBy == 0)
                ? model.UserId
                : model.ModifiedBy;

            var result = await _employeeReferenceService.UpdateAsync(model);

            if (!result)
                return NotFound(new { message = "Record not found" });

            return Ok(new { message = "Updated successfully" });
        }

        /// <summary>
        /// Delete employee reference record
        /// </summary>
        [HttpDelete("references/{id}")]
        public async Task<IActionResult> DeleteReference(int id)
        {
            var result = await _employeeReferenceService.DeleteAsync(id);

            if (!result)
                return NotFound(new { message = "Record not found" });

            return Ok(new { message = "Deleted successfully" });
        }

        #endregion

        // GET api/UserManagement/marital-statuses/active/names
        [HttpGet("marital-statuses/active/names")]
        public async Task<IActionResult> GetActiveMaritalStatusNames()
        {
            var names = await _employeeFamilyService.GetActiveNamesAsync();
            return Ok(names); // returns string[] e.g. ["Single","Married"]
        }

        [HttpGet("personal")]
        public async Task<IActionResult> GetAllPersonal()
        {
            var data = await _employeePersonalService.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("user/{userId}/personal")]
        public async Task<IActionResult> GetPersonalByUser(int userId)
        {
            if (userId <= 0) return BadRequest(new { message = "Invalid userId" });

            var data = await _employeePersonalService.GetByUserIdAsync(userId);
            if (data == null || !data.Any()) return NotFound(new { message = "No personal details found" });

            return Ok(data);
        }

        [HttpGet("personal/{id}")]
        public async Task<IActionResult> GetPersonalById(int id)
        {
            var data = await _employeePersonalService.GetByIdAsync(id);
            if (data == null) return NotFound(new { message = "Record not found" });
            return Ok(data);
        }

        [HttpPost("personal")]
        public async Task<IActionResult> AddPersonal([FromForm] EmployeePersonalDetailDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            string? profilePath = null;
            if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                string root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string folder = Path.Combine(root, "Uploads", "EmployeeProfilePictures");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                string fileName = $"{Guid.NewGuid()}_{model.ProfilePicture.FileName}";
                string fullPath = Path.Combine(folder, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await model.ProfilePicture.CopyToAsync(stream);

                profilePath = $"Uploads/EmployeeProfilePictures/{fileName}";
                model.ProfilePicturePath = profilePath;
                model.ProfilePictureName = model.ProfilePicture.FileName;

                // optional: populate base64 if you store in DB
                // using var ms = new MemoryStream();
                // await model.ProfilePicture.CopyToAsync(ms);
                // model.ProfilePictureBase64 = Convert.ToBase64String(ms.ToArray());
            }

            model.CreatedBy = model.UserId;
            model.CreatedAt = DateTime.Now;

            var id = await _employeePersonalService.AddAsync(model);
            return Ok(new { message = "Saved successfully", id });
        }

        [HttpPut("personal/{id}")]
        public async Task<IActionResult> UpdatePersonal(int id, [FromForm] EmployeePersonalDetailDto model)
        {
            if (id != model.Id) return BadRequest(new { message = "Id mismatch" });

            string? profilePath = null;
            if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                string root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string folder = Path.Combine(root, "Uploads", "EmployeeProfilePictures");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                string fileName = $"{Guid.NewGuid()}_{model.ProfilePicture.FileName}";
                string fullPath = Path.Combine(folder, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await model.ProfilePicture.CopyToAsync(stream);

                profilePath = $"Uploads/EmployeeProfilePictures/{fileName}";
                model.ProfilePicturePath = profilePath;
                model.ProfilePictureName = model.ProfilePicture.FileName;

                // optional: base64 conversion similar to Add
            }

            model.ModifiedBy = model.UserId;
            model.ModifiedAt = DateTime.Now;

            var result = await _employeePersonalService.UpdateAsync(model);
            if (!result) return NotFound(new { message = "Record not found" });
        /// Add a new job history entry
        /// </summary>
        [HttpPost("jobhistory")]
        public async Task<IActionResult> AddJobHistory([FromForm] EmployeeJobHistoryDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string path = Path.Combine(root, "Uploads", "EmployeeJobHistoryDocuments");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // File upload
            if (model.UploadDocument != null && model.UploadDocument.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}_{model.UploadDocument.FileName}";
                string fullPath = Path.Combine(path, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await model.UploadDocument.CopyToAsync(stream);

                model.UploadDocumentPath = $"Uploads/EmployeeJobHistoryDocuments/{fileName}";
            }

            model.CreatedBy = model.UserId;

            var id = await _employeeJobHistoryService.AddAsync(model);

            return Ok(new { message = "Saved successfully", id });
        }

        /// <summary>
        /// Update an existing job history entry
        /// </summary>
        [HttpPut("jobhistory/{id}")]
        public async Task<IActionResult> UpdateJobHistory(int id, [FromForm] EmployeeJobHistoryDto model)
        {
            if (id != model.Id)
                return BadRequest(new { message = "Id mismatch" });

            string root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string path = Path.Combine(root, "Uploads", "EmployeeJobHistoryDocuments");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // File upload logic
            if (model.UploadDocument != null && model.UploadDocument.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}_{model.UploadDocument.FileName}";
                string fullPath = Path.Combine(path, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await model.UploadDocument.CopyToAsync(stream);

                model.UploadDocumentPath = $"Uploads/EmployeeJobHistoryDocuments/{fileName}";
            }

            model.ModifiedBy = model.UserId;

            var result = await _employeeJobHistoryService.UpdateAsync(model);

            if (!result)
                return NotFound(new { message = "Record not found" });

            return Ok(new { message = "Updated successfully" });
        }

        [HttpDelete("personal/{id}")]
        public async Task<IActionResult> DeletePersonal(int id)
        {
            var result = await _employeePersonalService.DeleteAsync(id);
            if (!result) return NotFound(new { message = "Record not found" });

            return Ok(new { message = "Deleted successfully" });
        }
        /// <summary>
        /// Delete a job history record
        /// </summary>
        [HttpDelete("jobhistory/{id}")]
        public async Task<IActionResult> DeleteJobHistory(int id)
        {
            var result = await _employeeJobHistoryService.DeleteAsync(id);

            if (!result)
                return NotFound(new { message = "Record not found" });

            return Ok(new { message = "Deleted successfully" });
        }

        #endregion

    }

}
