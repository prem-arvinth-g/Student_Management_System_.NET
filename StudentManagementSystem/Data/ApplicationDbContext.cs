using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Data
{
    // ApplicationDbContext is the "bridge" between your C# code and MySQL.
    //Every DbSet<X> becomes a TABLE in your database.
    public class ApplicationDbContext : DbContext
    {
        // Constructor — just passes options up to the parent class
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Each of these becomes a table in MySQL
        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StaffSubject> StaffSubjects { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Mark> Marks { get; set; }

        // OnModelCreating — configure relationships and seed starter data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Unique constraints ---
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.RegisterNumber)
                .IsUnique();

            // --- Seed Departments ---
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "ECE" },
                new Department { Id = 2, Name = "EEE" },
                new Department { Id = 3, Name = "CSC" },
                new Department { Id = 4, Name = "AIDS" },
                new Department { Id = 5, Name = "MECH" }
            );

            // --- Seed Admin user (password: Admin@123) ---
            // BCrypt.Net hashes the password so it's never stored as plain text
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "System Admin",
                    Email = "admin@sms.com",
                    Phone = "9999999999",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = "Admin",
                    DepartmentId = null
                }
            );
        }
    }
}
