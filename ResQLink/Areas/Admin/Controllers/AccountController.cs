using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResQLink.DTOs;
using ResQLink.Models;

namespace ResQLink.Areas.Admin.Controllers
{
    [Area("Admin")]
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
        public IActionResult Login()
        {
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Identifier) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(string.Empty, "Email/Phone and Password are required.");
                return View(model);
            }

            ApplicationUser user = null;

            // Try to detect if identifier is email
            if (model.Identifier.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(model.Identifier);
            }
            else
            {
                // fallback to username or phone (assuming you're storing phone in username)
                user = await _userManager.FindByNameAsync(model.Identifier);
            }

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError(string.Empty, "Invalid login credentials.");
                return View(model);
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Admin"))
            {
                ModelState.AddModelError(string.Empty, "Access denied.");
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Dashboard", "Admin", new { area = "Admin" });
        }
    }
}

