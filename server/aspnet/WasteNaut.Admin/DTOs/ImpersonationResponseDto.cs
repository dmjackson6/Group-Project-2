namespace WasteNaut.Admin.DTOs
{
    public class ImpersonationResponseDto
    {
        public string ImpersonationToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
