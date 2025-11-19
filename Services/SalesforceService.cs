using Microsoft.Extensions.Options;
using NewLook.Data;
using NewLook.Models;
using NewLook.Models.DTOs.Salesforce;
using NewLook.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NewLook.Services;

public class SalesforceService : ISalesforceService
{
    private readonly SalesforceSettings _settings;
    private readonly ISalesforceAuthService _authService;
    private readonly HttpClient _httpClient;
    private readonly ILogger<SalesforceService> _logger;
    private readonly ApplicationDbContext _context;

    public SalesforceService(
        IOptions<SalesforceSettings> settings,
        ISalesforceAuthService authService,
        IHttpClientFactory httpClientFactory,
        ILogger<SalesforceService> logger,
        ApplicationDbContext context)
    {
        _settings = settings.Value;
        _authService = authService;
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _context = context;
    }

    public async Task<SalesforceAccountResultDto> CreateAccountWithContactAsync(
        CreateSalesforceAccountDto dto, int userId)
    {
        var result = new SalesforceAccountResultDto();

        try
        {
            var token = await _authService.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var accountId = await CreateAccountAsync(dto);

            if (string.IsNullOrEmpty(accountId))
            {
                result.Success = false;
                result.Message = "Failed to create Salesforce Account";
                return result;
            }

            var contactId = await CreateContactAsync(dto, accountId);

            if (string.IsNullOrEmpty(contactId))
            {
                result.Success = false;
                result.Message = "Account created but Contact creation failed";
                result.AccountId = accountId;
                return result;
            }

            await SaveSalesforceIdsToDatabase(userId, accountId, contactId);

            result.Success = true;
            result.AccountId = accountId;
            result.ContactId = contactId;
            result.Message = "Successfully created Account and Contact in Salesforce";

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Salesforce Account/Contact for user {UserId}", userId);
            result.Success = false;
            result.Message = "An error occurred while creating Salesforce records";
            result.Errors.Add(ex.Message);
            return result;
        }
    }

    public async Task<bool> CheckIfUserExistsInSalesforceAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user != null &&
               !string.IsNullOrEmpty(user.SalesforceAccountId) &&
               !string.IsNullOrEmpty(user.SalesforceContactId);
    }

    private async Task<string?> CreateAccountAsync(CreateSalesforceAccountDto dto)
    {
        var accountRequest = new SalesforceAccountRequest
        {
            Name = string.IsNullOrWhiteSpace(dto.Company)
                ? $"{dto.FirstName} {dto.LastName}"
                : dto.Company,
            Phone = dto.Phone,
            BillingStreet = dto.Street,
            BillingCity = dto.City,
            BillingState = dto.State,
            BillingPostalCode = dto.PostalCode,
            BillingCountry = dto.Country,
            Description = dto.Description
        };

        var url = $"{_authService.GetInstanceUrl()}/services/data/{_settings.ApiVersion}/sobjects/Account";
        var json = JsonSerializer.Serialize(accountRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to create Salesforce Account: {StatusCode} - {Content}",
                response.StatusCode, responseContent);
            return null;
        }

        var createResponse = JsonSerializer.Deserialize<SalesforceCreateResponse>(responseContent);
        return createResponse?.Success == true ? createResponse.Id : null;
    }

    private async Task<string?> CreateContactAsync(CreateSalesforceAccountDto dto, string accountId)
    {
        var contactRequest = new SalesforceContactRequest
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Title = dto.JobTitle,
            AccountId = accountId,
            MailingStreet = dto.Street,
            MailingCity = dto.City,
            MailingState = dto.State,
            MailingPostalCode = dto.PostalCode,
            MailingCountry = dto.Country,
            Description = dto.Description
        };

        var url = $"{_authService.GetInstanceUrl()}/services/data/{_settings.ApiVersion}/sobjects/Contact";
        var json = JsonSerializer.Serialize(contactRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to create Salesforce Contact: {StatusCode} - {Content}",
                response.StatusCode, responseContent);
            return null;
        }

        var createResponse = JsonSerializer.Deserialize<SalesforceCreateResponse>(responseContent);
        return createResponse?.Success == true ? createResponse.Id : null;
    }

    private async Task SaveSalesforceIdsToDatabase(int userId, string accountId, string contactId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.SalesforceAccountId = accountId;
            user.SalesforceContactId = contactId;
            await _context.SaveChangesAsync();
        }
    }
}
