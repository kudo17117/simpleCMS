namespace SimpleCMSWeb.Models.ModelView
{
    public class AdminCreateClinic
    {
        public string? ClinicName { get; set; }
        public string? ContactNo { get; set; }
        public string? AdditionalAddress { get; set; }

        public int SelectedRegionId { get; set; }
        public int SelectedProvinceId { get; set; }
        public int SelectedMunicipalityId { get; set; }

        public List<Clinic>? ClinicsExsting { get; set; }
        public List<Clinicdoctor>? ClinicDoctorsExsting { get; set; }
        public List<ClinicSchedule>? ExistingSchedules { get; set; }

        // Dropdown data
        public List<Region>? Regions { get; set; }
        public List<Province>? Provinces { get; set; }
        public List<Municipality>? Municipalities { get; set; }
    }


}
