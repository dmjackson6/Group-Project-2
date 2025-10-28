using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WasteNaut.Admin.Models
{
    [Table("requests")]
    public class Request
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Requester { get; set; } = string.Empty;

        [Required]
        public string Items { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Priority { get; set; } = "Medium";

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public int? OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization? Organization { get; set; }

        public string? Notes { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
