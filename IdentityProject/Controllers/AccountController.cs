using IdentityProject.Data;
using IdentityProject.Models;
using IdentityProject.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult RegisterUser()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    Name = model.Name,
                    Email = model.Email,
                    UserName = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);

                    ModelState.Clear();

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View(model);
        }


        public IActionResult LogIn(string ReturnUrl=null)
        {
            ViewBag.returnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel model, string ReturnUrl = null)
        {
            //ViewBag.returnUrl = ReturnUrl;
            ReturnUrl = ReturnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                    return LocalRedirect(ReturnUrl);
                }

                ModelState.AddModelError("", "Invalid Credentials");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult LogOutUser()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult ForgotPasswordUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPasswordUser(ForgotPasswordModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                var result = await _userManager.GeneratePasswordResetTokenAsync(user);
                return View("ResetPasswordConfirmation");
            }
            ModelState.AddModelError("", "Invalid Password");
            return View(model);
        }
    }
}
