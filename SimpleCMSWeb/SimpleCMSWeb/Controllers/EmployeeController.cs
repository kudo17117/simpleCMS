using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleCMSWeb.Data;
using SimpleCMSWeb.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleCMSWeb.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(AuthenticationSchemes = "AdminScheme")]
        public async Task<IActionResult> EmployeeView(string searchString, int page = 1)
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            
            var claims = User.Claims.FirstOrDefault(c => c.Type == "Employeeid");
            var currentUser = claims != null ? int.Parse(claims.Value) : 0;

            int pageSize = 10;

            var query = _context.Employees
                .Include(e => e.EmployeeType)
                .Where(e => e.Employeeid != currentUser && e.IsActive == true)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(e =>
                    e.FirstName.Contains(searchString) ||
                    e.LastName.Contains(searchString));
            }

            var allEmployees = await query
                .OrderBy(e => e.Employeeid)
                .ToListAsync();

            var totalItems = allEmployees.Count;

            var employees = allEmployees
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);

            SetNotificationData();
            return View(employees);
        }

        private void SetNotificationData()
        {
            var list = _context.Employees.Where(e => e.IsActive == false).ToList();
            ViewData["PendingDoctorCount"] = list.Count;
            ViewData["PendingDoctorList"] = list;
        }

        public async Task<IActionResult> Details(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.EmployeeType)
                .FirstOrDefaultAsync(e => e.Employeeid == id);

            if (employee == null) return NotFound();

            return PartialView("_EmployeeDetailsPartial", employee);
        }

        [HttpPost]
        public IActionResult DeleteEmployee(long employeeId)
        {
            var employee = _context.Employees.Find(employeeId);
            if (employee == null) return NotFound();

            _context.Employees.Remove(employee);
            _context.SaveChanges();

            return RedirectToAction("EmployeeView");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.EmployeeType)
                .Include(s => s.Specialization)
                .FirstOrDefaultAsync(e => e.Employeeid == id);

            if (employee == null) return NotFound();

            ViewBag.Specializations = await _context.Specializations
                .OrderBy(s => s.Name)
                .ToListAsync();

            return PartialView("_EditEmployeePartial", employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmployee(Employee model)
        {
            if (!ModelState.IsValid)
            {
                // Return validation errors
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = errors
                });
            }

            try
            {
                // Get existing employee from database
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Employeeid == model.Employeeid);

                if (employee == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Employee not found"
                    });
                }

                // Update only the fields that exist in the form
                employee.FirstName = model.FirstName;
                employee.MiddleName = model.MiddleName;
                employee.LastName = model.LastName;
                employee.Gender = model.Gender;
                employee.Email = model.Email;
                employee.ContactNo = model.ContactNo;
                employee.Address = model.Address;
                employee.HireDate = model.HireDate;
                employee.SpecializationId = model.SpecializationId;

                // Explicitly mark as modified (optional but can help in some cases)
                _context.Entry(employee).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Employee updated successfully"
                });
            }
            catch (Exception ex)
            {
               

                return Json(new
                {
                    success = false,
                    message = "An error occurred while updating the employee",
                    error = ex.Message
                });
            }
        }
    }
}
