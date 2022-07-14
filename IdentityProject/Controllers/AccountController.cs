using IdentityProject.Data;
using IdentityProject.Models;
using IdentityProject.Repository;
using IdentityProject.Services;
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
        private readonly ISendMailService _sendMailService;

        public AccountController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ISendMailService sendMailService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _sendMailService = sendMailService;
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
                    //await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var urlEmail = Url.Action("ConfirmEmailUser", "Account", new { userId = user.Id, token = token });

                    MailOptions mailOptions = new MailOptions()
                    {
                        MailTo = new List<string>() { user.Email },
                        Subject = "Confirm Your Email",
                        PlaceHolder = new List<KeyValuePair<string, string>>()
                        {
                            new KeyValuePair<string, string>("{{mylink}}", urlEmail)
                        }
                    };

                    await _sendMailService.SendEmail(mailOptions, "confirmMailTemplate");

                    ModelState.Clear();

                    ViewBag.IsSuccess = true;

                    return RedirectToAction("Index", "Home", new { isSuccess = ViewBag.IsSuccess });
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
                var result = await _signInManager.PasswordSignInAsync(await _userManager.FindByEmailAsync(model.Email), model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return LocalRedirect(ReturnUrl);
                }
                ModelState.AddModelError("", "Invalid Credentials");
                /*var user = await _userManager.FindByNameAsync(model.Email);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                    return LocalRedirect(ReturnUrl);
                }

                ModelState.AddModelError("", "Invalid Credentials");*/
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
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPasswordUser", "Account", new { userId = user.Id, token = token });

                MailOptions mailOptions = new MailOptions()
                {
                    MailTo = new List<string>() { model.Email },
                    Subject = "Sending Test",
                    PlaceHolder = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("{{mylink}}", callbackUrl) }
                };

                await _sendMailService.SendEmail(mailOptions, "resetPasswordTemplate");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            return View(model);

            #region For Tests

            /*var user = await _userManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                var result = await _userManager.GeneratePasswordResetTokenAsync(user);
                return View("ResetPasswordConfirmation");
            }
            ModelState.AddModelError("", "Invalid Password");*/

            /*if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPasswordUser", "Account", new { userId = user.Id, code = code });

                MailOptions mailOptions = new MailOptions()
                {
                    MailTo = new List<string>() { model.Email },
                    Subject = "Sending Test",
                    PlaceHolder = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("{{mylink}}", callbackUrl) }
                };

                await _sendMailService.SendEmail(mailOptions);
            }

            ModelState.Clear();
            return View();*/
            #endregion

        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPasswordUser(string userId, string token)
        {
            ResetPasswordModel resetPasswordModel = new ResetPasswordModel()
            {
                userId = userId,
                Token = token
            };
            return View(resetPasswordModel);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordUser(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.ResetPasswordAsync(await _userManager.FindByIdAsync(model.userId), model.Token, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmEmailUser(string userId, string token)
        {
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(token))
            {
                await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(userId), token);
                ViewBag.IsSuccess = true;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmailUser(ConfirmEmailModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var urlEmail = Url.Action("ConfirmEmailUser", "Account", new { userId = user.Id, token = token });

                    MailOptions mailOptions = new MailOptions()
                    {
                        MailTo = new List<string>() { user.Email },
                        Subject = "Confirm Your Email",
                        PlaceHolder = new List<KeyValuePair<string, string>>()
                        {
                            new KeyValuePair<string, string>("{{mylink}}", urlEmail)
                        }
                    };

                    await _sendMailService.SendEmail(mailOptions, "confirmMailTemplate");

                    ViewBag.IsSuccess = true;

                    return RedirectToAction("Index", "Home", new { isSuccess = ViewBag.IsSuccess });
                }

                ModelState.AddModelError("", "There is no Account with this Email. Please create an Account");
            }
            return View(model);
        }
    }
}
