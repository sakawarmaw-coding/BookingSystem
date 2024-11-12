using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSystem.Models
{
    [Table("TblPackage")]
    public class PackageModel
    {
        [Key]
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Country { get; set; }
        public decimal Price { get; set; }
        public decimal Credit { get; set; }
        public string? Status { get; set; }
        public DateTime? ExpiryDate { get; set; }

    }
}
