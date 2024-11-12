using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSystem.DTO
{
    public class UserDTO
    {
        [Key]
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? CountryCode { get; set; }
    }
}
