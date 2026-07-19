namespace StudentManagementSystem.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public int TotalClasses { get; set; }
        public int AttendedClasses { get; set; }

        // Which student this attendance belongs to
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        // Which staff recorded this (optional link)
        public int? StaffSubjectId { get; set; }
        public StaffSubject? StaffSubject { get; set; }

        // Computed property — calculates percentage automatically
        // No database column needed for this — just calculated on the fly
        public double Percentage => TotalClasses > 0
            ? Math.Round((double)AttendedClasses / TotalClasses * 100, 2)
            : 0;
    }
}
