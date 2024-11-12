using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSystem.DTO
{
    public class PackageDTO
    {
        [Key]
        public string? Name { get; set; }
        public string? CountryCode { get; set; }
        public decimal Price { get; set; }
        public decimal Credit { get; set; }
        public DateTime? ExpiryDate { get; set; }

    }
}
