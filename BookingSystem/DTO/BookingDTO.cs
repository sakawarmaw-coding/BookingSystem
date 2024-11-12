using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSystem.DTO
{
    public class BookingDTO
    {
        [Key]
        public string? UserId { get; set; }
        public string? ClassId { get; set; }
        public string? PackageId { get; set; }
        public string? CountryCode { get; set; }
    }
}
