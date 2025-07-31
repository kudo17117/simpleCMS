using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SimpleCMSWeb.Data;
using SimpleCMSWeb.Models;
using SimpleCMSWeb.Models.ModelView;
using SimpleCMSWeb.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SimpleCMSWeb.Controllers
{
    public class AdminAccount : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public AdminAccount(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Edit(int id)
        {
            var emplotype = _context.EmployeeTypes.Find(id);
            if(emplotype == null) 
            {
                return NotFound();
            }
            else
            {
                return View(emplotype);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeType employeeType)
        {
            if (ModelState.IsValid)
            {
                _context.EmployeeTypes.Update(employeeType);
                await _context.SaveChangesAsync();
                return RedirectToAction("Dashboard", "AdminAccount");
            }
            return View(employeeType);
        }

        public Task<IActionResult> Login()
        {
            // Check if the user is already authenticated
            var result = HttpContext.AuthenticateAsync("AdminScheme").Result;
            if (result.Succeeded && result.Principal.Identity != null && result.Principal.Identity.IsAuthenticated)
            {
                return Task.FromResult<IActionResult>(RedirectToAction("Dashboard", "AdminAccount"));
            }

            var result1 = HttpContext.AuthenticateAsync("PatientScheme").Result;
            if (result1.Succeeded && result1.Principal.Identity != null && result1.Principal.Identity.IsAuthenticated)
            {
                return Task.FromResult<IActionResult>(RedirectToAction("Dashboard", "PatientAccount"));
            }

            var result2 = HttpContext.AuthenticateAsync("DoctorScheme").Result;
            if (result2.Succeeded && result2.Principal.Identity != null && result2.Principal.Identity.IsAuthenticated)
            {
                return Task.FromResult<IActionResult>(RedirectToAction("Dashboard", "DoctorAccount"));
            }


            return Task.FromResult<IActionResult>(View());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var us = await _context.Users
                .Include(u => u.Employee)
                    .ThenInclude(e => e.EmployeeType)
                .FirstOrDefaultAsync(u =>
                    u.Username == model.Username &&
                    u.Userpass == model.Userpass &&
                    u.Isactive == true);

            if (us == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            var emTypeDes = us.Employee?.EmployeeType?.EmployeeTypeId;

            if (emTypeDes == null)
            {
                ModelState.AddModelError("", "Missing employee type information.");
                return View(model);
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, us.Username ?? string.Empty),
        new Claim("Employeeid", us.Employeeid.ToString())
    };

            ClaimsIdentity identity;
            string redirectAction;
            string redirectController;
            string scheme;

            switch (emTypeDes)
            {
                case 555:
                    claims.Add(new Claim("Role", "Doctor"));
                    scheme = "DoctorScheme";
                    redirectAction = "Dashboard";
                    redirectController = "DoctorAccount";
                    break;

                case 444:
                    claims.Add(new Claim("Role", "Admin"));
                    scheme = "AdminScheme";
                    redirectAction = "Dashboard";
                    redirectController = "AdminAccount";
                    break;

                default:
                    ModelState.AddModelError("", "Unauthorized account type.");
                    return View(model);
            }

            identity = new ClaimsIdentity(claims, scheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(scheme, principal);
            return RedirectToAction(redirectAction, redirectController);
        }


        [Authorize(AuthenticationSchemes = "AdminScheme")]
        public async Task<IActionResult> Dashboard()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            // Dashboard counts
            ViewBag.ClinicCount = await _context.Clinics
                .Where(c => c.IsActive == true)
                .CountAsync();

            ViewBag.DoctorCount = await _context.Employees
                .Where(d => d.IsActive == true && d.EmployeeTypeId == 555)
                .CountAsync();

            ViewBag.EmployeeCount = await _context.Employees
                .Where(e => e.IsActive == true)
                .CountAsync();

            ViewBag.AppointmentCount = await _context.Appointments
                .CountAsync();

            // Get clinics with their doctors
            ViewBag.ClinicDoctors = await _context.ClinicDoctors
                .Include(cd => cd.Clinic)
                .Include(cd => cd.Employee)
                .Where(cd => cd.Isactive == true)
                .OrderByDescending(cd => cd.CreatedAt)
                .Take(10) // or whatever number you want to display
                .ToListAsync();

            SetNotificationData();
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminScheme");
            return RedirectToAction("Login", "AdminAccount");
        }

        [Authorize(AuthenticationSchemes = "AdminScheme")]
        public async Task<IActionResult> Appointment()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            SetNotificationData();

            var appointmentData = await _context.Appointments
                .Where(a => a.DeletedAt == null)
                .Include(a => a.Patient)
                .Include(a => a.Clinic)
                .Include(a => a.Clinicdoctor)
                    .ThenInclude(cd => cd.Employee)
                    .ToListAsync();

            return View(appointmentData);
        }
            
        // Update Appointment
        [HttpPost]
        public async Task<IActionResult> UpdateAppointment(
            int AppointmentId,
            string PatientFirstname,
            string PatientLastname,
            string ClinicName,
            DateOnly ConsultDate,
            TimeOnly ConsultTime)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Clinic)
                .FirstOrDefaultAsync(a => a.Id == AppointmentId);

            if (appointment == null)
                return NotFound();

            if (appointment.Patient != null)
            {
                appointment.Patient.Firstname = PatientFirstname;
                appointment.Patient.Lastname = PatientLastname;
            }

            if (appointment.Clinic != null)
            {
                appointment.Clinic.Name = ClinicName;
            }

            appointment.Consultdate = ConsultDate;
            appointment.Consulttime = ConsultTime;
            appointment.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult Delete([FromBody] Dictionary<string, string> data)
        {
            if (!data.TryGetValue("ids", out var idsString) || string.IsNullOrWhiteSpace(idsString))
            {
                return BadRequest("No IDs provided.");
            }

            var idList = idsString.Split(',').Select(int.Parse).ToList();

            foreach (var id in idList)
            {
                var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
                if (appointment != null)
                {
                    // Soft delete by setting DeletedAt timestamp
                    appointment.DeletedAt = DateTime.Now;
                }
            }

            _context.SaveChanges();

            return Json(new { success = true, deleted = idList });
        }


        [Authorize(AuthenticationSchemes = "AdminScheme")]
        private void SetNotificationData()
        {
            var list = _context.Employees
                .Where(e => e.IsActive == false)
                .ToList(); 

            ViewData["PendingDoctorCount"] = list.Count;
            ViewData["PendingDoctorList"] = list;
        }


        //Approval by admin of the Doctor Account
        public async Task<IActionResult> PendingDoctors()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            SetNotificationData();

            var pending =_context .Employees
                .Where(e => e.IsActive == false)
                .ToList();

            return View(pending);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveDoctor(long id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp != null)
            {
                emp.IsActive = true;

                var user = _context.Users.FirstOrDefault(u => u.Employeeid == emp.Employeeid);
                if (user != null)
                {
                    user.Isactive = true;
                }

                await _context.SaveChangesAsync();

                try
                {
                    await _emailService.SendEmailAsync(
                        emp.Email,
                        "Account Approved",
                        $"Hi {emp.FirstName}, your doctor account has been approved. You can now log in!"
                    );
                }
                catch (Exception ex)
                {
                    // Log the error (you can use ILogger or just TempData for now)
                    TempData["ErrorMessage"] = $"Doctor approved, but email failed: {ex.Message}";
                    // Optional: Show notification to admin that email failed
                }

                return RedirectToAction("PendingDoctors");
            }

            return NotFound();
        }

        // ================================================================================================================================== //
        //Profile 
        // ================================================================================================================================== //
        [Authorize(AuthenticationSchemes = "AdminScheme")]
        public IActionResult Profile()
        {
            // Add anti-caching headers
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            SetNotificationData();

            var claims = User.Claims.FirstOrDefault(c => c.Type == "Employeeid");
            var employeeId = claims != null ? long.Parse(claims.Value) : 0;


            var doctorInfo = _context.Employees
                .Include(e => e.EmployeeType!)
                .FirstOrDefault(d => d.Employeeid == employeeId);

            return View(doctorInfo);
        }

        [Authorize(AuthenticationSchemes = "AdminScheme")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(Employee model, IFormFile? PhotoFile)
        {
            SetNotificationData();
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
            return RedirectToAction("Profile", "AdminAccount");
        }

        // ================================================================================================================================== //
        //Debug Create Clinic Doctor
        // ================================================================================================================================== //
        [Authorize(AuthenticationSchemes = "AdminScheme")]
        [HttpGet]
        public IActionResult CreateClinic() 
        {   
            SetNotificationData();

            var clinicsexisting = _context.Clinics
                .Include(c => c.Municipality)
                    .ThenInclude(p => p.Province)
                    .ThenInclude(r => r.Region)
                .ToList();

            var model = new AdminCreateClinic
            {
                Regions = _context.Regions.ToList(),
                Provinces = new List<Province>(),
                Municipalities = new List<Municipality>(),

                ClinicsExsting = clinicsexisting,
            };

            return View(model);
        }

        //Clinic logic
        [Authorize(AuthenticationSchemes = "AdminScheme")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClinic(AdminCreateClinic model)
        {
            if (!ModelState.IsValid)
            {
                model.Regions = _context.Regions.ToList();
                model.Provinces = new List<Province>();
                model.Municipalities = new List<Municipality>();
                return View(model);
            }

            // Create the clinic with only MunicipalityId
            var clinic = new Clinic
            {
                Name = model.ClinicName,
                ContactNo = model.ContactNo,
                AdditionAddress = model.AdditionalAddress,
                MunicipalityId = model.SelectedMunicipalityId,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Clinics.Add(clinic);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Clinic created successfully!";
            return RedirectToAction("CreateClinic", "AdminAccount");
        }
        // ================================================================================================================================== //
        //Debug Get Regions, Provinces, and Municipalities
        // ================================================================================================================================== //
        //Get Provinces by Region
        [Authorize(AuthenticationSchemes = "AdminScheme")]
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
        [Authorize(AuthenticationSchemes = "AdminScheme")]
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
        //Action of Create Clinic
        // ================================================================================================================================== //
        //Edit Clinic
        [Authorize(AuthenticationSchemes = "AdminScheme")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> EditClinic(Clinic model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingClinic = await _context.Clinics.FindAsync(model.Id);
            if (existingClinic == null)
            {
                return NotFound();
            }

            existingClinic.Name = model.Name;
            existingClinic.ContactNo = model.ContactNo;
            existingClinic.AdditionAddress = model.AdditionAddress;
            // update other fields if needed

            _context.Update(existingClinic);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Clinic updated successfully!";
            return RedirectToAction("CreateClinic", "AdminAccount"); // Replace with actual action
        }
        //Delete Clinic
        [Authorize(AuthenticationSchemes = "AdminScheme")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClinic(long id)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
            {
                return NotFound();
            }

            _context.Clinics.Remove(clinic);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Clinic deleted successfully!";
            return RedirectToAction("CreateClinic", "AdminAccount"); // Replace with actual action
        }
        // ================================================================================================================================== //
        //Patient Debug
        // ================================================================================================================================== //
        [Authorize(AuthenticationSchemes = "AdminScheme")]
        public async Task<IActionResult> Patient(string searchString, int page = 1)
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            SetNotificationData();
            int pageSize = 10;

            var query = _context.Patients.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p =>
                    p.Firstname.Contains(searchString) ||
                    p.Lastname.Contains(searchString) ||
                    p.Hospitalno.Contains(searchString) ||
                    p.Patientbarcode.Contains(searchString));
            }

            var allPatients = await query
                .Where(p => p.Isactive != false)
                .OrderBy(p => p.Patientid)
                .ToListAsync();

            var totalItems = allPatients.Count;

            var patients = allPatients
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);

            return View(patients);
        }

        [Authorize(AuthenticationSchemes = "AdminScheme")]
        public async Task<IActionResult> Details(long id)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Patientid == id);

            if (patient == null) return NotFound();

            return PartialView("PatientPartialView/_PatientDetailsPartial", patient);
        }
        [Authorize(AuthenticationSchemes = "AdminScheme")]
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Patientid == id);

            if (patient == null) return NotFound();

            return PartialView("PatientPartialView/_EditPatientPartial", patient);
        }


        [Authorize(AuthenticationSchemes = "AdminScheme")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePatient(Patient model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { success = false, message = "Validation failed: " + errors });
            }

            try
            {
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Patientid == model.Patientid);

                if (patient == null)
                    return Json(new { success = false, message = "Patient not found." });

                // Update all fields
                patient.Firstname = model.Firstname;
                patient.Lastname = model.Lastname;
                patient.Middlename = model.Middlename;
                patient.Hospitalno = model.Hospitalno;
                patient.Patientbarcode = model.Patientbarcode;
                patient.Birthdate = model.Birthdate;
                patient.Age = model.Age;
                patient.Gender = model.Gender;
                patient.Birthplace = model.Birthplace;
                patient.Homeaddress = model.Homeaddress;
                patient.Bloodtype = model.Bloodtype;
                patient.Religion = model.Religion;
                patient.Nationality = model.Nationality;
                patient.Occupation = model.Occupation;
                patient.Civilstatus = model.Civilstatus;
                patient.Philhealthno = model.Philhealthno;
                patient.Telephoneno = model.Telephoneno;
                patient.Mobileno = model.Mobileno;
                patient.Emailaddress = model.Emailaddress;
                patient.Allergies = model.Allergies;
                patient.Isactive = model.Isactive;
                patient.Familymedicalhistory = model.Familymedicalhistory;

                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [Authorize(AuthenticationSchemes = "AdminScheme")]
        [HttpPost]
        public async Task<IActionResult> DeletePatient(long patientId)
        {
            try
            {
                var patient = await _context.Patients.FindAsync(patientId);
                if (patient == null)
                {
                    return NotFound();
                }

                patient.Isactive = false;
                _context.Patients.Update(patient);


                await _context.SaveChangesAsync();

                return RedirectToAction("Patient", "AdminAccount");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error deleting patient" });
            }
        }
    }
}


    



