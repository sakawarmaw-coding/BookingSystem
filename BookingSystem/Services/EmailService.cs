using BookingSystem.IServices;

namespace BookingSystem.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendPasswordResetEmailAsync(string email, string token)
        {
            // Generate a URL for the reset password link
            var resetUrl = $"https://localhost:7008/reset-password?token={token}";
            // Use your preferred email sending library to send the email
            // Example: using SmtpClient or a service like SendGrid
        }
    }
}
