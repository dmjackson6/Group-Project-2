using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WasteNaut.Admin.Models
{
    [Table("organizations")]
    public class Organization
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "pending";

        [MaxLength(50)]
        public string? RegistrationNumber { get; set; }

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ContactName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string ContactEmail { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? ContactPhone { get; set; }

        public int CapacityMax { get; set; } = 0;

        public int CapacityUsed { get; set; } = 0;

        public string? CapacityNotes { get; set; }

        public string? ServiceAreas { get; set; } // JSON string

        public string? Description { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public DateTime? RejectedAt { get; set; }

        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
