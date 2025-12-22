using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.DBContext;

public partial class HRMSContext : DbContext
{
    public HRMSContext()
    {
    }

    public HRMSContext(DbContextOptions<HRMSContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountType> AccountTypes { get; set; }

    public virtual DbSet<AssetStatus> AssetStatuses { get; set; }

    public virtual DbSet<AttachmentType> AttachmentTypes { get; set; }

    public virtual DbSet<AttendanceStatus> AttendanceStatuses { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<AuditLogDetail> AuditLogDetails { get; set; }

    public virtual DbSet<BloodGroup> BloodGroups { get; set; }

    public virtual DbSet<CertificationType> CertificationTypes { get; set; }

    public virtual DbSet<CertificationType1> CertificationTypes1 { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Designation> Designations { get; set; }

    public virtual DbSet<DocumentType> DocumentTypes { get; set; }

    public virtual DbSet<EmployeeBankDetail> EmployeeBankDetails { get; set; }

    public virtual DbSet<EmployeeCertification> EmployeeCertifications { get; set; }

    public virtual DbSet<EmployeeDdlist> EmployeeDdlists { get; set; }

    public virtual DbSet<EmployeeEducation> EmployeeEducations { get; set; }

    public virtual DbSet<EmployeeEmergencyContact> EmployeeEmergencyContacts { get; set; }

    public virtual DbSet<EmployeeFamilyDetail> EmployeeFamilyDetails { get; set; }

    public virtual DbSet<EmployeeForm> EmployeeForms { get; set; }

    public virtual DbSet<EmployeeImmigration> EmployeeImmigrations { get; set; }

    public virtual DbSet<EmployeeJobHistory> EmployeeJobHistories { get; set; }

    public virtual DbSet<EmployeeReference> EmployeeReferences { get; set; }

    public virtual DbSet<EmployeeResignation> EmployeeResignations { get; set; }

    public virtual DbSet<EmployeeW4> EmployeeW4s { get; set; }

    public virtual DbSet<ExceptionLog> ExceptionLogs { get; set; }

    public virtual DbSet<ExpenseCategoryType> ExpenseCategoryTypes { get; set; }

    public virtual DbSet<ExpenseStatus> ExpenseStatuses { get; set; }

    public virtual DbSet<FilingStatus> FilingStatuses { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<HelpDeskCategory> HelpDeskCategories { get; set; }

    public virtual DbSet<KpiCategory> KpiCategories { get; set; }

    public virtual DbSet<LeaveStatus> LeaveStatuses { get; set; }

    public virtual DbSet<LeaveType> LeaveTypes { get; set; }

    public virtual DbSet<MaritalStatus> MaritalStatuses { get; set; }

    public virtual DbSet<MenuMaster> MenuMasters { get; set; }

    public virtual DbSet<MenuRoleMaster> MenuRoleMasters { get; set; }

    public virtual DbSet<ModeOfStudy> ModeOfStudies { get; set; }

    public virtual DbSet<PolicyCategory> PolicyCategories { get; set; }

    public virtual DbSet<ProjectStatus> ProjectStatuses { get; set; }

    public virtual DbSet<RaiseTicket> RaiseTickets { get; set; }

    public virtual DbSet<RaiseTicketApproval> RaiseTicketApprovals { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Relationship> Relationships { get; set; }

    public virtual DbSet<RoleMaster> RoleMasters { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VisaTypeMaster> VisaTypeMasters { get; set; }

    public virtual DbSet<WorkAuthStatusMaster> WorkAuthStatusMasters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-I445813;Initial Catalog=HRMSDB;User Id=sa;Password=Password@123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountType>(entity =>
        {
            entity.HasKey(e => e.AccountTypeId).HasName("PK__AccountT__8F95854FFBF8D46D");

            entity.ToTable("AccountType", "adminmaster");

            entity.Property(e => e.AccountTypeId).HasColumnName("AccountTypeID");
            entity.Property(e => e.AccountType1)
                .HasMaxLength(100)
                .HasColumnName("AccountType");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
        });

        modelBuilder.Entity<AssetStatus>(entity =>
        {
            entity.HasKey(e => e.AssetStatusId).HasName("PK__AssetSta__E63EE4F6D42298E4");

            entity.ToTable("AssetStatus", "adminmaster");

            entity.Property(e => e.AssetStatusId).HasColumnName("AssetStatusID");
            entity.Property(e => e.AssetStatusName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.AssetStatuses)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssetStatus_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.AssetStatuses)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssetStatus_Region");
        });

        modelBuilder.Entity<AttachmentType>(entity =>
        {
            entity.HasKey(e => e.AttachmentTypeId).HasName("PK__Attachme__5C63AB44D2657C07");

            entity.ToTable("AttachmentType", "adminmaster");

            entity.Property(e => e.AttachmentTypeId).HasColumnName("AttachmentTypeID");
            entity.Property(e => e.AttachmentTypeName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.AttachmentTypes)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AttachmentType_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.AttachmentTypes)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AttachmentType_Region");
        });

        modelBuilder.Entity<AttendanceStatus>(entity =>
        {
            entity.HasKey(e => e.AttendanceStatusId).HasName("PK__Attendan__7696A7158F7CB77E");

            entity.ToTable("AttendanceStatus", "adminmaster");

            entity.Property(e => e.AttendanceStatusId).HasColumnName("AttendanceStatusID");
            entity.Property(e => e.AttendanceStatusName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.AttendanceStatuses)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AttendanceStatus_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.AttendanceStatuses)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AttendanceStatus_Region");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId).HasName("PK__AuditLog__EB5F6CDD1D84F823");

            entity.ToTable("AuditLog", "Users");

            entity.Property(e => e.AuditLogId).HasColumnName("AuditLogID");
            entity.Property(e => e.ActionType).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Device).HasMaxLength(100);
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.RecordId)
                .HasMaxLength(150)
                .HasColumnName("RecordID");
            entity.Property(e => e.Remarks).HasMaxLength(255);
            entity.Property(e => e.TableName).HasMaxLength(150);
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserName).HasMaxLength(150);
        });

