using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSystem.Models
{
    [Table("TblUserPackage")]
    public class UserPackageModel
    {
        [Key]
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? PackageId { get; set; }
        public decimal? OriginalCredit { get; set; }
        public decimal? RemainingCredit { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? Status { get; set; }
    }
}
