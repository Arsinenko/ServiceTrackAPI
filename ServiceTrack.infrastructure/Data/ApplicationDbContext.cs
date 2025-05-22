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
            entity.Property(e => e.Customer).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            entity.Property(e => e.IsCompleted).IsRequired();
            entity.Property(e => e.CompletedAt);
            entity.Property(e => e.JobTypeId).IsRequired();

            entity.HasIndex(e => e.ContractId).IsUnique();

            entity.HasOne(e => e.JobType)
                .WithMany()
                .HasForeignKey(e => e.JobTypeId)
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
            entity.Property(e => e.Model).HasMaxLength(200);
            entity.Property(e => e.SerialNumber).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Manufacturer).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            entity.Property(e => e.Description).HasMaxLength(500);

            // Configure self-referencing relationship for components
            entity.HasOne<Equipment>()
                .WithMany(e => e.Components)
                .HasForeignKey(e => e.ParentId)
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

    }
}