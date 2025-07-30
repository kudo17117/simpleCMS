using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleCMSWeb.Models;

public partial class Clinic
{
    public long Id { get; set; } // Keep this as is (PK)

    [Column("cliniccode")]
    public string? Cliniccode { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("contact_no")]
    public string? ContactNo { get; set; }

    [Column("addition_address")]
    public string? AdditionAddress { get; set; }

    [Column("barangayid")]
    public long? BarangayId { get; set; }

    [Column("latitude")]
    public string? Latitude { get; set; }

    [Column("longitude")]
    public string? Longitude { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("is_default")]
    public bool? IsDefault { get; set; }

    [Column("isverified")]
    public bool? Isverified { get; set; }

    [Column("last_download")]
    public DateTime? LastDownload { get; set; }

    [Column("last_upload")]
    public DateTime? LastUpload { get; set; }

    [Column("issync")]
    public bool? Issync { get; set; }

    [Column("installationdate")]
    public DateTime? Installationdate { get; set; }

    [Column("serialkey")]
    public string? Serialkey { get; set; }

    [Column("isactivated")]
    public bool? Isactivated { get; set; }

    [Column("municipalityid")]
    public int? MunicipalityId { get; set; }
    public virtual ICollection<Clinicdoctor>? ClinicDoctors { get; set; }
    public virtual Municipality? Municipality { get; set; }
    public virtual ICollection<Appointment>? Appointments { get; set; }
}
