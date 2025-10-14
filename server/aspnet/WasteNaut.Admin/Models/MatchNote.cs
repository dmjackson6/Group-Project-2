using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WasteNaut.Admin.Models
{
    [Table("match_notes")]
    public class MatchNote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MatchId { get; set; }

        [ForeignKey("MatchId")]
        public virtual Match Match { get; set; } = null!;

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
