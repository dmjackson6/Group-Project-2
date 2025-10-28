namespace WasteNaut.Admin.DTOs
{
    public class OrganizationProfileDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public string? Website { get; set; }
        public string Address { get; set; } = string.Empty;
        public int CapacityMax { get; set; }
        public string? OperatingHours { get; set; }
        public string? Description { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UpdateOrganizationProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public string? Website { get; set; }
        public string Address { get; set; } = string.Empty;
        public int CapacityMax { get; set; }
        public string? OperatingHours { get; set; }
        public string? Description { get; set; }
    }
}
