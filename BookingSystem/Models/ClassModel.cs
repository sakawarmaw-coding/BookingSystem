using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSystem.Models
{
    [Table("TblClass")]
    public class ClassModel
    {
        [Key]
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Country { get; set; }
        public int CreditRequired { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int TotalSlot { get; set; }
        public int BookedSlot { get; set; }
    }
}
