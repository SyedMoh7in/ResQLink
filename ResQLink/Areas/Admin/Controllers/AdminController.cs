using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResQLink.Data;
using ResQLink.DTOs;
using ResQLink.Models;

namespace ResQLink.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        public AdminController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
    IWebHostEnvironment webHostEnvironment, ApplicationDbContext context, IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _context = context;
            _emailSender = emailSender;
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult ServiceList()
        {
            return View();
        }

        public IActionResult ResponderList()
        {
            var responders = _context.Profiles
                .Where(p => p.Role != "User")
                .OrderByDescending(p => p.Id)  
                .ToList();

            return View(responders);
        }


        [HttpGet]
        public IActionResult AddResponder()
        {
            return View(new AddResponderDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddResponder(AddResponderDto model)
        {

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.ContactNumber,
                EmailConfirmed = true 
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
                await _roleManager.CreateAsync(new IdentityRole(model.Role));

            await _userManager.AddToRoleAsync(user, model.Role);

            string photoUrl = null;
            if (model.Photo != null)
            {
                var fileName = $"{Guid.NewGuid()}_{model.Photo.FileName}";
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.Photo.CopyToAsync(stream);
                }
                photoUrl = $"/uploads/{fileName}";
            }

            var profile = new Profile
            {
                UserId = user.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Role = model.Role,
                PhoneNumber = model.ContactNumber,
                //ProfilePictureUrl = photoUrl,
                ProfilePictureUrl = "",
                ServiceType = model.ServiceType,
                PlateNumber = model.PlateNumber,
                BankName = model.BankName,
                AccountNumber = model.AccountNumber,
                BankIBAN = model.BankIBAN,
                RoutingNumber = model.RoutingNumber,
                BankSwift = model.BankSwift,
                Address = model.Address,
                Bio = "",
            };

            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();

            var subject = "Welcome to ResQLink!";
            var message = $"Hello {model.FirstName},<br/><br/>" +
                          $"You have been registered as <strong>{model.Role}</strong> for ResQLink.<br/>" +
                          $"You can now log in using your email and the password provided to you.<br/><br/>" +
                          $"Thank you!<br/>ResQLink Team";

            await _emailSender.SendEmailAsync(user.Email, subject, message);

            TempData["Success"] = "Responder/Admin created successfully!";
            return RedirectToAction("ResponderList");
        }

        public IActionResult ResponderDoc()
        {
            return View();
        }
    }
}
