using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleCMSWeb.Data;
using SimpleCMSWeb.Models;
using SimpleCMSWeb.Models.ModelView;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleCMSWeb.Controllers
{
    public class PatientAccountController : Controller
    {
        private readonly AppDbContext _context;

        public PatientAccountController(AppDbContext context)
        {
            _context = context;
        }

        #region Authentication

        public async Task<IActionResult> Login()
        {
            var result = await HttpContext.AuthenticateAsync("PatientScheme");

            if (result.Succeeded && result.Principal?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Dashboard", "PatientAccount");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Patientuser model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.PatientUsers
                .FirstOrDefaultAsync(u => u.Username == model.Username && u.Isactive);

            if (user != null)
            {
                var passwordHasher = new PasswordHasher<Patientuser>();
                var result = passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim("Role", "Patient"),
                new Claim("PatientUserid", user.Patientuserid.ToString())
            };

                    var identity = new ClaimsIdentity(claims, "PatientScheme");
                    var principal = new ClaimsPrincipal(identity);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    };

                    await HttpContext.SignInAsync("PatientScheme", principal, authProperties);

                    // ✅ Check for appointmentData cookie and redirect to appointment page if present
                    if (Request.Cookies.TryGetValue("appointmentData", out var appointmentCookie))
                    {
                        try
                        {
                            // ✅ Decode and deserialize cookie JSON
                            var decodedJson = System.Net.WebUtility.UrlDecode(appointmentCookie);
                            var appointmentData = JsonSerializer.Deserialize<Dictionary<string, string>>(decodedJson);

                            if (appointmentData != null &&
                                appointmentData.TryGetValue("clinicId", out var clinicId) &&
                                appointmentData.TryGetValue("clinicDoctorId", out var clinicDoctorId))
                            {
                                // ✅ Create appointment and save to session
                                var appointment = new Appointment
                                {
                                    Clinicid = long.Parse(clinicId),
                                    Clinicdoctorid = long.Parse(clinicDoctorId),
                                    Patientid = user.Patientid
                                };

                                HttpContext.Session.SetString("appointmentData", JsonSerializer.Serialize(appointment));

                                // ✅ Remove cookie after use
                                Response.Cookies.Delete("appointmentData", new CookieOptions { Path = "/" });

                                // ✅ Redirect to appointment creation
                                return RedirectToAction("Appointment");
                            }
                        }
                        catch (Exception ex)
                        {
                            // ❗ Optional: log this in your logger instead
                            Console.WriteLine("Failed to process appointmentData cookie: " + ex);
                            // Don't redirect if cookie was bad — fall through to dashboard
                        }
                    }

                    return RedirectToAction("Dashboard");
                }
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("PatientScheme");
            HttpContext.Session.Clear();
            // ✅ Ensure cookie is fully deleted from root
            Response.Cookies.Delete("appointmentData", new CookieOptions
            {
                Path = "/"
            });

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> IsPatientLoggedIn()
        {
            var result = await HttpContext.AuthenticateAsync("PatientScheme");
            bool isLoggedIn = result.Succeeded && result.Principal?.Identity?.IsAuthenticated == true;
            return Json(new { isLoggedIn });
        }

        #endregion

        #region Registration

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var municipalities = await _context.Municipalities
                .Where(m => (bool)m.IsActive)
                .OrderBy(m => m.MunicipalityName)
                .ToListAsync();

            var model = new RegistrationFormVM
            {
                Municipalities = municipalities
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationFormVM model)
        {
            // Custom validation
            if (model.Municipalityid == null || model.Municipalityid == 0)
            {
                ModelState.AddModelError(nameof(model.Municipalityid), "Please select a municipality.");
            }

            if (string.IsNullOrWhiteSpace(model.Username))
            {
                ModelState.AddModelError(nameof(model.Username), "Username is required.");
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(nameof(model.Password), "Password is required.");
            }

            if (await _context.PatientUsers.AnyAsync(u => u.Username == model.Username))
            {
                ModelState.AddModelError(nameof(model.Username), "Username is already taken.");
            }

            // If validation fails
            if (!ModelState.IsValid)
            {
                model.Municipalities = await _context.Municipalities
                    .Where(m => (bool)m.IsActive)
                    .OrderBy(m => m.MunicipalityName)
                    .ToListAsync();
                return View(model);
            }

            // Create Patient
            var patient = new Patient
            {
                Lastname = model.Lastname,
                Firstname = model.Firstname,
                Middlename = model.Middlename,
                Suffixname = string.IsNullOrWhiteSpace(model.Suffixname) ? null : model.Suffixname,
                Birthdate = model.Birthdate,
                Age = model.Birthdate.HasValue ? (int)((DateTime.UtcNow - model.Birthdate.Value).TotalDays / 365.25) : (int?)null,
                Gender = model.Gender,
                Civilstatus = model.Civilstatus,
                Mobileno = model.Mobileno,
                Homeaddress = model.Homeaddress,
                Municipalityid = model.Municipalityid.Value,
                Philhealthno = string.IsNullOrWhiteSpace(model.Philhealthno) ? null : model.Philhealthno,
                Isphilhealthmember = model.Isphilhealthmember ?? false,
                Occupation = model.Occupation,
                Company = model.Company
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            // Create User Account
            var patientUser = new Patientuser
            {
                Username = model.Username,
                Isactive = true,
                Dateonlineaccountactivated = DateTime.UtcNow,
                Patientid = patient.Patientid
            };

            var hasher = new PasswordHasher<Patientuser>();
            patientUser.Password = hasher.HashPassword(patientUser, model.Password);

            _context.PatientUsers.Add(patientUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login", "PatientAccount");
        }


        #endregion

        #region Dashboard

        [Authorize(AuthenticationSchemes = "PatientScheme")]
        public async Task<IActionResult> Dashboard()
        {
            var patientUserIdClaim = User.FindFirst("PatientUserid")?.Value;

            if (!long.TryParse(patientUserIdClaim, out var patientUserId))
            {
                return RedirectToAction("Login");
            }

            var patientUser = await _context.PatientUsers
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(p => p.Patientuserid == patientUserId);

            if (patientUser?.Patient == null)
            {
                ViewBag.PatientName = "Unknown Patient";
                return View(new List<AppointmentViewModel>());
            }

            // Format patient name
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            string fullName = textInfo.ToTitleCase($"{patientUser.Patient.Firstname} {patientUser.Patient.Middlename} {patientUser.Patient.Lastname}".ToLower());
            ViewBag.PatientName = fullName;

            // Get all appointments with doctor and clinic information
            var appointments = await _context.Appointments
                .Where(a => a.Patientid == patientUser.Patientid)
                .Select(a => new AppointmentViewModel
                {
                    Appointment = a,
                    DoctorName = _context.ClinicDoctors
                        .Where(cd => cd.Clinicdoctorid == a.Clinicdoctorid)
                        .Join(_context.Employees,
                            cd => cd.Employeeid,
                            e => e.Employeeid,
                            (cd, e) => e.LastName + ", " + e.FirstName)
                        .FirstOrDefault() ?? "Unknown Doctor",
                    ClinicName = _context.ClinicDoctors
                        .Where(cd => cd.Clinicdoctorid == a.Clinicdoctorid)
                        .Join(_context.Clinics,
                            cd => cd.Clinicid,
                            c => c.Id,
                            (cd, c) => c.Name)
                        .FirstOrDefault() ?? "Unknown Clinic"
                })
                .ToListAsync();

            // Order appointments by status (pending first, then success, then cancelled)
            var orderedAppointments = appointments
                .OrderBy(a => a.Appointment.IsDone switch
                {
                    0 => 0,    // Pending comes first
                    1 => 1,    // Success comes second
                    -1 => 2,   // Cancelled comes last
                    _ => 3     // Any other status goes to the end
                })
                .ThenByDescending(a => a.Appointment.Consultdate)
                .ThenByDescending(a => a.Appointment.Consulttime)
                .ToList();

            return View(orderedAppointments);
        }





        #endregion

        #region Municipality

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var municipalities = await _context.Municipalities
                .Where(m => (bool)m.IsActive)
                .OrderBy(m => m.MunicipalityName)
                .ToListAsync();

            return View(municipalities);
        }

        [HttpPost]
        public IActionResult SelectMunicipality(int municipalityId)
        {
            ViewBag.SelectedMunicipalityId = municipalityId;
            return Content($"Selected Municipality ID: {municipalityId}");
        }

        #endregion

        #region Username Validation

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsUsernameAvailable(string username)
        {
            var exists = await _context.PatientUsers.AnyAsync(u => u.Username == username);
            return Json(!exists);
        }

        [HttpGet]
        public async Task<IActionResult> IsUsernameTaken(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return Json(false);

            bool exists = await _context.PatientUsers.AnyAsync(u => u.Username == username);
            return Json(!exists);
        }

        #endregion

        #region Forgot Password

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            var model = new ForgotPasswordVM
            {
                Username = string.Empty,
                NewPassword = string.Empty,
                ConfirmPassword = string.Empty,
                IsUsernameValid = false
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (string.IsNullOrWhiteSpace(model.Username))
            {
                ModelState.AddModelError("Username", "Username is required.");
                return View(model);
            }

            var matchedUser = await _context.PatientUsers.FirstOrDefaultAsync(u => u.Username == model.Username);

            if (matchedUser == null)
            {
                ModelState.AddModelError("Username", "Username not found.");
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.NewPassword) && !string.IsNullOrEmpty(model.ConfirmPassword))
            {
                if (model.NewPassword != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                    return View(model);
                }
                var hasher = new PasswordHasher<Patientuser>();

                matchedUser.Password = hasher.HashPassword(matchedUser, model.NewPassword);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Password has been reset successfully.";
                return RedirectToAction("Login");
            }

            model.IsUsernameValid = true;
            return View(model);
        }

        #endregion

        [Authorize(AuthenticationSchemes = "PatientScheme")]
        public async Task<IActionResult> Appointment()
        {
            var model = new Appointment();

            var patientUserIdClaim = User.FindFirst("PatientUserid")?.Value;

            if (long.TryParse(patientUserIdClaim, out var patientUserId))
            {
                var patientUser = await _context.PatientUsers
                    .Include(pu => pu.Patient)
                    .FirstOrDefaultAsync(pu => pu.Patientuserid == patientUserId);

                if (patientUser?.Patient != null)
                {
                    model.Patientid = patientUser.Patientid;
                    ViewBag.PatientName = $"{patientUser.Patient.Lastname}, {patientUser.Patient.Firstname}";
                }
                else
                {
                    ViewBag.PatientName = "Unknown Patient";
                }
            }

            // Load appointment data from session
            var appointmentJson = HttpContext.Session.GetString("appointmentData");
            if (!string.IsNullOrEmpty(appointmentJson))
            {
                try
                {
                    var appointmentData = JsonSerializer.Deserialize<Appointment>(appointmentJson);
                    if (appointmentData != null)
                    {
                        model.Clinicid = appointmentData.Clinicid;
                        model.Clinicdoctorid = appointmentData.Clinicdoctorid;

                        // 👇 Get doctor name via foreign key
                        var employee = await _context.ClinicDoctors
                            .Where(cd => cd.Clinicdoctorid == model.Clinicdoctorid)
                            .Join(_context.Employees,
                                cd => cd.Employeeid,
                                e => e.Employeeid,
                                (cd, e) => new { e.FirstName, e.LastName })
                            .FirstOrDefaultAsync();

                        if (employee != null)
                        {
                            ViewBag.DoctorName = $"{employee.LastName}, {employee.FirstName}";
                        }
                        else
                        {
                            ViewBag.DoctorName = null;
                        }
                    }
                }
                catch
                {
                    ViewBag.DoctorName = "Invalid Doctor";
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorSchedule(long clinicDoctorId)
        {
            var schedules = await _context.ClinicSchedules
                .Where(cs => cs.ClinicDoctorId == clinicDoctorId)
                .Select(cs => cs.DayOfWeek)
                .Distinct()
                .ToListAsync();

            return Json(schedules);
        }



        [HttpPost]
        [Authorize(AuthenticationSchemes = "PatientScheme")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment model)
        {
            if (!ModelState.IsValid)
                return View("Appointment", model);

            // Get Patient ID from claims
            var patientUserIdClaim = User.Claims.FirstOrDefault(c => c.Type == "PatientUserid")?.Value;

            if (long.TryParse(patientUserIdClaim, out var patientUserId))
            {
                var patient = await _context.PatientUsers
                    .FirstOrDefaultAsync(p => p.Patientuserid == patientUserId);

                if (patient != null)
                {
                    model.Patientid = patient.Patientid;
                }
            }

            // ✅ Set backend fields
            model.CreatedAt = DateTime.UtcNow;
            model.Issync = true;

            // Random long value (use this if .NET 6+)
            model.ServerId = Random.Shared.NextInt64();

            // If .NET 5 or lower, use:
            // model.ServerId = (long)(new Random().Next(1, int.MaxValue));

            // ✅ Sequence number logic
            //bool alreadyBooked = await _context.Appointments.AnyAsync(a =>
            //    a.Patientid == model.Patientid &&
            //    a.Consultdate == model.Consultdate &&
            //    a.Consulttime == model.Consulttime);

            //if (alreadyBooked)
            //{
            //    ModelState.AddModelError("", "You already have an appointment at this time.");
            //    return View("Appointment", model);
            //}

            //if (model.Consultdate.HasValue && model.Consulttime.HasValue)
            //{
            //    var existingCount = await _context.Appointments.CountAsync(a =>
            //        a.Consultdate == model.Consultdate &&
            //        a.Consulttime == model.Consulttime);

            //    model.Sequenceno = existingCount + 1;
            //}

            _context.Appointments.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Appointment successfully submitted!";
            HttpContext.Session.Remove("appointmentData");

            return RedirectToAction("Dashboard");
        }


        [HttpGet]
        public async Task<IActionResult> GetFullyBookedDates(long clinicDoctorId)
        {
            var maxAppointmentsPerDay = 10;

            var fullyBookedDates = await _context.Appointments
                .Where(a => a.Clinicdoctorid == clinicDoctorId)
                .GroupBy(a => a.Consultdate)
                .Where(g => g.Count() >= maxAppointmentsPerDay)
                .Select(g => g.Key)
                .ToListAsync();

            return Json(fullyBookedDates.Select(d => d?.ToString("yyyy-MM-dd")));
        }




        #region Profile

        [Authorize(AuthenticationSchemes = "PatientScheme")]
        public async Task<IActionResult> Profile()
        {
            var patientUserIdClaim = User.FindFirst("PatientUserid")?.Value;

            if (long.TryParse(patientUserIdClaim, out var patientUserId))
            {
                var patientUser = await _context.PatientUsers
                    .Include(pu => pu.Patient)
                    .FirstOrDefaultAsync(pu => pu.Patientuserid == patientUserId);

                if (patientUser?.Patient != null)
                if (patientUser?.Patient != null)
                {
                    var textInfo = CultureInfo.CurrentCulture.TextInfo;

                    string firstName = textInfo.ToTitleCase(patientUser.Patient.Firstname?.ToLower() ?? "");
                    string middleName = textInfo.ToTitleCase(patientUser.Patient.Middlename?.ToLower() ?? "");
                    string lastName = textInfo.ToTitleCase(patientUser.Patient.Lastname?.ToLower() ?? "");

                    var fullName = $"{firstName} {middleName+"."} {lastName}".Trim();
                    ViewBag.PatientName = fullName;
                }
                else
                {
                    ViewBag.PatientName = "Unknown Patient";
                }
            }
            else
            {
                ViewBag.PatientName = "Unauthenticated";
            }

            return View();
        }

        //public async Task<IActionResult> IsAppointmentSlotAvailable(DateTime consultDate, TimeSpan consultTime)
        //{
        //    var isTaken = await _context.Appointments
        //        .AnyAsync(a => a.Consultdate == cons && a.Consulttime == consultTime);

        //    return Json(new { available = !isTaken });
        //}


        #endregion

        [HttpPost]
        [Authorize(AuthenticationSchemes = "PatientScheme")]
        public IActionResult ClearAppointmentSession()
        {
            HttpContext.Session.Remove("appointmentData");
            return Json(new { success = true });
        }
    }
}