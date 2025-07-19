using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ResQLink.DTOs;
using ResQLink.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ResQLink.Controllers
{
    // Controllers/AuthController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              IConfiguration config,
                              RoleManager<IdentityRole> roleManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        private static readonly Dictionary<string, (string Otp, DateTime Expiry)> _otpStore = new();


        [HttpPost("send-verification-email")]
        public async Task<IActionResult> SendVerificationEmail([FromBody] EmailRequestDto model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest("Email is already in use.");
            
            var otp = new Random().Next(100000, 999999).ToString();
            var expiryTime = DateTime.UtcNow.AddSeconds(120);

            _otpStore[model.Email] = (otp, expiryTime);

            var emailBody = $"<h3>Your verification code:</h3><p>{otp}</p>";
            await _emailSender.SendEmailAsync(model.Email, "ResQLink - Email Verification Code", emailBody);

            return Ok(new { message = "Verification code sent successfully." });
        }


        [HttpPost("verify-code")]
        public IActionResult VerifyCode([FromBody] EmailVerificationDto model)
        {
            if (!_otpStore.TryGetValue(model.Email, out var otpEntry))
                return BadRequest("No verification code found for this email.");

            if (DateTime.UtcNow > otpEntry.Expiry)
            {
                _otpStore.Remove(model.Email);
                return BadRequest("Verification code has expired.");
            }

            if (otpEntry.Otp != model.Code)
                return BadRequest("Invalid verification code.");

            _otpStore.Remove(model.Email); // One-time use
            return Ok("Email verified successfully.");
        }



        [HttpPost("complete-registration")]
        public async Task<IActionResult> CompleteRegistration([FromBody] RegisterDto model)
        {
            if (model.Password != model.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
                return BadRequest("User already exists.");

            var user = new ApplicationUser
            {
                UserName = model.Email ?? model.PhoneNumber ?? Guid.NewGuid().ToString(),
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Name = model.Name,
                Age = model.Age ?? 0,
                Location = model.Location,
                ProfilePicture = model.ProfilePicture,
                EmailConfirmed = true 
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var role = "User";
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            return Ok("User registered successfully.");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(model.Identifier)
                                      ?? await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.Identifier);

            if (user == null)
                return Unauthorized("User not found.");

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized("Invalid credentials.");

            var userRoles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, userRoles);

            return Ok(new
            {
                token,
                role = userRoles.FirstOrDefault(),
                username = user.UserName
            });
        }

        private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                expires: DateTime.Now.AddDays(7),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
