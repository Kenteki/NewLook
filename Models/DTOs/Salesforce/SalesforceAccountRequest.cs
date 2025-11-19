using System.Text.Json.Serialization;

namespace NewLook.Models.DTOs.Salesforce;

public class SalesforceAccountRequest
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("BillingStreet")]
    public string? BillingStreet { get; set; }

    [JsonPropertyName("BillingCity")]
    public string? BillingCity { get; set; }

    [JsonPropertyName("BillingState")]
    public string? BillingState { get; set; }

    [JsonPropertyName("BillingPostalCode")]
    public string? BillingPostalCode { get; set; }

    [JsonPropertyName("BillingCountry")]
    public string? BillingCountry { get; set; }

    [JsonPropertyName("Description")]
    public string? Description { get; set; }
}
