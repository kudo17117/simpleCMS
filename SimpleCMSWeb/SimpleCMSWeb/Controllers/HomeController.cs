using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleCMSWeb.Data;
using SimpleCMSWeb.Models;
using SimpleCMSWeb.Models.ModelView;
using System.Diagnostics;

namespace SimpleCMSWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // get the count of active doctors and clinics
        public async Task<IActionResult> Index()
        {
            //HttpContext.Session.Remove("AppointmentData");    
            //Response.Cookies.Delete("AppointmentData");

            var model = new StatsViewModel
            {
                DoctorCount = await _context.ClinicDoctors
                    .Where(d => d.Isactive == true)    // Check if property is spelled "IsActive" (PascalCase)

                    .CountAsync(),

                ClinicCount = await _context.Clinics
                    .Where(c => c.IsActive == true)    // Changed `d` to `c` for Clinics

                    .CountAsync(),
            };

            return View(model);
        }


        // direct to choices page
        public IActionResult Choices()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ClinicDoctorsDetail(int clinicId, int municipalityId)
        {
            var doctorSchedules = (
                from cd in _context.ClinicDoctors
                join e in _context.Employees on cd.Employeeid equals e.Employeeid
                join cs in _context.ClinicSchedules on cd.Clinicdoctorid equals cs.ClinicDoctorId
                join c in _context.Clinics on cd.Clinicid equals c.Id
                where cd.Clinicid == clinicId && c.MunicipalityId == municipalityId
                select new
                {
                    ClinicDoctorId = cd.Clinicdoctorid,
                    ClinicId = c.Id,
                    FullName = e.FirstName + " " + e.LastName,
                    DayOfWeek = cs.DayOfWeek,
                    StartTime = cs.StartTime,
                    EndTime = cs.EndTime
                }
            ).ToList();

            // Group by doctor
            var groupedDoctors = doctorSchedules
                .GroupBy(d => new { d.ClinicDoctorId, d.ClinicId, d.FullName })
                .Select(g => new ClinicDoctorDetailViewModel
                {
                    ClinicDoctorId =(int) g.Key.ClinicDoctorId,
                    ClinicId = (int)g.Key.ClinicId,
                    FullName = g.Key.FullName,
                    Schedules = g.Select(s => new DoctorScheduleViewModel
                    {
                        DayOfWeek = s.DayOfWeek,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime
                    }).ToList()
                })
                .ToList();

            return View(groupedDoctors);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
