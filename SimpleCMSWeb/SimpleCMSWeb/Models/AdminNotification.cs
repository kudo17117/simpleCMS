using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleCMSWeb.Models
{
    [Table("AdminNotification")]
    public class AdminNotification
    {
        [Column("NotificationId")]
        public int Id { get; set; }
        public string? Message { get; set; }
        public bool? IsRead { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
