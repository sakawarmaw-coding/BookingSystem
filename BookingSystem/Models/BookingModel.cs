using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSystem.Models
{
    [Table("TblBooking")]
    public class BookingModel
    {
        [Key]
        public string Id { get; set; }
        public string? UserId { get; set; }
        public string? ClassId { get; set; }
        public string? UserPackageId { get; set; }
        public DateTime? BookingTime { get; set; }
        public string? Status { get; set; }
    }
}
