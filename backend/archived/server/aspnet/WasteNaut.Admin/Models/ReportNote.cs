using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WasteNaut.Admin.Models
{
    [Table("report_notes")]
    public class ReportNote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ReportId { get; set; }

        [ForeignKey("ReportId")]
        public virtual Report Report { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;

        [Required]
        public string Text { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = "internal";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