        modelBuilder.Entity<AuditLogDetail>(entity =>
        {
            entity.HasKey(e => e.AuditLogDetailId).HasName("PK__AuditLog__A5C56C58E5501460");

            entity.ToTable("AuditLogDetail", "Users");

            entity.Property(e => e.AuditLogDetailId).HasColumnName("AuditLogDetailID");
            entity.Property(e => e.AuditLogId).HasColumnName("AuditLogID");
            entity.Property(e => e.ColumnName).HasMaxLength(150);

            entity.HasOne(d => d.AuditLog).WithMany(p => p.AuditLogDetails)
                .HasForeignKey(d => d.AuditLogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AuditLogD__Audit__2EA5EC27");
        });

        modelBuilder.Entity<BloodGroup>(entity =>
        {
            entity.HasKey(e => e.BloodGroupId).HasName("PK__BloodGro__4398C6AF248AFEB1");

            entity.ToTable("BloodGroup", "adminmaster");

            entity.Property(e => e.BloodGroupId).HasColumnName("BloodGroupID");
            entity.Property(e => e.BloodGroupName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.BloodGroups)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BloodGroup_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.BloodGroups)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BloodGroup_Region");
        });

        modelBuilder.Entity<CertificationType>(entity =>
        {
            entity.HasKey(e => e.CertificationTypeId).HasName("PK__Certific__D1A096612830D989");

            entity.ToTable("CertificationType", "adminmaster");

            entity.Property(e => e.CertificationTypeId).HasColumnName("CertificationTypeID");
            entity.Property(e => e.CertificationTypeName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.CertificationTypes)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CertificationType_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.CertificationTypes)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CertificationType_Region");
        });

        modelBuilder.Entity<CertificationType1>(entity =>
        {
            entity.HasKey(e => e.CertificationTypeId).HasName("PK__Certific__D1A096414047D821");

            entity.ToTable("CertificationTypes", "adminmaster");

            entity.HasIndex(e => e.CertificationTypeName, "UQ__Certific__A329BCE6A3B46592").IsUnique();

            entity.Property(e => e.CertificationTypeName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CompanyId)
                .HasDefaultValue(1)
                .HasColumnName("CompanyID");
            entity.Property(e => e.CreatedBy).HasDefaultValue(1);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.RegionId)
                .HasDefaultValue(1)
                .HasColumnName("RegionID");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__Company__2D971C4C79CE3C20");

            entity.ToTable("Company", "UM");

            entity.HasIndex(e => e.CompanyCode, "UQ__Company__11A0134BC4D51258").IsUnique();

            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CompanyCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CompanyName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Headquarters)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.IndustryType)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BCD290FD5A4");

            entity.ToTable("Department", "adminmaster");

            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.Departments)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Department_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.Departments)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Department_Region");
        });

        modelBuilder.Entity<Designation>(entity =>
        {
            entity.HasKey(e => e.DesignationId).HasName("PK__Designat__BABD603E27944C14");

            entity.ToTable("Designation", "adminmaster");

            entity.Property(e => e.DesignationId).HasColumnName("DesignationID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.DesignationName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.Designations)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Designation_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.Designations)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Designation_Region");
        });

        modelBuilder.Entity<DocumentType>(entity =>
        {
            entity.ToTable("DocumentType", "adminmaster");

            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<EmployeeBankDetail>(entity =>
        {
            entity.HasKey(e => e.BankDetailsId).HasName("PK__Employee__1759C3A76A6F8551");

            entity.ToTable("EmployeeBankDetails", "employee");

            entity.Property(e => e.BankDetailsId).HasColumnName("BankDetailsID");
            entity.Property(e => e.AccountHolderName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.AccountNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.AccountTypeId).HasColumnName("AccountTypeID");
            entity.Property(e => e.BankName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.BranchName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Ifsccode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IFSCCode");
            entity.Property(e => e.Micrcode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MICRCode");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Upiid)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("UPIID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.AccountType).WithMany(p => p.EmployeeBankDetails)
                .HasForeignKey(d => d.AccountTypeId)
                .HasConstraintName("FK_EmployeeBankDetails_AccountType");
        });

        modelBuilder.Entity<EmployeeCertification>(entity =>
        {
            entity.HasKey(e => e.CertificationId).HasName("PK__Employee__1237E58A5749EA0E");

            entity.ToTable("EmployeeCertifications", "employee");

            entity.Property(e => e.CertificationName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CertificationTypeId).HasDefaultValue(1);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.DocumentPath)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeDdlist>(entity =>
        {
            entity.HasKey(e => e.DdlistId).HasName("PK__Employee__B012948F03AB1F69");

            entity.ToTable("EmployeeDDList", "employee");

            entity.Property(e => e.DdlistId).HasColumnName("DDListID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BankName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.BranchName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DdcopyFilePath)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("DDCopyFilePath");
            entity.Property(e => e.Dddate).HasColumnName("DDDate");
            entity.Property(e => e.Ddnumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DDNumber");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.PayeeName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<EmployeeEducation>(entity =>
        {
            entity.HasKey(e => e.EducationId).HasName("PK__Employee__4BBE3805DF829B96");

            entity.ToTable("EmployeeEducation", "employee");

            entity.Property(e => e.Board)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CertificateFilePath)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Institution)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Qualification)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.Result)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Specialization)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.ModeOfStudy).WithMany(p => p.EmployeeEducations)
                .HasForeignKey(d => d.ModeOfStudyId)
                .HasConstraintName("FK_EmployeeEducation_ModeOfStudyId");
        });

        modelBuilder.Entity<EmployeeEmergencyContact>(entity =>
        {
            entity.HasKey(e => e.EmergencyContactId).HasName("PK__Employee__E8A61DAE5E4FA2B6");

            entity.ToTable("EmployeeEmergencyContact", "employee");

            entity.Property(e => e.EmergencyContactId).HasColumnName("EmergencyContactID");
            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.AlternatePhone).HasMaxLength(20);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.ContactName).HasMaxLength(150);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.Relationship).HasMaxLength(100);
        });

        modelBuilder.Entity<EmployeeFamilyDetail>(entity =>
        {
            entity.HasKey(e => e.FamilyId).HasName("PK__Employee__41D82F6BEA74CFBD");

            entity.ToTable("EmployeeFamilyDetails", "employee");

            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Occupation).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.Relationship).HasMaxLength(50);
        });

        modelBuilder.Entity<EmployeeForm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07F0C31DBB");

            entity.ToTable("EmployeeForms", "employee");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DocumentName).HasMaxLength(100);
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Remarks).HasMaxLength(500);

            entity.HasOne(d => d.DocumentType).WithMany(p => p.EmployeeForms)
                .HasForeignKey(d => d.DocumentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeForms_DocumentType");
        });

        modelBuilder.Entity<EmployeeImmigration>(entity =>
        {
            entity.HasKey(e => e.ImmigrationId).HasName("PK__Employee__A69E9F83B6B1A733");

            entity.ToTable("EmployeeImmigration", "employee");

            entity.Property(e => e.ImmigrationId).HasColumnName("ImmigrationID");
            entity.Property(e => e.ContactPerson).HasMaxLength(150);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployerAddress).HasMaxLength(250);
            entity.Property(e => e.EmployerContact).HasMaxLength(100);
            entity.Property(e => e.EmployerName).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Nationality).HasMaxLength(100);
            entity.Property(e => e.OtherDocumentsPath).HasMaxLength(255);
            entity.Property(e => e.PassportCopyPath).HasMaxLength(255);
            entity.Property(e => e.PassportNumber).HasMaxLength(50);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.VisaCopyPath).HasMaxLength(255);
            entity.Property(e => e.VisaIssuingCountry).HasMaxLength(100);
            entity.Property(e => e.VisaNumber).HasMaxLength(50);
            entity.Property(e => e.VisaTypeId).HasColumnName("VisaTypeID");
        });

        modelBuilder.Entity<EmployeeJobHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07194AB8C3");

            entity.ToTable("EmployeeJobHistory", "employee");

            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Employer)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.JobTitle)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastCtc)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("LastCTC");
            entity.Property(e => e.ReasonForLeaving)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UploadDocument)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Website)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EmployeeReference>(entity =>
        {
            entity.HasKey(e => e.ReferenceId).HasName("PK__Employee__E1A99A7921E1B494");

            entity.ToTable("EmployeeReferences", "employee");

            entity.Property(e => e.ReferenceId).HasColumnName("ReferenceID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CompanyName).HasMaxLength(150);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmailId)
                .HasMaxLength(100)
                .HasColumnName("EmailID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.MobileNumber).HasMaxLength(15);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.TitleOrDesignation).HasMaxLength(100);
        });

        modelBuilder.Entity<EmployeeResignation>(entity =>
        {
            entity.HasKey(e => e.ResignationId).HasName("PK__Employee__CD4E6DB544FA72CC");

            entity.ToTable("EmployeeResignation", "employee");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.NoticePeriod).HasMaxLength(50);
            entity.Property(e => e.ResignationType).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
        });

        modelBuilder.Entity<EmployeeW4>(entity =>
        {
            entity.HasKey(e => e.W4Id).HasName("PK__employee__6B594179FA1727B3");

            entity.ToTable("employee_w4s", "employee");

            entity.HasIndex(e => e.Ssn, "UQ__employee__DDDF0AE6E90C8FDF").IsUnique();

            entity.Property(e => e.W4Id).HasColumnName("w4_id");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("city");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Deductions)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("deductions");
            entity.Property(e => e.DependentAmounts)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("dependent_amounts");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.EmployeeSignature)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("employee_signature");
            entity.Property(e => e.ExtraWithholding)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("extra_withholding");
            entity.Property(e => e.FilingStatus)
                .HasMaxLength(100)
                .HasColumnName("filing_status");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.FormDate).HasColumnName("form_date");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleInitial)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("middle_initial");
            entity.Property(e => e.MultipleJobsOrSpouse)
                .HasDefaultValue(false)
                .HasColumnName("multiple_jobs_or_spouse");
            entity.Property(e => e.OtherIncome)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("other_income");
            entity.Property(e => e.Ssn)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ssn");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("state");
            entity.Property(e => e.TotalDependents)
                .HasDefaultValue(0)
                .HasColumnName("total_dependents");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.ZipCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("zip_code");
        });

        modelBuilder.Entity<ExceptionLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Exceptio__3214EC07C725AFEB");

            entity.ToTable("Exception_Log", "logger");

            entity.Property(e => e.ActionName).HasMaxLength(150);
            entity.Property(e => e.BrowserInfo).HasMaxLength(500);
            entity.Property(e => e.ClientIp)
                .HasMaxLength(50)
                .HasColumnName("ClientIP");
            entity.Property(e => e.ControllerName).HasMaxLength(150);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ErrorCode).HasMaxLength(50);
            entity.Property(e => e.ErrorType).HasMaxLength(100);
            entity.Property(e => e.HostName).HasMaxLength(150);
            entity.Property(e => e.RequestPath).HasMaxLength(500);
            entity.Property(e => e.UserId).HasMaxLength(100);
        });

        modelBuilder.Entity<ExpenseCategoryType>(entity =>
        {
            entity.HasKey(e => e.ExpenseCategoryTypeId).HasName("PK__ExpenseC__CB9B6F4BE0C86BB1");

            entity.ToTable("ExpenseCategoryType", "adminmaster");

            entity.Property(e => e.ExpenseCategoryTypeId).HasColumnName("ExpenseCategoryTypeID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.ExpenseCategoryTypeName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.ExpenseCategoryTypes)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExpenseCategoryType_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.ExpenseCategoryTypes)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExpenseCategoryType_Region");
        });

        modelBuilder.Entity<ExpenseStatus>(entity =>
        {
            entity.HasKey(e => e.ExpenseStatusId).HasName("PK__ExpenseS__A8E82F404FDF3CCE");

            entity.ToTable("ExpenseStatus", "adminmaster");

            entity.Property(e => e.ExpenseStatusId).HasColumnName("ExpenseStatusID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.ExpenseStatusName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.ExpenseStatuses)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExpenseStatus_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.ExpenseStatuses)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExpenseStatus_Region");
        });

        modelBuilder.Entity<FilingStatus>(entity =>
        {
            entity.HasKey(e => e.FilingStatusId).HasName("PK__FilingSt__93F42EE70433A449");

            entity.ToTable("FilingStatus", "adminmaster");

            entity.Property(e => e.FilingStatusId).HasColumnName("FilingStatusID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.StatusName).HasMaxLength(150);
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => e.GenderId).HasName("PK__Gender__4E24E8176628D8B0");

            entity.ToTable("Gender", "adminmaster");

            entity.Property(e => e.GenderId).HasColumnName("GenderID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.GenderName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.Genders)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gender_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.Genders)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gender_Region");
        });

        modelBuilder.Entity<HelpDeskCategory>(entity =>
        {
            entity.HasKey(e => e.HelpDeskCategoryId).HasName("PK__HelpDesk__9F010540142CF9E9");

            entity.ToTable("HelpDeskCategory", "adminmaster");

            entity.Property(e => e.HelpDeskCategoryId).HasColumnName("HelpDeskCategoryID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.HelpDeskCategoryName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.HelpDeskCategories)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HelpDeskCategory_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.HelpDeskCategories)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HelpDeskCategory_Region");
        });

        modelBuilder.Entity<KpiCategory>(entity =>
        {
            entity.HasKey(e => e.KpiCategoryId).HasName("PK__KpiCateg__B31BD9B84C7433EB");

            entity.ToTable("KpiCategory", "adminmaster");

            entity.Property(e => e.KpiCategoryId).HasColumnName("KpiCategoryID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.KpiCategoryName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.KpiCategories)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KpiCategory_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.KpiCategories)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KpiCategory_Region");
        });

        modelBuilder.Entity<LeaveStatus>(entity =>
        {
            entity.HasKey(e => e.LeaveStatusId).HasName("PK__LeaveSta__75EE81DA77CEAA6F");

            entity.ToTable("LeaveStatus", "adminmaster");

            entity.Property(e => e.LeaveStatusId).HasColumnName("LeaveStatusID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LeaveStatusName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.LeaveStatuses)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveStatus_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.LeaveStatuses)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveStatus_Region");
        });

        modelBuilder.Entity<LeaveType>(entity =>
        {
            entity.HasKey(e => e.LeaveTypeId).HasName("PK__LeaveTyp__43BE8FF43D4F1902");

            entity.ToTable("LeaveType", "adminmaster");

            entity.Property(e => e.LeaveTypeId).HasColumnName("LeaveTypeID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LeaveTypeName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.LeaveTypes)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveType_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.LeaveTypes)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveType_Region");
        });

        modelBuilder.Entity<MaritalStatus>(entity =>
        {
            entity.HasKey(e => e.MaritalStatusId).HasName("PK__MaritalS__C8B1BA529B6B0811");

            entity.ToTable("MaritalStatus", "adminmaster");

            entity.Property(e => e.MaritalStatusId).HasColumnName("MaritalStatusID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MaritalStatusName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.MaritalStatuses)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaritalStatus_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.MaritalStatuses)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaritalStatus_Region");
        });

        modelBuilder.Entity<MenuMaster>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__MenuMast__C99ED25090C1D2FF");

            entity.ToTable("MenuMaster", "UM");

            entity.Property(e => e.MenuId).HasColumnName("MenuID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Icon).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MenuName).HasMaxLength(100);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ParentMenuId).HasColumnName("ParentMenuID");
            entity.Property(e => e.Url).HasMaxLength(255);
        });

        modelBuilder.Entity<MenuRoleMaster>(entity =>
        {
            entity.HasKey(e => e.MenuRoleId).HasName("PK__MenuRole__880F2CC158F7D715");

            entity.ToTable("MenuRoleMaster", "UM");

            entity.HasIndex(e => new { e.RoleId, e.MenuId }, "UQ_MenuRole").IsUnique();

            entity.Property(e => e.MenuRoleId).HasColumnName("MenuRoleID");
            entity.Property(e => e.CanAdd).HasDefaultValue(false);
            entity.Property(e => e.CanApprove).HasDefaultValue(false);
            entity.Property(e => e.CanDelete).HasDefaultValue(false);
            entity.Property(e => e.CanEdit).HasDefaultValue(false);
            entity.Property(e => e.CanView).HasDefaultValue(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MenuId).HasColumnName("MenuID");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Menu).WithMany(p => p.MenuRoleMasters)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MenuRoleMaster_Menu");

            entity.HasOne(d => d.Role).WithMany(p => p.MenuRoleMasters)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MenuRoleMaster_Role");
        });

        modelBuilder.Entity<ModeOfStudy>(entity =>
        {
            entity.ToTable("ModeOfStudy", "adminmaster");

            entity.HasIndex(e => e.ModeName, "UQ_ModeName").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModeName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<PolicyCategory>(entity =>
        {
            entity.HasKey(e => e.PolicyCategoryId).HasName("PK__PolicyCa__C0F36D7D5A496BDF");

            entity.ToTable("PolicyCategory", "adminmaster");

            entity.Property(e => e.PolicyCategoryId).HasColumnName("PolicyCategoryID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.PolicyCategoryName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.PolicyCategories)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PolicyCategory_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.PolicyCategories)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PolicyCategory_Region");
        });

        modelBuilder.Entity<ProjectStatus>(entity =>
        {
            entity.HasKey(e => e.ProjectStatusId).HasName("PK__ProjectS__F3B67D2D40D59090");

            entity.ToTable("ProjectStatus", "adminmaster");

            entity.Property(e => e.ProjectStatusId).HasColumnName("ProjectStatusID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ProjectStatusName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.ProjectStatuses)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectStatus_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.ProjectStatuses)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectStatus_Region");
        });

        modelBuilder.Entity<RaiseTicket>(entity =>
        {
            entity.HasKey(e => e.RaiseTicketId).HasName("PK__RaiseTicket__1788CCAC45456CCE");

            entity.ToTable("RaiseTicket", "UM");

            entity.Property(e => e.RaiseTicketId).HasColumnName("RaiseTicketID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Open");
            entity.Property(e => e.SubjectIssue)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UploadPicPath)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.HasOne<RaiseTicketApproval>()
        .WithOne()
        .HasForeignKey<RaiseTicketApproval>(a => a.RaiseTicketId)
        .OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<RaiseTicketApproval>(entity =>
        {
            entity.HasKey(e => e.RaiseTicketApprovalId).HasName("PK__RaiseTicketApproval__1788CCAC45456CCE");

            entity.ToTable("RaiseTicketApproval", "UM");
            entity.HasOne<RaiseTicket>()
             .WithOne()
             .HasForeignKey<RaiseTicketApproval>(a => a.RaiseTicketId)
             .OnDelete(DeleteBehavior.Cascade);


            entity.Property(e => e.RaiseTicketApprovalId).HasColumnName("RaiseTicketApprovalID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ManagerComments)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ManagerId).HasColumnName("ManagerID");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RaiseTicketId).HasColumnName("RaiseTicketID");
            entity.Property(e => e.RaisedOn).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RegionId).HasName("PK__Region__ACD84443212B21B7");

            entity.ToTable("Region", "UM");

            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Company).WithMany(p => p.Regions)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Region__CompanyI__2BC97F7C");
        });

        modelBuilder.Entity<Relationship>(entity =>
        {
            entity.HasKey(e => e.RelationshipId).HasName("PK__Relation__31FEB861F1AE473A");

            entity.ToTable("Relationship", "adminmaster");

            entity.Property(e => e.RelationshipId).HasColumnName("RelationshipID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.RelationshipName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Company).WithMany(p => p.Relationships)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Relationship_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.Relationships)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Relationship_Region");
        });

        modelBuilder.Entity<RoleMaster>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__RoleMast__8AFACE3A396228F9");

            entity.ToTable("RoleMaster", "UM");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.RoleDescription).HasMaxLength(255);
            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.StateId).HasName("PK__States__C3BA3B5AD3E85F66");

            entity.ToTable("States", "adminmaster");

            entity.Property(e => e.StateId).HasColumnName("StateID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.StateName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC45456CCE");

            entity.ToTable("Users", "UM");

            entity.HasIndex(e => e.EmployeeCode, "UQ__Users__1F6425485EF5F0DF").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105344FC457C0").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(120)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RefreshTokenExpiry).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.RoleId).HasDefaultValueSql("('Employee')");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Company).WithMany(p => p.Users)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__CompanyID__571DF1D5");

            entity.HasOne(d => d.Region).WithMany(p => p.Users)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RegionID__5812160E");
        });

        modelBuilder.Entity<VisaTypeMaster>(entity =>
        {
            entity.HasKey(e => e.VisaTypeId).HasName("PK__VisaType__9522E6791944D799");

            entity.ToTable("VisaTypeMaster", "adminmaster");

            entity.Property(e => e.VisaTypeId).HasColumnName("VisaTypeID");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.VisaTypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<WorkAuthStatusMaster>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__WorkAuth__C8EE2043FF212E32");

            entity.ToTable("WorkAuthStatusMaster", "adminmaster");

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
