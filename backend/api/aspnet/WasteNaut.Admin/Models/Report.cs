using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WasteNaut.Admin.Models
{
    [Table("reports")]
    public class Report
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Priority { get; set; } = "medium";

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "pending";

        [Required]
        public int ReporterId { get; set; }

        [ForeignKey("ReporterId")]
        public virtual User Reporter { get; set; } = null!;

        public int? ReportedUserId { get; set; }

        [ForeignKey("ReportedUserId")]
        public virtual User? ReportedUser { get; set; }

        public int? AssignedAdminId { get; set; }

        [ForeignKey("AssignedAdminId")]
        public virtual Admin? AssignedAdmin { get; set; }

        public DateTime? ResolvedAt { get; set; }

        public string? ResolutionNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<ReportEvidence> Evidence { get; set; } = new List<ReportEvidence>();
        public virtual ICollection<ReportNote> Notes { get; set; } = new List<ReportNote>();
    }
}
