namespace SimpleCMSWeb.Models.ModelView
{
    public class ClinicDoctorDetailViewModel
    {
        public int ClinicId { get; set; }
        public int ClinicDoctorId { get; set; }
        public string FullName { get; set; }

        public List<DoctorScheduleViewModel> Schedules { get; set; }
    }

    public class DoctorScheduleViewModel
    {
        public string DayOfWeek { get; set; } // "Monday", "Tuesday", etc.
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }


}
