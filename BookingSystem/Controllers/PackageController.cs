using BookingSystem.DB;
using BookingSystem.DTO;
using BookingSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly BookingDbContext _context;

        public PackageController(BookingDbContext context)
        {
            _context = context;
        }

        [HttpPost("create_package")]
        public async Task<IActionResult> CreatePackage([FromBody] PackageDTO reqPackage)
        {
            PackageModel packageModel = new PackageModel();
            packageModel.Id = Guid.NewGuid().ToString();
            packageModel.Name = reqPackage.Name;
            packageModel.Country = reqPackage.CountryCode;
            packageModel.Price = reqPackage.Price;
            packageModel.Credit = reqPackage.Credit;
            packageModel.Status = "Active";
            packageModel.ExpiryDate = reqPackage.ExpiryDate;
            _context.Packages.Add(packageModel);
            int result = _context.SaveChanges();
            string message = result > 0 ? "Saving Successful" : "Saving Failed";
            return Ok(message);
        }

        [HttpPost("create_class")]
        public async Task<IActionResult> CreateClass([FromBody] ClassDTO reqClass)
        {
            ClassModel classModel = new ClassModel();
            classModel.Id = Guid.NewGuid().ToString();
            classModel.Name = reqClass.Name;
            classModel.Country = reqClass.CountryCode;
            classModel.CreditRequired = reqClass.CreditRequired;
            classModel.StartTime = reqClass.StartTime;
            classModel.EndTime = reqClass.EndTime;
            classModel.TotalSlot = reqClass.TotalSlot;
            classModel.BookedSlot = reqClass.BookedSlot;
            _context.Classes.Add(classModel);
            int result = _context.SaveChanges();
            string message = result > 0 ? "Saving Successful" : "Saving Failed";
            return Ok(message);
        }

        [HttpPost("purchase_package")]
        public async Task<IActionResult> PurchasePackage([FromBody] UserPackageDTO reqPackage)
        {
            UserPackageModel packageModel = new UserPackageModel();
            packageModel.Id = Guid.NewGuid().ToString();
            packageModel.UserId = reqPackage.UserId;
            packageModel.PackageId = reqPackage.PackageId;

            var user = _context.Users.FirstOrDefault(x => x.Id == reqPackage.UserId && x.CountryCode == reqPackage.CountryCode);

            var package = _context.Packages.FirstOrDefault(x => x.Id == reqPackage.PackageId && x.Country == reqPackage.CountryCode);


            if (user == null || package == null) return NotFound("User or package not found.");

            if (package != null)
            {
                packageModel.OriginalCredit = package.Credit;
                packageModel.RemainingCredit = package.Credit;
            }
            _context.Users.Update(user);

            packageModel.PurchaseDate = DateTime.Now;
            packageModel.Status = "Active";
            _context.UserPackages.Add(packageModel);
            int result = _context.SaveChanges();
            string message = result > 0 ? "Package Purchase Successfully" : "Saving Failed";
            return Ok(message);
        }

        [HttpPost("booking")]
        public async Task<IActionResult> BookClass([FromBody] BookingDTO reqBooking)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == reqBooking.UserId && x.CountryCode == reqBooking.CountryCode);
            var classSchedule = await _context.Classes.FirstOrDefaultAsync(cs => cs.Id == reqBooking.ClassId && cs.Country == reqBooking.CountryCode);

            if (user == null || classSchedule == null) return NotFound("User or class not found.");

            var userPackage = await _context.UserPackages.OrderBy(x => x.PurchaseDate).FirstOrDefaultAsync(x => x.UserId == reqBooking.UserId && x.PackageId == reqBooking.PackageId && x.Status == "Active");
            if (userPackage != null)
            {
                if (userPackage.RemainingCredit < classSchedule.CreditRequired) return BadRequest("Not enough credits.");

                if (classSchedule.BookedSlot >= classSchedule.TotalSlot)
                {
                    var waitlist = new BookingModel { Id = Guid.NewGuid().ToString(), UserId = reqBooking.UserId, ClassId = reqBooking.ClassId, BookingTime = DateTime.Now, UserPackageId = userPackage.Id, Status = "WaitList" };
                    _context.Bookings.Add(waitlist);
                }
                else
                {
                    var booking = new BookingModel { Id = Guid.NewGuid().ToString(), UserId = reqBooking.UserId, ClassId = reqBooking.ClassId, BookingTime = DateTime.Now, UserPackageId = userPackage.Id, Status = "Booked" };
                    _context.Bookings.Add(booking);
                    userPackage.RemainingCredit -= classSchedule.CreditRequired;
                }
            }

            await _context.SaveChangesAsync();

            var classObj = await _context.Classes.FirstOrDefaultAsync(x => x.Id == reqBooking.ClassId);
            if (classObj != null)
            {
                classObj.BookedSlot = (from row in _context.Bookings
                                       where row.Status == "Booked" && row.ClassId == reqBooking.ClassId
                                       select row).Count();
            }

            userPackage = _context.UserPackages.OrderBy(x => x.PurchaseDate).FirstOrDefault(x => x.UserId == reqBooking.UserId && x.PackageId == reqBooking.PackageId);
            if (userPackage != null)
            {
                if (userPackage.RemainingCredit == 0)
                {
                    userPackage.Status = "Expiry";
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Class booked successfully." });
        }

        [HttpPost("cancel_booking")]
        public async Task<IActionResult> CancelBooking(string bookingId, string userId, string classId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId && b.ClassId == classId);
            if (booking == null) return NotFound("Booking not found.");
            else
            {
                booking.Status = "Canceled";
                await _context.SaveChangesAsync();
            }
            var classSchedule = await _context.Classes.FindAsync(classId);

            var waitBook = await _context.Bookings.OrderBy(b => b.BookingTime).FirstOrDefaultAsync(b => b.ClassId == classId && b.Status == "WaitList");
            if (waitBook != null)
            {
                waitBook.Status = "Booked";
            }
            else
            {
                var user = await _context.UserPackages.FindAsync(booking.UserPackageId);
                if (user != null && classSchedule != null)
                {
                    user.RemainingCredit += classSchedule.CreditRequired;
                }

                if (classSchedule != null)
                {
                    classSchedule.BookedSlot = (from row in _context.Bookings
                                                where row.Status == "Booked" && row.ClassId == classId
                                                select row).Count();
                }
            }
            await _context.SaveChangesAsync();

            var userObj = await _context.UserPackages.FindAsync(booking.UserPackageId);
            if (userObj!=null)
            {
                if(userObj.RemainingCredit != 0)
                {
                    userObj.Status = "Active";
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Booking canceled." });
        }
    }
}
