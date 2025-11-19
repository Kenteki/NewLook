using NewLook.Models.DTOs.Salesforce;

namespace NewLook.Services.Interfaces;

public interface ISalesforceService
{
    Task<SalesforceAccountResultDto> CreateAccountWithContactAsync(CreateSalesforceAccountDto dto, int userId);
    Task<bool> CheckIfUserExistsInSalesforceAsync(int userId);
}
