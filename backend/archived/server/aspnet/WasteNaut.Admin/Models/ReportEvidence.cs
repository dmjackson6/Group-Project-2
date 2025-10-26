using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WasteNaut.Admin.Models
{
    [Table("report_evidence")]
    public class ReportEvidence
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ReportId { get; set; }

        [ForeignKey("ReportId")]
        public virtual Report Report { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
