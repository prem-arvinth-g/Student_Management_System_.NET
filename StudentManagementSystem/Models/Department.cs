namespace StudentManagementSystem.Models
{
    // A "class" in C# is like a blueprint for an object.
    // This class represents one Department row in the database.
    public class Department
    {
        public int Id { get; set; }           // Auto-number (1, 2, 3...)
        public string Name { get; set; } = string.Empty; // "ECE", "CSC" etc.

        // Navigation properties — EF uses these to JOIN tables automatically
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<StaffSubject> StaffSubjects { get; set; } = new List<StaffSubject>();
    }
}
