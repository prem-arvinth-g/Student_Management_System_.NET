using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _db;
        public StudentController(ApplicationDbContext db) { _db = db; }

        // Helper: Check if logged in and get role
        private string? GetRole() => HttpContext.Session.GetString("UserRole");
        private int GetDeptId() => HttpContext.Session.GetInt32("UserDeptId") ?? 0;

        // GET: /Student/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var role = GetRole();
            if (role == null) return RedirectToAction("Login", "Account");

            var student = await _db.Students
                .Include(s => s.Department)
                .Include(s => s.Attendances)
                .Include(s => s.Marks)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound();

            // Access control: Staff can only view their own dept students
            if (role == "Staff" && student.DepartmentId != GetDeptId())
                return RedirectToAction("Index", "Dashboard");

            // Student can only view their own profile
            if (role == "Student" && student.Id != HttpContext.Session.GetInt32("UserId"))
                return RedirectToAction("Index", "Dashboard");

            return View(student);
        }

        // GET: /Student/Create
        public async Task<IActionResult> Create()
        {
            var role = GetRole();
            if (role == null) return RedirectToAction("Login", "Account");
            if (role == "Student") return RedirectToAction("Index", "Dashboard");

            ViewBag.Departments = await _db.Departments.ToListAsync();
            return View();
        }

        // POST: /Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            var role = GetRole();
            if (role == null) return RedirectToAction("Login", "Account");
            if (role == "Student") return RedirectToAction("Index", "Dashboard");

            // Staff can only create students in their own department
            if (role == "Staff")
                student.DepartmentId = GetDeptId();

            if (ModelState.IsValid)
            {
                // Check if Register Number already exists
                bool exists = await _db.Students.AnyAsync(s => s.RegisterNumber == student.RegisterNumber);
                if (exists)
                {
                    ModelState.AddModelError("RegisterNumber", "This Register Number is already in use.");
                }
                else
                {
                    _db.Students.Add(student);
                    await _db.SaveChangesAsync();
                    TempData["Success"] = $"Student {student.Name} created successfully!";
                    return RedirectToAction("Index", "Dashboard");
                }
            }

            ViewBag.Departments = await _db.Departments.ToListAsync();
            return View(student);
        }

        // GET: /Student/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var role = GetRole();
            if (role == null) return RedirectToAction("Login", "Account");
            if (role == "Student") return RedirectToAction("Index", "Dashboard");

            var student = await _db.Students.FindAsync(id);
            if (student == null) return NotFound();

            // Staff can only edit their own dept
            if (role == "Staff" && student.DepartmentId != GetDeptId()) return RedirectToAction("Index", "Dashboard");

            ViewBag.Departments = await _db.Departments.ToListAsync();
            return View(student);
        }

        // POST: /Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            var role = GetRole();
            if (role == null) return RedirectToAction("Login", "Account");
            if (role == "Student") return RedirectToAction("Index", "Dashboard");

            var existing = await _db.Students.FindAsync(id);
            if (existing == null) return NotFound();

            // Staff can only edit their own dept
            if (role == "Staff" && existing.DepartmentId != GetDeptId()) return RedirectToAction("Index", "Dashboard");

            if (ModelState.IsValid)
            {
                // Check if Register Number already exists for another student
                bool exists = await _db.Students.AnyAsync(s => s.RegisterNumber == student.RegisterNumber && s.Id != id);
                if (exists)
                {
                    ModelState.AddModelError("RegisterNumber", "This Register Number is already in use.");
                }
                else
                {
                    existing.RegisterNumber = student.RegisterNumber;
                    existing.Name = student.Name;
                    existing.Email = student.Email;
                    existing.Phone = student.Phone;
                    existing.Address = student.Address;
                    existing.Year = student.Year;
                    existing.Section = student.Section;
                    existing.DateOfBirth = student.DateOfBirth;
                    // Admin can change department, Staff cannot
                    if (role == "Admin") existing.DepartmentId = student.DepartmentId;

                    await _db.SaveChangesAsync();
                    TempData["Success"] = "Student updated successfully!";
                    return RedirectToAction("Index", "Dashboard");
                }
            }

            ViewBag.Departments = await _db.Departments.ToListAsync();
            return View(student);
        }

        // POST: /Student/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var role = GetRole();
            if (role == null) return RedirectToAction("Login", "Account");
            if (role == "Student") return RedirectToAction("Index", "Dashboard");

            var student = await _db.Students.FindAsync(id);
            if (student == null) return NotFound();

            // Staff can only delete their own dept
            if (role == "Staff" && student.DepartmentId != GetDeptId()) return RedirectToAction("Index", "Dashboard");

            _db.Students.Remove(student);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Student deleted successfully!";
            return RedirectToAction("Index", "Dashboard");
        }

        // GET: /Student/AddAttendance/5
        public async Task<IActionResult> AddAttendance(int id)
        {
            var role = GetRole();
            if (role == null) return RedirectToAction("Login", "Account");
            if (role == "Student") return RedirectToAction("Index", "Dashboard");

            var student = await _db.Students.FindAsync(id);
            if (student == null) return NotFound();
            if (role == "Staff" && student.DepartmentId != GetDeptId()) return RedirectToAction("Index", "Dashboard");

            ViewBag.Student = student;
            return View(new Attendance { StudentId = id });
        }

        // POST: /Student/AddAttendance
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAttendance(Attendance attendance)
        {
            var role = GetRole();
            if (role == null) return RedirectToAction("Login", "Account");
            if (role == "Student") return RedirectToAction("Index", "Dashboard");

            _db.Attendances.Add(attendance);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Attendance added!";
            return RedirectToAction("Details", new { id = attendance.StudentId });
        }

        // GET: /Student/AddMark/5
        public async Task<IActionResult> AddMark(int id)
        {
            var role = GetRole();
            if (role == null) return RedirectToAction("Login", "Account");
            if (role == "Student") return RedirectToAction("Index", "Dashboard");

            var student = await _db.Students.FindAsync(id);
            if (student == null) return NotFound();
            if (role == "Staff" && student.DepartmentId != GetDeptId()) return RedirectToAction("Index", "Dashboard");

            ViewBag.Student = student;
            return View(new Mark { StudentId = id });
        }

        // POST: /Student/AddMark
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMark(Mark mark)
        {
            var role = GetRole();
            if (role == null) return RedirectToAction("Login", "Account");
            if (role == "Student") return RedirectToAction("Index", "Dashboard");

            _db.Marks.Add(mark);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Mark added!";
            return RedirectToAction("Details", new { id = mark.StudentId });
        }
    }
}
