using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.ViewModels;

namespace StudentManagementSystem.Controllers
{
    // AccountController handles Login and Logout
    public class AccountController : Controller
    {
        // _db is our connection to MySQL — injected automatically by ASP.NET
        private readonly ApplicationDbContext _db;

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Account/Login  — Show the login page
        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to dashboard
            if (HttpContext.Session.GetString("UserRole") != null)
                return RedirectToAction("Index", "Dashboard");

            return View(new LoginViewModel());
        }

        // POST: /Account/Login  — Handle form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (model.Role == "Student")
            {
                // Student logs in with Register Number + Date of Birth
                if (string.IsNullOrEmpty(model.RegisterNumber) || model.DateOfBirth == null)
                {
                    ModelState.AddModelError("", "Please enter Register Number and Date of Birth.");
                    return View(model);
                }

                var student = await _db.Students
                    .Include(s => s.Department)
                    .FirstOrDefaultAsync(s => s.RegisterNumber == model.RegisterNumber
                        && s.DateOfBirth.Date == model.DateOfBirth.Value.Date);

                if (student == null)
                {
                    ModelState.AddModelError("", "Invalid Register Number or Date of Birth.");
                    return View(model);
                }

                // Save login info in Session
                HttpContext.Session.SetString("UserRole", "Student");
                HttpContext.Session.SetInt32("UserId", student.Id);
                HttpContext.Session.SetString("UserName", student.Name);
                HttpContext.Session.SetString("UserDept", student.Department?.Name ?? "");
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                // Staff or Admin logs in with Email + Password
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError("", "Please enter Email and Password.");
                    return View(model);
                }

                var user = await _db.Users
                    .Include(u => u.Department)
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Role == model.Role);

                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    ModelState.AddModelError("", "Invalid Email or Password.");
                    return View(model);
                }

                // Save login info in Session
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserDept", user.Department?.Name ?? "");
                HttpContext.Session.SetInt32("UserDeptId", user.DepartmentId ?? 0);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();  // Wipe all session data
            return RedirectToAction("Login");
        }
    }
}
