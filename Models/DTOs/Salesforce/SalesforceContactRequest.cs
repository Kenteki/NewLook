using System.Text.Json.Serialization;

namespace NewLook.Models.DTOs.Salesforce;

public class SalesforceContactRequest
{
    [JsonPropertyName("FirstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("LastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("Email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("Phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("Title")]
    public string? Title { get; set; }

    [JsonPropertyName("AccountId")]
    public string? AccountId { get; set; }

    [JsonPropertyName("MailingStreet")]
    public string? MailingStreet { get; set; }

    [JsonPropertyName("MailingCity")]
    public string? MailingCity { get; set; }

    [JsonPropertyName("MailingState")]
    public string? MailingState { get; set; }

    [JsonPropertyName("MailingPostalCode")]
    public string? MailingPostalCode { get; set; }

    [JsonPropertyName("MailingCountry")]
    public string? MailingCountry { get; set; }

    [JsonPropertyName("Description")]
    public string? Description { get; set; }
}
