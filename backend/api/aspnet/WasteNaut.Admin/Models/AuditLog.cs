using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WasteNaut.Admin.Models
{
    [Table("audit_logs")]
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string User { get; set; } = string.Empty;

        public int? UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Target { get; set; } = string.Empty;

        public int? TargetId { get; set; }

        public string? Details { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
