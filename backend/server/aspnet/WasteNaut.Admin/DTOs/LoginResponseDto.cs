namespace WasteNaut.Admin.DTOs
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public AdminDto User { get; set; } = new();
    }
}
