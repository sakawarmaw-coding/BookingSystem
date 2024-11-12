using System.ComponentModel.DataAnnotations;

namespace BookingSystem.DTO
{
    public class CountryDTO
    {
        [Key]
        public string? Code { get; set; }
    }
}
