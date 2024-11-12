using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSystem.DTO
{
    public class ClassDTO
    {
        [Key]
        public string? Name { get; set; }
        public string? CountryCode { get; set; }
        public int CreditRequired { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int TotalSlot { get; set; }
        public int BookedSlot { get; set; }
    }
}
