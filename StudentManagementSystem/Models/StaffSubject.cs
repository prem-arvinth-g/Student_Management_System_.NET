namespace StudentManagementSystem.Models
{
    // Which subjects a staff member teaches — and for which class
    public class StaffSubject
    {
        public int Id { get; set; }
        public string SubjectName { get; set; } = string.Empty;   // "Digital Electronics"
        public string SubjectCode { get; set; } = string.Empty;   // "EC301"
        public int Year { get; set; }                             // 2nd year students
        public string Section { get; set; } = string.Empty;       // "A"

        // Which staff teaches this
        public int StaffId { get; set; }
        public User? Staff { get; set; }

        // In which department
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        // Attendance records linked to this subject
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
