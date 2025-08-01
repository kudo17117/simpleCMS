using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleCMSWeb.Models;

public partial class Appointment
{
    public long Id { get; set; }

    public long? Patientid { get; set; }

    public long? Clinicdoctorid { get; set; }

    public long? Clinicid { get; set; }

    public long? Doctorid { get; set; }

    public DateOnly? Consultdate { get; set; }

    public TimeOnly? Consulttime { get; set; }

    public string? Commentdoctor { get; set; }

    public string? Commentpatient { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("is_approved_doctor")]
    public int IsApprovedDoctor { get; set; }

    [Column("is_approved_patient")]
    public int IsApprovedPatient { get; set; }

    [Column("patient_record_id")]
    public long? PatientRecordId { get; set; }

    [Column("is_done")]
    public int IsDone { get; set; }

    public bool Issync { get; set; }

    [Column("server_id")]
    public long ServerId { get; set; }

    public int Sequenceno { get; set; }

    public long? Referringhciid { get; set; }

    public string? Referringreason { get; set; }

    public string? Bloodpressure { get; set; }

    public string? Breathing { get; set; }

    public string? Pulse { get; set; }

    public string? Temperature { get; set; }

    public string? Height { get; set; }

    public string? Weight { get; set; }

    public string? Bpdiastolic { get; set; }

    public string? Subjective { get; set; }

    public string? Objective { get; set; }


    [NotMapped]
    public string? Doctor { get; set; }

    public virtual Clinic? Clinic { get; set; }

    public virtual Patient? Patient { get; set; }
    public virtual Clinicdoctor? Clinicdoctor { get; set; }

}
