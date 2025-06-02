using AuthApp.domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<ServiceRequest> ServiceRequests { get; set; }
    public DbSet<JobType> JobTypes { get; set; }
    public DbSet<UserServiceRequest> UserServiceRequests { get; set; }
    public DbSet<ServiceRequestEquipment> ServiceRequestEquipments { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<EquipmentAttachment> EquipmentAttachments { get; set; }
    public DbSet<SecurityLevel> SecurityLevels { get; set; }
    public DbSet<InspectionMethod> InspectionMethods { get; set; }
    public DbSet<EquipmentInspectionMethod> EquipmentInspectionMethods { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.LastLoginAt);
            entity.Property(e => e.UpdatedAt);
            entity.Property(e => e.IsAlive).IsRequired();
            
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.HasOne(e => e.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ServiceRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ContractId).IsRequired();
            entity.Property(e => e.RequestNumber).IsRequired();
            entity.Property(e => e.CustomerId).IsRequired();
            entity.Property(e => e.Reasons).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            entity.Property(e => e.PlannedCompletionDate).IsRequired();
            entity.Property(e => e.IsCompleted).IsRequired();
            entity.Property(e => e.CompletedAt);
            entity.Property(e => e.JobTypeId).IsRequired();

            entity.HasIndex(e => e.ContractId).IsUnique();

            entity.HasOne(e => e.JobType)
                .WithMany()
                .HasForeignKey(e => e.JobTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UserServiceRequest>(entity =>
        {
            entity.HasKey(usr => new { usr.UserId, usr.ServiceRequestId });

            entity.Property(usr => usr.AssignedAt).IsRequired();
            entity.Property(usr => usr.IsPrimaryAssignee).IsRequired();

            entity.HasOne(usr => usr.User)
                .WithMany(u => u.UserServiceRequests)
                .HasForeignKey(usr => usr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(usr => usr.ServiceRequest)
                .WithMany(sr => sr.UserServiceRequests)
                .HasForeignKey(usr => usr.ServiceRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ServiceRequestEquipment>(entity =>
        {
            entity.HasKey(sre => new { sre.ServiceRequestId, sre.EquipmentId });

            entity.Property(sre => sre.AddedAt).IsRequired();
            entity.Property(sre => sre.Notes).HasMaxLength(500);

            entity.HasOne(sre => sre.ServiceRequest)
                .WithMany(sr => sr.ServiceRequestEquipments)
                .HasForeignKey(sre => sre.ServiceRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(sre => sre.Equipment)
                .WithMany(e => e.ServiceRequestEquipments)
                .HasForeignKey(sre => sre.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Model).HasMaxLength(200).IsRequired();
            entity.Property(e => e.SerialNumber).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Manufacturer).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Category).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.SZZ).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);

            // Configure relationship with User (Executor)
            entity.HasOne(e => e.Executor)
                .WithMany()
                .HasForeignKey(e => e.ExecutorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure self-referencing relationship for components
            entity.HasOne<Equipment>()
                .WithMany(e => e.Components)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure many-to-many relationship with SecurityLevel
            entity.HasOne(e => e.SecurityLevel)
                .WithMany()
                .HasForeignKey(e => e.SecurityLevelId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure many-to-many relationship with InspectionMethods
            entity.HasMany(e => e.EquipmentInspectionMethods)
                .WithOne(eim => eim.Equipment)
                .HasForeignKey(eim => eim.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many relationship with Attachments
            entity.HasMany(e => e.Attachments)
                .WithOne(a => a.Equipment)
                .HasForeignKey(a => a.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<JobType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
        });
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<EquipmentAttachment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.FilePath).HasMaxLength(500).IsRequired();
            entity.Property(e => e.FileSize).IsRequired();
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.UploadDate).IsRequired();
        });

        modelBuilder.Entity<SecurityLevel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsAlive).IsRequired();
        });

        modelBuilder.Entity<InspectionMethod>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsAlive).IsRequired();
        });

        modelBuilder.Entity<EquipmentInspectionMethod>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(eim => eim.Equipment)
                .WithMany(e => e.EquipmentInspectionMethods)
                .HasForeignKey(eim => eim.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(eim => eim.InspectionMethod)
                .WithMany()
                .HasForeignKey(eim => eim.InspectionMethodId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}