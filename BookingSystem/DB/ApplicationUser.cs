using Microsoft.AspNetCore.Identity;

namespace BookingSystem.DB
{
    public class ApplicationUser : IdentityUser
    {
        public string Email { get; set; }
    }
}
