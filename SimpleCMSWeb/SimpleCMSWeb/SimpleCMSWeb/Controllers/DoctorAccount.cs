using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SimpleCMSWeb.Data;
using SimpleCMSWeb.Models;
using SimpleCMSWeb.Models.ModelView;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using X.PagedList;

namespace SimpleCMSWeb.Controllers
{
    public class DoctorAccount : Controller
    {
        // ================================================================================================================================== //
        //DbContext
        // ================================================================================================================================== //
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DoctorAccount(AppDbContext context)
        {
            _context = context;
        }

        // ================================================================================================================================== //
        //login
        // ================================================================================================================================== //
        public async Task<IActionResult> Login()
        {
            var result = await HttpContext.AuthenticateAsync("DoctorScheme");
            if (result.Succeeded && result.Principal.Identity != null && result.Principal.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "DoctorAccount");
            }

            var result1 = await HttpContext.AuthenticateAsync("PatientScheme");
            if (result1.Succeeded && result1.Principal.Identity != null && result1.Principal.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "PatientAccount");
            }

            var result2 = await HttpContext.AuthenticateAsync("AdminScheme");
            if (result2.Succeeded && result2.Principal.Identity != null && result2.Principal.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "AdminAccount");
            }

            return View();
        }

        //Login Logic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Find user by username (don't compare password yet)
            var us = _context.Users.FirstOrDefault(u =>
                u.Username == model.Username && u.Isactive == true);

            if (us != null)
            {
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(us, us.Userpass, model.Userpass);

                if (result == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, us.Username ?? string.Empty),
                new Claim("Role", "Doctor"),
                new Claim("Employeeid", us.Employeeid.ToString())
            };

                    var identity = new ClaimsIdentity(claims, "DoctorScheme");
                    var principal = new ClaimsPrincipal(identity);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    };

                    await HttpContext.SignInAsync("DoctorScheme", principal, authProperties);

                    return RedirectToAction("Dashboard", "DoctorAccount");
                }
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View(model);
        }

        // ================================================================================================================================== //
        //Dashboard
        // ================================================================================================================================== //
        [Authorize(AuthenticationSchemes = "DoctorScheme")]
        public IActionResult Dashboard()
        {
            // Prevent caching
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            // Get EmployeeId from claims
            var doctorIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Employeeid")?.Value;
            if (doctorIdClaim == null)
            {
                return RedirectToAction("Login");
            }

            var doctorId = long.Parse(doctorIdClaim);


            var appointments = _context.Appointments
                 .Include(c => c.Clinic)
                 .Include(p => p.Patient)
                .Where(a => a.Clinicdoctor.Employeeid == doctorId)
                .ToList();

            // Count values
            var totalCount = appointments.Count;
            var pendingCount = appointments.Count(a => a.IsApprovedDoctor == 0 && a.IsDone == 0);
            var completedCount = appointments.Count(a => a.IsDone == 1);

            // Pass to view
            ViewBag.TotalCount = totalCount;
            ViewBag.PendingCount = pendingCount;
            ViewBag.CompletedCount = completedCount;

            return View(appointments);
        }

        // ================================================================================================================================== //
        //Logout
        // ================================================================================================================================== //
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("DoctorScheme");

            return RedirectToAction("Login", "DoctorAccount");
        }

        // ================================================================================================================================== //
        //Register 
        // ================================================================================================================================== //
        [HttpGet]
        public IActionResult Register()
        {

            return View();
        }

        //Register Logic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(DoctorRegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }



            if (model.Userpass != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                return View(model);
            }

            byte[] photoBytes = null;
            if (model.Photo != null && model.Photo.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await model.Photo.CopyToAsync(ms);
                    photoBytes = ms.ToArray();
                }
            }

            var employee = new Employee
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                MiddleName = model.MiddleName,
                Email = model.Email,
                Gender = model.Gender,
                BirthDate = model.BirthDate,
                ContactNo = model.ContactNo,
                EmployeeTypeId = model.EmployeeTypeId,
                SpecializationId = model.SpecializationId,
                LicenceNo = model.PTR,
                PTR = model.PTR,
                Photo = photoBytes,
                IsActive = false,
                Address = model.Address,
                EDateCreated = DateTime.Now
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var passwordHasher = new PasswordHasher<User>();
            var user = new User
            {
                Employeeid = employee.Employeeid,
                Username = model.Username,
                Isactive = false,
                UDatecreated = DateTime.Now
            };
            user.Userpass = passwordHasher.HashPassword(user, model.Userpass);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var notif = new AdminNotification
            {
                Message = $"New doctor registration: {employee.FirstName} {employee.LastName}",
                IsRead = false,
                CreatedAt = DateTime.Now
            };
            _context.AdminNotifications.Add(notif);
            await _context.SaveChangesAsync();

            TempData["ThankYou"] = "true";
            return RedirectToAction("RegisterThankYou", "DoctorAccount");
        }

        //Register Thank You Page
        public IActionResult RegisterThankYou()
        {
            // Add anti-caching headers
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (TempData["ThankYou"] == null)
            {
                return RedirectToAction("Register");
            }
            return View();
        }

        // ================================================================================================================================== //
        //Mark as Approved Appointment and Cancel Appointment
        // ================================================================================================================================== //

        //Mark As Done Appointment
        [Authorize(AuthenticationSchemes = "DoctorScheme")]
        [HttpPost]
        public JsonResult MarkAsDone(long id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment != null)
            {
                appointment.IsDone = 1;
                _context.SaveChanges();

                var totalCount = _context.Appointments.Count();
                var pendingCount = _context.Appointments.Count(a => a.IsApprovedDoctor == 0 && a.IsDone == 0);
                var completedCount = _context.Appointments.Count(a => a.IsDone == 1);

                return Json(new
                {
                    success = true,
                    totalCount,
                    pendingCount,
                    completedCount
                });
            }

            return Json(new { success = false });
        }

        //Cancel Appointment
        [Authorize(AuthenticationSchemes = "DoctorScheme")]
        [HttpPost]
        public JsonResult CancelAppointment(long id)
        {
            try
            {
                var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
                if (appointment == null)
                    return Json(new { success = false, message = "Appointment not found." });

                appointment.IsDone = -1;
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ================================================================================================================================== //
        //Create Clinic
        // ================================================================================================================================== //
        [Authorize(AuthenticationSchemes = "DoctorScheme")]
        [HttpGet]
        public IActionResult CreateClinic()
        {
            var employeeId = long.Parse(User.Claims.FirstOrDefault(c => c.Type == "Employeeid")?.Value ?? "0");

            var clinicdoctors = _context.ClinicDoctors
                .Where(cd => cd.Employeeid == employeeId && cd.Isactive == true)
                .Include(cd => cd.Clinic)
                .ToList();

            var schedules = _context.ClinicSchedules
                .Where(cs => cs.Clinicdoctors.Employeeid == employeeId)
                .Include(cs => cs.Clinicdoctors)
                .ToList();

            var model = new ClinicCreateViewModel
            {
                ClinicDoctorsExsting = clinicdoctors,
                ExistingSchedules = schedules
            };

            return View(model);
        }

        //Pre set-up Clinic logic
        [Authorize(AuthenticationSchemes = "DoctorScheme")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClinic(ClinicCreateViewModel model, string SchedulesJson)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Deserialize the schedule JSON
            var schedules = JsonConvert.DeserializeObject<List<ScheduleViewModel>>(SchedulesJson);

            var claims = User.Claims.FirstOrDefault(c => c.Type == "Employeeid");
            var employeeId = claims != null ? long.Parse(claims.Value) : 0;

            // Insert one ClinicDoctor
            var clinicDoctor = new Clinicdoctor
            {
                Clinicid = model.SelectedClinicId,
                Employeeid = employeeId,
                Isactive = true,
                CreatedAt = DateTime.Now
            };

            _context.ClinicDoctors.Add(clinicDoctor);
            await _context.SaveChangesAsync(); 

            // Add many schedules to that doctor
            foreach (var sched in schedules)
            {
                var scheduleEntity = new ClinicSchedule
                {
                    ClinicDoctorId = clinicDoctor.Clinicdoctorid, 
                    DayOfWeek = sched.DayOfWeek,
                    StartTime = sched.StartTime ?? TimeSpan.Zero,
                    EndTime = sched.EndTime ?? TimeSpan.Zero
                };

                _context.ClinicSchedules.Add(scheduleEntity);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Clinic and schedule created successfully!";
            return RedirectToAction("CreateClinic", "DoctorAccount");
        }
        // ================================================================================================================================== //
        //Get Regions, Provinces, and Municipalities
        // ================================================================================================================================== //

        //Get Provinces by Region
        [Authorize(AuthenticationSchemes = "DoctorScheme")]
        [HttpGet]
        public JsonResult GetProvincesByRegion(int regionId)
        {
            var provinces = _context.Provinces
                .Where(p => p.RegionId == regionId && p.IsActive == true)
                .Select(p => new { p.ProvinceId, p.ProvinceName })
                .ToList();

            return Json(provinces);
        }

        //Get Municipalities by Province
        [Authorize(AuthenticationSchemes = "DoctorScheme")]
        [HttpGet]
        public JsonResult GetMunicipalitiesByProvince(int provinceId)
        {
            var municipalities = _context.Municipalities
                .Where(m => m.ProvinceId == provinceId && m.IsActive == true)
                .Select(m => new { m.MunicipalityId, m.MunicipalityName })
                .ToList();

            return Json(municipalities);
        }
        // ================================================================================================================================== //
        //Profile 
        // ================================================================================================================================== //
        [Authorize(AuthenticationSchemes = "DoctorScheme")]
        public IActionResult Profile()
        {
            // Add anti-caching headers
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            var claims = User.Claims.FirstOrDefault(c => c.Type == "Employeeid");
            var employeeId = claims != null ? long.Parse(claims.Value) : 0;


            var doctorInfo = _context.Employees
                .Include(e => e.Specialization!)
                .Include(a => a.ClinicDoctors!)
                    .ThenInclude(cd => cd.Clinic)
                .FirstOrDefault(d => d.Employeeid == employeeId);

            var clinicName = doctorInfo?.ClinicDoctors?.FirstOrDefault()?.Clinic?.Name;

            ViewBag.ClinicName = clinicName ?? "No clinic assigned";

            return View(doctorInfo);
        }

        [Authorize(AuthenticationSchemes = "DoctorScheme")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(Employee model, IFormFile? PhotoFile)
        {
            // Get the currently logged-in doctor's ID (adjust as needed)
            var claims = User.Claims.FirstOrDefault(c => c.Type == "Employeeid");
            var employeeId = claims != null ? long.Parse(claims.Value) : 0;

            // Find existing record
            var employee = await _context.Employees.FindAsync(employeeId);

            if (employee == null)
            {
                return NotFound();
            }

            // Update only the editable fields
            employee.FirstName = model.FirstName;
            employee.MiddleName = model.MiddleName;
            employee.LastName = model.LastName;
            employee.ExtensioName = model.ExtensioName;
            employee.Gender = model.Gender;
            employee.BirthDate = model.BirthDate;
            employee.PlaceOfBirth = model.PlaceOfBirth;
            employee.CivilStatus = model.CivilStatus;
            employee.Citizenship = model.Citizenship;

            employee.Email = model.Email;
            employee.ContactNo = model.ContactNo;
            employee.Address = model.Address;

            employee.Designation = model.Designation;
            employee.LicenceNo = model.LicenceNo;
            employee.PTR = model.PTR;
            employee.HireDate = model.HireDate;

            employee.College = model.College;
            employee.ColDegreeCourse = model.ColDegreeCourse;
            employee.GraduateStudies = model.GraduateStudies;
            employee.GradDegreeCourse = model.GradDegreeCourse;

            employee.TINNo = model.TINNo;
            employee.S2No = model.S2No;
            employee.PhilHealthNo = model.PhilHealthNo;
            employee.PagIbigIdNo = model.PagIbigIdNo;

            // Handle photo upload if available
            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await PhotoFile.CopyToAsync(ms);
                employee.Photo = ms.ToArray();
            }

            // Optional: update audit trail
            employee.EDateUpdated = DateTime.Now;
            // employee.EUpdatedById = // get current user ID if applicable

            await _context.SaveChangesAsync();

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile", "DoctorAccount");
        }
        // ================================================================================================================================== //
        //Live Search 
        // ================================================================================================================================== //
        [HttpGet]
        public IActionResult SearchClinics(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new List<object>());
            }

            var clinics = _context.Clinics
                .Include(c => c.Municipality)
                    .ThenInclude(m => m.Province)
                        .ThenInclude(p => p.Region)
                .Where(c =>
                    c.Name.Contains(term) ||
                    c.Municipality.MunicipalityName.Contains(term) ||
                    c.Municipality.Province.ProvinceName.Contains(term) ||
                    c.Municipality.Province.Region.Regionname.Contains(term) ||
                    c.AdditionAddress.Contains(term))
                .Select(c => new
                {
                    id = c.Id,
                    name = c.Name,
                    contactNo = c.ContactNo,
                    region = c.Municipality.Province.Region.Regionname,
                    province = c.Municipality.Province.ProvinceName,
                    municipality = c.Municipality.MunicipalityName,
                    additionAddress = c.AdditionAddress
                })
                .Take(10)
                .ToList();

            return Json(clinics);
        }
        // ================================================================================================================================== //
        //Edit Schedule
        // ================================================================================================================================== //
    }
}



