using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    // StaffController handles Staff management (Admin only) + Staff profile (Staff themselves)
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _db;
        public StaffController(ApplicationDbContext db) { _db = db; }

        private string? GetRole() => HttpContext.Session.GetString("UserRole");
        private int GetUserId() => HttpContext.Session.GetInt32("UserId") ?? 0;

        // GET: /Staff/Profile — Staff views their own profile
        public async Task<IActionResult> Profile()
        {
            var role = GetRole();
            if (role == null) return RedirectToAction("Login", "Account");
            if (role == "Student") return RedirectToAction("Index", "Dashboard");

            var userId = GetUserId();
            var staff = await _db.Users
                .Include(u => u.Department)
                .Include(u => u.StaffSubjects)
                    .ThenInclude(ss => ss.Department)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return View(staff);
        }

        // GET: /Staff/Index — Admin sees all staff
        public async Task<IActionResult> Index(string? dept, string? search)
        {
            var role = GetRole();
            if (role == null) return RedirectToAction("Login", "Account");
            if (role != "Admin") return RedirectToAction("Index", "Dashboard");

            var query = _db.Users
                .Include(u => u.Department)
                .Where(u => u.Role == "Staff")
                .AsQueryable();

            if (!string.IsNullOrEmpty(dept))
                query = query.Where(u => u.Department!.Name == dept);
            
            if (!string.IsNullOrEmpty(search))
                query = query.Where(u => u.Name.Contains(search) 
                    || u.Email.Contains(search));

            var staffList = await query.ToListAsync();
            
            ViewBag.Departments = await _db.Departments.ToListAsync();
            ViewBag.SelectedDept = dept;
            ViewBag.Search = search;

            return View(staffList);
        }

        // GET: /Staff/Create — Admin creates new staff
        public async Task<IActionResult> Create()
        {
            if (GetRole() == null) return RedirectToAction("Login", "Account");
            if (GetRole() != "Admin") return RedirectToAction("Index", "Dashboard");
            ViewBag.Departments = await _db.Departments.ToListAsync();
            return View();
        }

        // POST: /Staff/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user, string password)
        {
            if (GetRole() == null) return RedirectToAction("Login", "Account");
            if (GetRole() != "Admin") return RedirectToAction("Index", "Dashboard");

            user.Role = "Staff";
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Staff {user.Name} created successfully!";
            return RedirectToAction("Index");
        }

        // GET: /Staff/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (GetRole() == null) return RedirectToAction("Login", "Account");
            if (GetRole() != "Admin") return RedirectToAction("Index", "Dashboard");

            var staff = await _db.Users.FindAsync(id);
            if (staff == null) return NotFound();

            ViewBag.Departments = await _db.Departments.ToListAsync();
            return View(staff);
        }

        // POST: /Staff/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user, string? newPassword)
        {
            if (GetRole() == null) return RedirectToAction("Login", "Account");
            if (GetRole() != "Admin") return RedirectToAction("Index", "Dashboard");

            var existing = await _db.Users.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Name = user.Name;
            existing.Email = user.Email;
            existing.Phone = user.Phone;
            existing.DepartmentId = user.DepartmentId;

            // Only update password if a new one was provided
            if (!string.IsNullOrEmpty(newPassword))
                existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _db.SaveChangesAsync();
            TempData["Success"] = "Staff updated!";
            return RedirectToAction("Index");
        }

        // POST: /Staff/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (GetRole() == null) return RedirectToAction("Login", "Account");
            if (GetRole() != "Admin") return RedirectToAction("Index", "Dashboard");

            var staff = await _db.Users.FindAsync(id);
            if (staff == null) return NotFound();

            _db.Users.Remove(staff);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Staff deleted!";
            return RedirectToAction("Index");
        }

        // GET: /Staff/AddSubject — Staff or Admin can add subjects to a staff
        public async Task<IActionResult> AddSubject(int staffId)
        {
            if (GetRole() == "Student" || GetRole() == null) return Forbid();
            ViewBag.StaffId = staffId;
            ViewBag.Departments = await _db.Departments.ToListAsync();
            return View(new StaffSubject { StaffId = staffId });
        }

        // POST: /Staff/AddSubject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSubject(StaffSubject subject)
        {
            if (GetRole() == "Student" || GetRole() == null) return Forbid();

            _db.StaffSubjects.Add(subject);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Subject added!";
            return RedirectToAction("Profile");
        }
    }
}
