using System.ComponentModel.DataAnnotations;

namespace WasteNaut.Admin.DTOs
{
    public class SuspendUserRequestDto
    {
        [Required]
        public string Reason { get; set; } = string.Empty;
    }
}
