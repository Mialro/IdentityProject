using IdentityProject.Data;
using IdentityProject.Models;
using IdentityProject.Repository;
using IdentityProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityProject.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ISendMailService _sendMailService;
        private readonly IOptions<IdentityOptions> _options;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, 
            ISendMailService sendMailService, IOptions<IdentityOptions> options, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _sendMailService = sendMailService;
            _options = options;
            _roleManager = roleManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> RegisterUser()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }
            List<string> listRole = new List<string>();
            var roles = _roleManager.Roles.ToList();
            foreach (var item in roles)
            {
                listRole.Add(item.Name);
            }

            ViewBag.AllRoles = listRole;
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

                #region Handle if the user doesn't select a role
                /*if (model.Role == null)
                {
                    model.Role = "User";
                }
                var x = await _roleManager.FindByNameAsync(model.Role);
                var y = x.Name;*/
                #endregion


                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);

                    if (model.Role != null && model.Role == "Admin")
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }
                    else
                    {
                        model.Role = "User";
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

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

                    return RedirectToAction("Index", "Home", new { isSuccess = ViewBag.IsSuccess, role = model.Role });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }

            List<string> listRole = new List<string>();
            var roles = _roleManager.Roles.ToList();
            foreach (var item in roles)
            {
                listRole.Add(item.Name);
            }

            ViewBag.AllRoles = listRole;
            return View(model);
        }

       
        [AllowAnonymous]
        public IActionResult LogIn(string ReturnUrl=null)
        {
            ViewBag.returnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel model, string ReturnUrl = null)
        {
            //ViewBag.returnUrl = ReturnUrl;
            var x = Url.Content("/");
            ReturnUrl = ReturnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid Credentials");
                    return View(model);
                }
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
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


        [AllowAnonymous]
        [HttpPost]
        public IActionResult LogOutUser()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [AllowAnonymous]
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


        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        [AllowAnonymous]
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


        [AllowAnonymous]
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string ReturnUrl = null)
        {
            //provider = "Facebook";
            var callbackUrl = Url.Action("ExternalLoginCallback", "Account", new { urlBack = ReturnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, callbackUrl);

            return Challenge(properties, provider);
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl=null, string remoteError=null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError("", "Error from External provider: " + remoteError);
                return RedirectToAction("LogIn", "Account");
            }

            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (loginInfo==null)
            {
                return RedirectToAction("LogIn", "Account");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                await _signInManager.UpdateExternalAuthenticationTokensAsync(loginInfo);
                return LocalRedirect(returnUrl);
            }
            else
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.ProviderName = loginInfo.ProviderDisplayName;
                var email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
                var name = loginInfo.Principal.FindFirstValue(ClaimTypes.Name);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationModel() { Email=email, Name=name});
            }
        }


        [HttpPost]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationModel model, string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~");
            if (ModelState.IsValid)
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("Error");
                }

                var user = new ApplicationUser()
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Name = model.Name
                };

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }


        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
