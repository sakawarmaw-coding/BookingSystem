using BookingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookingSystem.DB
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<PackageModel> Packages { get; set; }
        public DbSet<UserPackageModel> UserPackages { get; set; }
        public DbSet<ClassModel> Classes { get; set; }
        public DbSet<BookingModel> Bookings { get; set; }
        public DbSet<CountryModel> Countries { get; set; }
    }
}
