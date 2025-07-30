using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleCMSWeb.Models;

public partial class User
{
    [Key]
    [Column("employeeid")]
    public long Employeeid { get; set; }

    [Column("username")]
    public string? Username { get; set; }

    [Column("userpass")]
    public string? Userpass { get; set; } = null!;

    [Column("usersign")]
    public byte[]? Usersign { get; set; } = null!;

    [Column("isappsysadmin")]
    public bool? Isappsysadmin { get; set; }

    [Column("isactive")]
    public bool? Isactive { get; set; }

    [Column("u_createdby")]
    public long? UCreatedby { get; set; }

    [Column("u_datecreated")]
    public DateTime? UDatecreated { get; set; }

    [Column("u_updatedby")]
    public long? UUpdatedby { get; set; }

    [Column("u_dateupdated")]
    public DateTime? UDateupdated { get; set; }

    public virtual Employee? Employee { get; set; }
}
