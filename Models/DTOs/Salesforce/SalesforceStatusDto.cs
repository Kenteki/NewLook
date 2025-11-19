namespace NewLook.Models.DTOs.Salesforce;

public class SalesforceStatusDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsSynced { get; set; }
    public string? AccountId { get; set; }
    public string? ContactId { get; set; }
}
