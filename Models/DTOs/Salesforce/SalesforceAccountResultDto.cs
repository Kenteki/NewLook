namespace NewLook.Models.DTOs.Salesforce;

public class SalesforceAccountResultDto
{
    public bool Success { get; set; }
    public string? AccountId { get; set; }
    public string? ContactId { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
}
