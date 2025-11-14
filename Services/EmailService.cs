using SendGrid;
using SendGrid.Helpers.Mail;
using NewLook.Services.Interfaces;

namespace NewLook.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendVerificationEmailAsync(string toEmail, string username, string verificationLink)
        {
            try
            {
                var apiKey = _configuration["SendGrid:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogWarning("SendGrid API key not configured");
                    return;
                }

                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(
                    _configuration["SendGrid:FromEmail"] ?? "noreply@newlook.com",
                    _configuration["SendGrid:FromName"] ?? "NewLook"
                );
                var to = new EmailAddress(toEmail, username);
                var subject = "Verify your email address";

                var htmlContent = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; padding: 20px;'>
                        <div style='max-width: 600px; margin: 0 auto; background-color: #f8f9fa; padding: 30px; border-radius: 10px;'>
                            <h2 style='color: #333;'>Welcome to NewLook, {username}!</h2>
                            <p style='color: #666; font-size: 16px;'>
                                Thank you for registering. Please verify your email address by clicking the button below:
                            </p>
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{verificationLink}' 
                                   style='background-color: #0d6efd; color: white; padding: 12px 30px; 
                                          text-decoration: none; border-radius: 5px; display: inline-block;'>
                                    Verify Email
                                </a>
                            </div>
                            <p style='color: #999; font-size: 14px;'>
                                Or copy and paste this link into your browser:<br>
                                <a href='{verificationLink}' style='color: #0d6efd;'>{verificationLink}</a>
                            </p>
                            <p style='color: #999; font-size: 14px; margin-top: 30px;'>
                                This link will expire in 24 hours.
                            </p>
                            <hr style='border: 1px solid #dee2e6; margin: 20px 0;'>
                            <p style='color: #999; font-size: 12px;'>
                                If you didn't create an account, you can safely ignore this email.
                            </p>
                        </div>
                    </body>
                    </html>
                ";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
                var response = await client.SendEmailAsync(msg);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    _logger.LogInformation($"Verification email sent to {toEmail}");
                }
                else
                {
                    _logger.LogWarning($"Failed to send email. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending verification email");
            }
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string username, string resetLink)
        {
            // TODO: Implement password reset email
            await Task.CompletedTask;
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string username)
        {
            // TODO: Implement welcome email
            await Task.CompletedTask;
        }
    }
}