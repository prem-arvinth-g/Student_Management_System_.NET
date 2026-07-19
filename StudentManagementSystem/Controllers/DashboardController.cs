using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;

namespace StudentManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DashboardController(ApplicationDbContext db) { _db = db; }

        // GET: /Dashboard/Index
        public async Task<IActionResult> Index(string? dept, string? search)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role == null) return RedirectToAction("Login", "Account");

            if (role == "Student")
            {
                // Student sees only their own info
                var studentId = HttpContext.Session.GetInt32("UserId") ?? 0;
                var student = await _db.Students
                    .Include(s => s.Department)
                    .Include(s => s.Attendances)
                    .Include(s => s.Marks)
                    .FirstOrDefaultAsync(s => s.Id == studentId);

                return View("StudentDashboard", student);
            }
            else if (role == "Staff")
            {
                // Staff sees only their own department's students
                var deptId = HttpContext.Session.GetInt32("UserDeptId") ?? 0;
                var students = await _db.Students
                    .Include(s => s.Department)
                    .Where(s => s.DepartmentId == deptId)
                    .ToListAsync();

                return View("StaffDashboard", students);
            }
            else // Admin
            {
                // Admin sees all students, with optional filter
                var query = _db.Students.Include(s => s.Department).AsQueryable();

                if (!string.IsNullOrEmpty(dept))
                    query = query.Where(s => s.Department!.Name == dept);

                if (!string.IsNullOrEmpty(search))
                    query = query.Where(s => s.Name.Contains(search)
                        || s.RegisterNumber.Contains(search));

                var students = await query.ToListAsync();
                var departments = await _db.Departments.ToListAsync();

                ViewBag.Departments = departments;
                ViewBag.SelectedDept = dept;
                ViewBag.Search = search;

                return View("AdminDashboard", students);
            }
        }
    }
}
