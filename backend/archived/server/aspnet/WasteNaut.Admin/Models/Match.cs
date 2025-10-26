using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WasteNaut.Admin.Models
{
    [Table("matches")]
    public class Match
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [Range(0, 100)]
        public int Confidence { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "pending";

        [Required]
        public int FromUserId { get; set; }

        [ForeignKey("FromUserId")]
        public virtual User FromUser { get; set; } = null!;

        [Required]
        public int ToUserId { get; set; }

        [ForeignKey("ToUserId")]
        public virtual User ToUser { get; set; } = null!;

        public string? AiNotes { get; set; }

        public string? Factors { get; set; } // JSON string

        public string? OverrideReason { get; set; }

        public string? OverrideNotes { get; set; }

        public int? OverrideConfidence { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public DateTime? AcceptedAt { get; set; }

        public DateTime? RejectedAt { get; set; }

        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<MatchNote> Notes { get; set; } = new List<MatchNote>();
    }
}
