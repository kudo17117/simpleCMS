using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleCMSWeb.Models;

public partial class Patient
{
    [Column("patientid")]
    public long Patientid { get; set; }

    public string? Hospitalno { get; set; }

    public string? Patientbarcode { get; set; }

    public string? Lastname { get; set; }

    public string? Firstname { get; set; }

    public string? Middlename { get; set; }

    public DateTime? Birthdate { get; set; }

    public int? Age { get; set; }

    public string? Gender { get; set; }

    public string? Birthplace { get; set; }

    public string? Homeaddress { get; set; }

    public string? Bloodtype { get; set; }

    public string? Obstetricalhistory { get; set; }

    public string? Religion { get; set; }

    public string? Nationality { get; set; }

    public string? Occupation { get; set; }

    public string? Civilstatus { get; set; }

    public string? Philhealthno { get; set; }

    public string? Sssno { get; set; }

    public string? Gsisno { get; set; }

    public string? Motherid { get; set; }

    public string? Telephoneno { get; set; }

    public string? Mobileno { get; set; }

    public string? Emailaddress { get; set; }

    public string? Mothername { get; set; }

    public string? Fathername { get; set; }

    public string? Allergies { get; set; }

    public string? Familymedicalhistory { get; set; }

    public string? Socialhistory { get; set; }

    public DateTime? Registrydate { get; set; }

    public string? Registeredby { get; set; }

    public bool Isactive { get; set; }

    public bool? Isnewborn { get; set; }

    public string? Dependents { get; set; }

    public string? Educationalprofile { get; set; }

    public string? Socioeconomicprofile { get; set; }

    public decimal? Income { get; set; }

    public string? Company { get; set; }

    public string? Businessaddress { get; set; }

    public string? Oscaid { get; set; }

    public byte[]? Photo { get; set; }

    public string? Housenostreet { get; set; }

    public string? Barangayid { get; set; }

    public int? Municipalityid { get; set; }

    public int? Provinceid { get; set; }

    public int? Regionid { get; set; }

    public int? Glsalesaccount { get; set; }

    public short? Patienttype { get; set; }

    public bool? Isphilhealthmember { get; set; }

    public string? Relationshiptomember { get; set; }

    public string? Suffixname { get; set; }

    public DateTime? Dateupdated { get; set; }

    public bool? Isexpired { get; set; }

    public virtual ICollection<Patientuser>? Patientusers { get; set; }
    public virtual ICollection<Appointment>? Appointments { get; set; }

}
