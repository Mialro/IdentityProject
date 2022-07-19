using IdentityProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityProject.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ClaimsController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View(User.Claims);
        }

        [HttpGet]
        public IActionResult CreateClaim()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateClaim(ClaimModel model)
        {
            if (ModelState.IsValid)
            {
                var applicationUser = await _userManager.GetUserAsync(HttpContext.User);
                Claim claim = new Claim(model.ClaimType, model.ClaimValue, ClaimValueTypes.String);
                var result = await _userManager.AddClaimAsync(applicationUser, claim);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Claims");
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }

            return View();
        }
    }
}
