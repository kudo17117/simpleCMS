using System;
using System.Collections.Generic;

namespace SimpleCMSWeb.Models;

public partial class Region
{
    public int Regionid { get; set; }

    public string? Regioncode { get; set; }

    public string? Regionname { get; set; }

    public bool? Isactive { get; set; }

    public bool? Isdefault { get; set; }

    public long? Createdbyid { get; set; }

    public DateTime? Datecreated { get; set; }

    public long? Updatebyid { get; set; }

    public DateTime? Dateupdate { get; set; }
    public virtual ICollection<Province>? Provinces { get; set; }
}
