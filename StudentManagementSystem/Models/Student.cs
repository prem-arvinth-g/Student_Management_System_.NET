namespace StudentManagementSystem.Models
{
    // Each Student row in the database
    public class Student
    {
        public int Id { get; set; }
        public string RegisterNumber { get; set; } = string.Empty;  // Login credential
        public string Name { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }                    // Login credential
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Year { get; set; }                                // 1, 2, 3, 4
        public string Section { get; set; } = string.Empty;         // "A", "B"
        public string? ProfilePhoto { get; set; }

        // Which department this student belongs to
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        // One student has many attendance records and marks
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Mark> Marks { get; set; } = new List<Mark>();
    }
}
