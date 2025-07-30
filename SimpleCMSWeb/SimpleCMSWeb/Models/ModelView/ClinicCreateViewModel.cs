using System.ComponentModel.DataAnnotations;

namespace SimpleCMSWeb.Models.ModelView
{
    public class ClinicCreateViewModel
    {
        public string? ClinicName { get; set; }
        public string? ContactNo { get; set; }
        public string? AdditionalAddress { get; set; }

        public string? Region { get; set; }
        public string? Province { get; set; }
        public string? Municipality { get; set; }
        public string? AdditionAddress { get; set; }
        public int? SelectedClinicId { get; set; }
        public long ClinicDoctorId { get; set; }

        public List<ScheduleViewModel> Schedules { get; set; } = new();

        public List<Clinicdoctor>? ClinicDoctorsExsting { get; set; }
        public List<ClinicSchedule>? ExistingSchedules { get; set; }


    }

    public class ScheduleViewModel
    {
        public string? DayOfWeek { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
