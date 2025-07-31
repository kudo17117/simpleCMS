using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleCMSWeb.Models;

[Table("patientusers")]
public partial class Patientuser
{
    [Key]
    public int Patientuserid { get; set; }
    [Column("patientid")]
    public long? Patientid { get; set; }

    public bool Isactive { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = null!;

    [Required]
    [RegularExpression(@"^[a-zA-Z0-9]{6,16}$", ErrorMessage = "Password must be 6-16 characters and contain only letters and numbers.")]
    [DataType(DataType.Password)]
    [MaxLength(100)] // adjust if your DB column is longer or shorter
    public string Password { get; set; } = null!;

    [MaxLength(100)]
    public string? Googleaccountid { get; set; }

    [MaxLength(100)]
    public string? Facebookaccountid { get; set; }

    public DateTime? Dateonlineaccountactivated { get; set; }

    [MaxLength(100)]
    public string? Passwordresettoken { get; set; }

    [MaxLength(100)]
    public string? Usertoken { get; set; }

    [MaxLength(255)]
    public string? Userprofile { get; set; }

    public virtual Patient? Patient { get; set; }
}