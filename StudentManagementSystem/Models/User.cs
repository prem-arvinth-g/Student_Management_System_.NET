namespace StudentManagementSystem.Models
{
    // This represents a Staff or Admin user in the system.
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;  // Never store plain passwords!
        public string Role { get; set; } = string.Empty;          // "Staff" or "Admin"
        public string? ProfilePhoto { get; set; }                 // Optional photo file path

        // Foreign key — which department this staff belongs to (null for Admin)
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }               // EF will auto-join this

        // A staff member can teach many subjects
        public ICollection<StaffSubject> StaffSubjects { get; set; } = new List<StaffSubject>();
    }
}
