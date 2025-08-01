using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleCMSWeb.Models
{
    public class EmployeeType
    {
        public int EmployeeTypeId { get; set; }
        [Column("employeetypedescription")]
        public string? EmployeeTypeDescription { get; set; }

        public int? ReportSeqNo { get; set; }

        public bool? IsActive { get; set; } = true;
        [Column("et_isdefault")]
        public bool? EtIsDefault { get; set; }
        [Column("et_memo")]
        public string? EtMemo { get; set; }

        public long? CreatedById { get; set; }

        public DateTime? DateCreated { get; set; }

        public long? UpdatedById { get; set; }

        public DateTime? DateUpdated { get; set; }

        public long? SalesAccountId { get; set; }

        public long? ExpenseAccountId { get; set; }

        public virtual ICollection<Employee>? Employees { get; set; }
    }
}
