using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewLook.Data;
using NewLook.Models.DTOs.Salesforce;
using NewLook.Services.Interfaces;
using System.Security.Claims;

namespace NewLook.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesforceController : ControllerBase
{
    private readonly ISalesforceService _salesforceService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SalesforceController> _logger;

    public SalesforceController(
        ISalesforceService salesforceService,
        ApplicationDbContext context,
        ILogger<SalesforceController> logger)
    {
        _salesforceService = salesforceService;
        _context = context;
        _logger = logger;
    }

    [HttpPost("create-account")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateSalesforceAccountDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && userId != user.Id)
            {
                return Forbid();
            }

            var exists = await _salesforceService.CheckIfUserExistsInSalesforceAsync(userId);
            if (exists)
            {
                return BadRequest(new
                {
                    message = "User already has a Salesforce Account and Contact"
                });
            }

            var result = await _salesforceService.CreateAccountWithContactAsync(dto, userId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateAccount endpoint");
            return StatusCode(500, new { message = "An error occurred while creating Salesforce records" });
        }
    }

    [HttpGet("check-status")]
    public async Task<IActionResult> CheckStatus()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var exists = await _salesforceService.CheckIfUserExistsInSalesforceAsync(userId);

            return Ok(new
            {
                existsInSalesforce = exists
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CheckStatus endpoint");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpGet("admin/check-status/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminCheckStatus(int userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var isSynced = !string.IsNullOrEmpty(user.SalesforceAccountId) &&
                          !string.IsNullOrEmpty(user.SalesforceContactId);

            return Ok(new SalesforceStatusDto
            {
                Username = user.Username,
                Email = user.Email,
                IsSynced = isSynced,
                AccountId = user.SalesforceAccountId,
                ContactId = user.SalesforceContactId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AdminCheckStatus endpoint");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpPost("admin/create-account/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminCreateAccount(int userId, [FromBody] CreateSalesforceAccountDto dto)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var exists = await _salesforceService.CheckIfUserExistsInSalesforceAsync(userId);
            if (exists)
            {
                return BadRequest(new
                {
                    message = "User already has a Salesforce Account and Contact"
                });
            }

            var result = await _salesforceService.CreateAccountWithContactAsync(dto, userId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AdminCreateAccount endpoint");
            return StatusCode(500, new { message = "An error occurred while creating Salesforce records" });
        }
    }
}
