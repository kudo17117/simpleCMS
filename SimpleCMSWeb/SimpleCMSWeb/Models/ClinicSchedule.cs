using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleCMSWeb.Models
{
    public class ClinicSchedule
    {
        [Key]
        [Column("clinicscheduleid")]
        public long ClinicScheduleId { get; set; }

        [Required]
        [Column("clinicdoctorid")]
        public long ClinicDoctorId { get; set; }

        [Required]
        [Column("dayofweek")]
        [StringLength(20)]
        public string? DayOfWeek { get; set; }

        [Required]
        [Column("starttime")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Column("endtime")]
        public TimeSpan EndTime { get; set; }

        public Clinicdoctor? Clinicdoctors { get; set; }
    }
}
