using BusinessLayer.DTOs;
using BusinessLayer.Implementations;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.AspNetCore.Hosting;
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

        private readonly IEmployeeBankDetailsService _bankService;
        private readonly IEmployeeDdlistService _ddlistService;
        private readonly IEmployeeW4Service _w4Service;
        private readonly IWebHostEnvironment _env;
        private readonly IMissedPunchService _missedPunchService;
        private readonly IEmailService _emailService;
        public UserManagementController(ICompanyService companyService, IRegionService regionService, IUserService userService
            , IMenuMasterService menuService, IRoleMasterService roleService, IMenuRoleService menuRoleService, IEmployeeBankDetailsService bankService, IEmployeeDdlistService ddlistService, IEmployeeW4Service w4Service, IWebHostEnvironment env, IMissedPunchService missedPunchService, IEmailService emailService

)
        {
            _companyService = companyService;
            _regionService = regionService;
            _userService = userService;
            _menuService = menuService;
            _roleService = roleService;
            _menuRoleService = menuRoleService;

            _bankService = bankService;
            _ddlistService = ddlistService;
            _w4Service = w4Service;
            _env = env;
            _missedPunchService = missedPunchService;
            _emailService = emailService;
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




        //-----------------------------------Employee-Finance (BANK-DETAILS)--------------------------------------------//
        #region Employee Bank Details

        [HttpGet("GetAllBankDetails")]
        public async Task<IActionResult> GetAllBankDetails()
        {
            var data = await _bankService.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("GetBankDetailsById/{id}")]
        public async Task<IActionResult> GetBankDetailsById(int id)
        {
            var data = await _bankService.GetByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost("CreateBankDetails")]
        public async Task<IActionResult> CreateBankDetails([FromBody] EmployeeBankDetailsDto dto)
        {
            if (dto == null) return BadRequest("Invalid data");
            var success = await _bankService.AddAsync(dto);
            if (!success) return BadRequest("Failed to create bank details");
            return Ok(dto);
        }

        [HttpPut("UpdateBankDetails")]
        public async Task<IActionResult> UpdateBankDetails([FromBody] EmployeeBankDetailsDto dto)
        {
            if (dto == null) return BadRequest("Invalid data");
            var success = await _bankService.UpdateAsync(dto);
            if (!success) return BadRequest("Failed to update bank details");
            return Ok(dto);
        }

        [HttpDelete("DeleteBankDetails/{id}")]
        public async Task<IActionResult> DeleteBankDetails(int id)
        {
            var success = await _bankService.DeleteAsync(id);
            if (!success) return NotFound("Bank details not found");
            return NoContent();
        }
        [HttpGet("GetAccountTypeDropdown")]
        public async Task<IActionResult> GetAccountTypeDropdown()
        {
            var data = await _bankService.GetDropdownAsync();
            return Ok(data);
        }

        #endregion


        //----------------------------------Employee-Finance (DD-LIST)--------------------------------------------//

        #region DD List CRUD

        [HttpGet("GetAllDdlist")]
        public async Task<IActionResult> GetAllDdlist()
        {
            var data = await _ddlistService.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("GetDdlistById/{id}")]
        public async Task<IActionResult> GetDdlistById(int id)
        {
            var data = await _ddlistService.GetByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost("CreateDdlist")]
        public async Task<IActionResult> CreateDdlist([FromBody] EmployeeDdlistDto dto)
        {
            var result = await _ddlistService.AddAsync(dto);
            if (!result) return BadRequest("Failed to create DD List entry.");
            return Ok(dto);
        }

        [HttpPut("UpdateDdlist")]
        public async Task<IActionResult> UpdateDdlist([FromBody] EmployeeDdlistDto dto)
        {
            var result = await _ddlistService.UpdateAsync(dto);
            if (!result) return BadRequest("Failed to update DD List entry.");
            return Ok(dto);
        }

        [HttpDelete("DeleteDdlist/{id}")]
        public async Task<IActionResult> DeleteDdlist(int id)
        {
            var result = await _ddlistService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        #endregion

        #region DD Copy File Upload

        [HttpPost("UploadDDCopy")]
        public async Task<IActionResult> UploadDDCopy(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var uploadsFolder = Path.Combine(_env.WebRootPath!, "DDCopies");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(new
                {
                    FileName = fileName,
                    FileUrl = $"/DDCopies/{fileName}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }





        [HttpGet("DownloadDDCopy/{fileName}")]
        public IActionResult DownloadDDCopy(string fileName)
        {
            var filePath = Path.Combine(_env.WebRootPath!, "DDCopies", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentType = "application/octet-stream";

            return File(fileBytes, contentType, fileName);
        }


        #endregion

        //----------------------------------Employee-Finance (W4(USA))--------------------------------------------//
        #region Employee W4
        [HttpGet("GetAllW4s")]
        public async Task<IActionResult> GetAllW4s()
        {
            var data = await _w4Service.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("GetW4ById/{id}")]
        public async Task<IActionResult> GetW4ById(int id)
        {
            var data = await _w4Service.GetByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost("CreateW4")]
        public async Task<IActionResult> CreateW4([FromBody] EmployeeW4Dto dto)
        {
            var result = await _w4Service.AddAsync(dto);
            if (!result) return BadRequest("Failed to create W4 entry.");
            return Ok(dto);
        }

        [HttpPut("UpdateW4")]
        public async Task<IActionResult> UpdateW4([FromBody] EmployeeW4Dto dto)
        {
            var result = await _w4Service.UpdateAsync(dto);
            if (!result) return BadRequest("Failed to update W4 entry.");
            return Ok(dto);
        }

        [HttpDelete("DeleteW4/{id}")]
        public async Task<IActionResult> DeleteW4(int id)
        {
            var result = await _w4Service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        #endregion



        //---------------------------------Missed Punch Request-------------------------------------------//
        #region Missed Punch Requests

        // GET my requests for logged-in employee
        [HttpGet("GetMyRequests")]
        public async Task<IActionResult> GetMyRequests([FromQuery] int employeeId)
        {
            if (employeeId == 0) return BadRequest("EmployeeId is required.");

            var requests = await _missedPunchService.GetMyRequestsAsync(employeeId);
            return Ok(requests);
        }

        // GET pending requests for manager (current logged-in user)
        [HttpGet("GetPendingRequests")]
        public async Task<IActionResult> GetPendingRequests([FromQuery] int? userId)
        {
            if (userId == null || userId == 0)
                return BadRequest("UserId is required.");

            var user = await _userService.GetUserByIdAsync(userId.Value);

            if (user == null)
                return Ok(new List<MissedPunchRequestDto>());

            var pendingRequests = await _missedPunchService.GetPendingRequestsForManagerAsync(userId.Value);
            return Ok(pendingRequests);
        }

        // Submit a missed punch request
        [HttpPost("SubmitRequest")]
        public async Task<IActionResult> SubmitRequest([FromBody] MissedPunchRequestDto dto)
        {
            if (dto == null) return BadRequest("Invalid request data.");

            try
            {
                var result = await _missedPunchService.SubmitRequestAsync(dto);

                if (dto.ManagerID != 0)
                {
                    await _emailService.SendMissedPunchEmailAsync(result);
                }
                else
                {
                    return BadRequest("No reporting manager assigned. You may be a manager.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Approve/Reject a missed punch request
        [HttpPost("TakeAction")]
        public async Task<IActionResult> TakeAction([FromBody] MissedPunchActionDto dto)
        {
            if (dto == null) return BadRequest("Invalid request data.");

            try
            {
                var result = await _missedPunchService.TakeActionAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Missed Types Dropdown
        [HttpGet("GetMissedTypes")]
        public async Task<IActionResult> GetMissedTypes()
        {
            var types = await _missedPunchService.GetActiveMissedTypesAsync();
            return Ok(types);
        }

        #endregion
    }
}