using Microsoft.AspNetCore.Mvc;
using SimpleCMSWeb.Data;

namespace SimpleCMSWeb.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get Regions
        [HttpGet]
        public IActionResult GetRegions()
        {
            var regions = _context.Regions
                .Select(r => new { r.Regionid, r.Regionname })
                .ToList();

            return Json(regions);
        }

        // ✅ Get Provinces for selected Region
        [HttpGet]
        public IActionResult GetProvinces(int regionId)
        {
            var provinces = _context.Provinces
                .Where(p => p.RegionId == regionId)
                .Select(p => new { p.ProvinceId, p.ProvinceName })
                .ToList();

            return Json(provinces);
        }

        // ✅ Get Municipalities for selected Province
        [HttpGet]
        public IActionResult GetMunicipalities(int regionId, int provinceId)
        {
            var municipalities = _context.Municipalities
                .Where(m => m.ProvinceId == provinceId)
                .Select(m => new { m.MunicipalityId, m.MunicipalityName })
                .ToList();

            return Json(municipalities);
        }

        // ✅ Get Clinics in selected Municipality
        [HttpGet]
        public IActionResult GetClinics(int municipalityId)
        {
            var clinics = _context.Clinics
                .Where(c => c.MunicipalityId == municipalityId)
                .Select(c => new { c.Id, c.Name })
                .ToList();

            return Json(clinics);
        }

        // ✅ Get Doctors for selected Clinic
        //[HttpGet]
        //public IActionResult GetDoctors(int clinicId)
        //{
        //    var doctors = _context.ClinicDoctors
        //        .Where(cd => cd.Clinicdoctorid == clinicId)
        //        .Select(cd => new
        //        {
        //            cd.Clinicdoctorid,
        //            FullName = _context.Employees
        //                .Where(e => e.Employeeid == cd.Employeeid)
        //                .Select(e => e.FirstName + " " + e.LastName)
        //                .FirstOrDefault()
        //        })
        //        .ToList();

        //    return Json(doctors);
        //}

        [HttpGet]
        public IActionResult GetDoctors(int clinicId)
        {
            var doctors = _context.ClinicDoctors
                .Where(cd => cd.Clinicid == clinicId) // ✅ Correct field
                .Select(cd => new
                {
                    clinicDoctorId = cd.Clinicdoctorid, // match JavaScript
                    fullName = _context.Employees
                        .Where(e => e.Employeeid == cd.Employeeid)
                        .Select(e => e.FirstName + " " + e.LastName)
                        .FirstOrDefault()
                })
                .ToList();

            return Json(doctors);
        }


        // ✅ Autocomplete for search bar
        [HttpGet]
        public JsonResult Autocomplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new List<object>());
            }

            var results = (from c in _context.Clinics
                           join m in _context.Municipalities on c.MunicipalityId equals m.MunicipalityId
                           join p in _context.Provinces on m.ProvinceId equals p.ProvinceId
                           join r in _context.Regions on p.RegionId equals r.Regionid
                           where c.Name.Contains(term) ||
                                 m.MunicipalityName.Contains(term) ||
                                 p.ProvinceName.Contains(term) ||
                                 r.Regionname.Contains(term)
                           select new
                           {
                               id = c.Id,
                               clinicName = c.Name,
                               municipalityId = m.MunicipalityId,
                               municipalityName = m.MunicipalityName,
                               provinceId = p.ProvinceId,
                               provinceName = p.ProvinceName,
                               regionid = r.Regionid,
                               regionname = r.Regionname
                           })
                           .Take(10)
                           .ToList();

            return Json(results);
        }

    }
}
