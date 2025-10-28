namespace WasteNaut.Admin.DTOs
{
    public class VolunteerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? Skills { get; set; }
        public DateTime StartDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? OrganizationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateVolunteerDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? Skills { get; set; }
        public DateTime StartDate { get; set; }
        public string Status { get; set; } = "pending";
        public int? OrganizationId { get; set; }
    }

    public class UpdateVolunteerDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? Skills { get; set; }
        public DateTime StartDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
