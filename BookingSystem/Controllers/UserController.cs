using BookingSystem.DTO;
using BookingSystem.DB;
using BookingSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookingSystem.IServices;
using System.Numerics;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Azure.Core;
using BookingSystem.Services;
using Hangfire;
using Newtonsoft.Json;

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BookingDbContext _context;

        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public UserController(BookingDbContext context, ITokenService tokenService, IEmailService emailService)
        {
            _context = context;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO reqUser)
        {
            var model = new UserModel
            {
                Id = Guid.NewGuid().ToString(),
                Name = reqUser.Name,
                Email = reqUser.Email,
                Password = reqUser.Password,
                CountryCode = reqUser.CountryCode
            };
            RecurringJob.AddOrUpdate(
               "Register",
               () => H_Register(model),
               Cron.Minutely);
            return Ok(model);
        }

        [NonAction]
        public async Task<string> H_Register(UserModel model)
        {
            await _context.Users.AddAsync(model);
            var result = await _context.SaveChangesAsync();
            var message = result > 0 ? "Register Successful." : "Saving Fail.";
            Console.WriteLine(message);
            return message;
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var item = _context.Users.FirstOrDefault(x => x.Email == email && x.Password == password);
            if (item != null)
            {
                return new JsonResult(new { status = "Registration Successful!", userName = email, token = _tokenService.CreateToken(email, out DateTime ExpireDate), expireDate = ExpireDate, refreshToken = _tokenService.RefreshToken(email, Guid.NewGuid().ToString(), out DateTime RefreshExpireDate), refreshExpiryDatetime = RefreshExpireDate });

            }
            else
            {
                return BadRequest("Registration Fail!");
            }
        }

        [HttpPost("get-profile")]
        public async Task<IActionResult> GetProfile(string email)
        {
            var item = _context.Users.FirstOrDefault(x => x.Email == email);
            var jobId = BackgroundJob.Enqueue(
                () => Console.WriteLine("GetProfile Data  => {0}", item));
            var result = string.IsNullOrWhiteSpace(jobId) ? "Fail." : "Success.";
            return Ok(new { result, item });
        }



        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO reqUser)
        {
            if (reqUser.NewPassword == reqUser.ConfirmPassword)
            {
                var item = _context.Users.FirstOrDefault(x => x.Email == reqUser.Email && x.Password== reqUser.CurrentPassword);
                if (item != null)
                {
                    item.Password=reqUser.NewPassword;
                }
                else
                {
                    return BadRequest("Please check current password, User Not found!");
                }
            }
            else
            {
                return BadRequest("New Password and ConfirmPassword don't match");
            }
            int result = _context.SaveChanges();
            string message = result > 0 ? "Updating Successful" : "Updating Failed";
            return Ok(message);
        }


        //[HttpPost("request-password-reset")]
        //public async Task<IActionResult> RequestPasswordReset([FromBody] ResetPasswordDTO model)
        //{
        //    var user = await _userManager.FindByEmailAsync(model.Email);
        //    if (user == null)
        //    {
        //        return BadRequest("No user found with this email.");
        //    }

        //    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        //    await _emailService.SendPasswordResetEmailAsync(model.Email, token);

        //    return Ok("Password reset email sent.");
        //}

        //[HttpPost("reset-password")]
        //public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        //{
        //    var user = await _userManager.FindByEmailAsync(model.Email);
        //    if (user == null)
        //    {
        //        return BadRequest("No user found with this email.");
        //    }

        //    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        //    if (result.Succeeded)
        //    {
        //        return Ok("Password has been reset successfully.");
        //    }

        //    return BadRequest(result.Errors);
        //}

    }
}
