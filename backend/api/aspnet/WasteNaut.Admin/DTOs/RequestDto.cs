namespace WasteNaut.Admin.DTOs
{
    public class RequestDto
    {
        public int Id { get; set; }
        public string Requester { get; set; } = string.Empty;
        public string Items { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public int? OrganizationId { get; set; }
        public string? Notes { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateRequestDto
    {
        public string Requester { get; set; } = string.Empty;
        public string Items { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public string Status { get; set; } = "Pending";
        public int? OrganizationId { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateRequestDto
    {
        public string Requester { get; set; } = string.Empty;
        public string Items { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
