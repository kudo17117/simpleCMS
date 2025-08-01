using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleCMSWeb.Models;

public partial class Clinicdoctor
{
    [Column("clinicdoctorid")]
    public long Clinicdoctorid { get; set; }

    [Column("clinicid")]
    public long? Clinicid { get; set; }

    [Column("employeeid")]
    public long? Employeeid { get; set; }

    [Column("class_id")]
    public int? ClassId { get; set; }

    [Column("best_time_to_call")]
    public string? BestTimeToCall { get; set; }

    [Column("createdbyid")]
    public long? Createdbyid { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updatedbyid")]
    public long? Updatedbyid { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("isactive")]
    public bool? Isactive { get; set; }

    [Column("patienttypeid")]
    public long? Patienttypeid { get; set; }

    [Column("clinicscheduleid")]
    public long? ClinicScheduleId { get; set; }

    [Column("averagepxperday")]
    public int? Averagepxperday { get; set; }

    // Navigation properties
    public virtual Employee? Employee { get; set; }
    public virtual Clinic? Clinic { get; set; }
    public virtual ICollection<ClinicSchedule>? ClinicSchedules { get; set; }
    public virtual ICollection<Appointment>? Appointments { get; set; }
}