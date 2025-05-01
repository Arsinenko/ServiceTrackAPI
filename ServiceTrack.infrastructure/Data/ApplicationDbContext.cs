using AuthApp.domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<EquipmentComponent> EquipmentComponents { get; set; }

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
            
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.HasOne(e => e.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
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
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            entity.HasMany(e => e.Components)
                .WithOne(c => c.Equipment)
                .HasForeignKey(c => c.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EquipmentComponent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Model).HasMaxLength(200);
            entity.Property(e => e.SerialNumber).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Manufacturer).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });
    }
}