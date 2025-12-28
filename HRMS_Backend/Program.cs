using BusinessLayer.Implementations;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// 1️⃣ Configure DbContext
// --------------------
builder.Services.AddDbContext<HRMSContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --------------------
// 2️⃣ Configure CORS
// --------------------
var corsPolicyName = "AllowAllOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName,
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// --------------------
// 3️⃣ Register Repositories & UnitOfWork
// --------------------
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGeneralRepository<>), typeof(GenericRepository<>));

// --------------------
// 4️⃣ Register Business Services
// --------------------
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IRegionService, RegionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMenuMasterService, MenuMasterService>();
builder.Services.AddScoped<IRoleMasterService, RoleMasterService>();
builder.Services.AddScoped<IMenuRoleService, MenuRoleService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDesignationService, DesignationService>();
builder.Services.AddScoped<IClockInOutService, ClockInOutService>();
//builder.Services.AddScoped<IBloodGroupService, BloodGroupService>();
builder.Services.AddScoped<IGenderService, GenderService>();

// --------------------
// 5️⃣ Controllers & Swagger
// --------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --------------------
// 6️⃣ Configure Middleware
// --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsPolicyName);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
