using BookingSystem.DB;
using BookingSystem.DTO;
using BookingSystem.Models;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly BookingDbContext _context;

        public CountryController(BookingDbContext context)
        {
            _context = context;
        }

        [HttpPost("create-country")]
        public async Task<IActionResult> CreateCountry([FromBody] CountryDTO reqCountry)
        {
            var model = new CountryModel
            {
                Id = Guid.NewGuid().ToString(),
                Code = reqCountry.Code
            };
            RecurringJob.AddOrUpdate(
               "CountryCreate",
               () => H_CountryCreate(model),
               Cron.Minutely);
            return Ok(model);
        }

        [HttpPost("get-country-list")]
        public async Task<IActionResult> GetCountry()
        {
            var lst = JsonConvert.SerializeObject(_context.Countries.ToList(), Formatting.Indented);
            var jobId = BackgroundJob.Enqueue(
                () => Console.WriteLine("CountryList  => {0}", lst));
            var result = string.IsNullOrWhiteSpace(jobId) ? "Fail." : "Success.";
            return Ok(new {result, lst});
        }

        [NonAction]
        public async Task<string> H_CountryCreate(CountryModel model)
        {
            await _context.Countries.AddAsync(model);
            var result = await _context.SaveChangesAsync();
            var message = result > 0 ? "Create Country Successful." : "Saving Fail.";
            Console.WriteLine(message);
            return message;
        }

    }
}
