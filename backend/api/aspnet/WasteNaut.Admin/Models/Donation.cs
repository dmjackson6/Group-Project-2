using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WasteNaut.Admin.Models
{
    [Table("donations")]
    public class Donation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "active";

        [Required]
        public int Quantity { get; set; }

        [Required]
        [MaxLength(50)]
        public string Unit { get; set; } = string.Empty;

        [Required]
        public int DonorId { get; set; }

        [ForeignKey("DonorId")]
        public virtual User Donor { get; set; } = null!;

        public int? RecipientId { get; set; }

        [ForeignKey("RecipientId")]
        public virtual User? Recipient { get; set; }

        public string? LocationAddress { get; set; }

        [Column(TypeName = "decimal(10,8)")]
        public decimal? LocationLat { get; set; }

        [Column(TypeName = "decimal(11,8)")]
        public decimal? LocationLng { get; set; }

        public DateTime? PickupWindowStart { get; set; }

        public DateTime? PickupWindowEnd { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string? Tags { get; set; } // JSON string

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
