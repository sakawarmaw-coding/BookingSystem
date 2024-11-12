namespace BookingSystem.IServices
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string email, string token);
    }
}
