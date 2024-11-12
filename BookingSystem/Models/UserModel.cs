using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSystem.Models
{
    [Table("TblUser")]
    public class UserModel
    {
        [Key]
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? CountryCode { get; set; }
    }
}
