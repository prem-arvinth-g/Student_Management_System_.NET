using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.ViewModels
{
    // This is NOT a database table — it's just data we need for the login form
    public class LoginViewModel
    {
        // Which tab is selected: "Student", "Staff", or "Admin"
        [Required]
        public string Role { get; set; } = "Student";

        // --- Student login fields ---
        public string? RegisterNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        // --- Staff / Admin login fields ---
        [EmailAddress]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
