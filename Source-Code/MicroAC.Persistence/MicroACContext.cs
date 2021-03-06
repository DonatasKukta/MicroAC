using MicroAC.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

#nullable disable

namespace MicroAC.Persistence
{
    public partial class MicroACContext : DbContext
    {
        public MicroACContext()
        {
        }

        public MicroACContext(DbContextOptions<MicroACContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Organisation> Organisations { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RolesPermission> RolesPermissions { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UsersRole> UsersRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Organisation>(entity =>
            {
                entity.HasKey(e => e.Name);

                entity.Property(e => e.Name).HasMaxLength(42);
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(42);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(402);

                entity.Property(e => e.ServiceName)
                    .IsRequired()
                    .HasMaxLength(42);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(42);

                entity.HasOne(d => d.ServiceNameNavigation)
                    .WithMany(p => p.Permissions)
                    .HasForeignKey(d => d.ServiceName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Permissions_Services");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Name);

                entity.Property(e => e.Name).HasMaxLength(42);
            });

            modelBuilder.Entity<RolesPermission>(entity =>
            {
                entity.HasKey(e => new { e.Role, e.Permission });

                entity.ToTable("Roles_Permissions");

                entity.Property(e => e.Role).HasMaxLength(42);

                entity.HasOne(d => d.PermissionNavigation)
                    .WithMany(p => p.RolesPermissions)
                    .HasForeignKey(d => d.Permission)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Roles_Permissions_Permissions");

                entity.HasOne(d => d.RoleNavigation)
                    .WithMany(p => p.RolesPermissions)
                    .HasForeignKey(d => d.Role)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Roles_Permissions_Roles");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.Name);

                entity.Property(e => e.Name).HasMaxLength(42);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Email).HasMaxLength(82);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(42);

                entity.Property(e => e.Organisation).HasMaxLength(42);

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(64)
                    .IsFixedLength(true);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(22);

                entity.Property(e => e.Salt)
                    .HasMaxLength(16)
                    .IsFixedLength(true);

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(42);

                entity.HasOne(d => d.OrganisationNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.Organisation)
                    .HasConstraintName("FK_Users_Organisations");
            });

            modelBuilder.Entity<UsersRole>(entity =>
            {
                entity.HasKey(e => new { e.User, e.Role });

                entity.ToTable("Users_Roles");

                entity.Property(e => e.Role).HasMaxLength(42);

                entity.HasOne(d => d.RoleNavigation)
                    .WithMany(p => p.UsersRoles)
                    .HasForeignKey(d => d.Role)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_Roles_Roles");

                entity.HasOne(d => d.UserNavigation)
                    .WithMany(p => p.UsersRoles)
                    .HasForeignKey(d => d.User)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_Roles_Users");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
