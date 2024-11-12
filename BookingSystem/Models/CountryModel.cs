using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSystem.Models
{
    [Table("TblCountry")]
    public class CountryModel
    {
        [Key]
        public string Id { get; set; }
        public string? Code { get; set; }
    }
}
