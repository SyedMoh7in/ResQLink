using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResQLink.DTOs;
using ResQLink.Models;

namespace ResQLink.Areas.Responder.Controllers
{
    [Area("Responder")]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Identifier)
                        ?? await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.Identifier);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return View("Login", model);

            var roles = await _userManager.GetRolesAsync(user);

            await _signInManager.SignInAsync(user, isPersistent: false);

            if (roles.Contains("Med Emergency Responder"))
                return RedirectToAction("Dashboard", "MedResponder", new { area = "Responder" });

            if (roles.Contains("Tow Emergency Responder"))
                return RedirectToAction("Dashboard", "TowEmergency", new { area = "Responder" });

            if (roles.Contains("Firetruck Emergency Responder"))
                return RedirectToAction("Dashboard", "FiretruckEmergency", new { area = "Responder" });

            if (roles.Contains("Security Emergency Responder"))
                return RedirectToAction("Dashboard", "SecurityEmergency", new { area = "Responder" });

            return RedirectToAction("AccessDenied", "Account");
        }
    }
}
