using IdentityProject.Data;
using IdentityProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityProject.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<IdentityUser> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var userList = _context.ApplicationUser.ToList();
            var userRoles = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();

            foreach (var user in userList)
            {
                var role = userRoles.FirstOrDefault(x => x.UserId == user.Id);
                if (role == null)
                {
                    user.Role = "None Role";
                }
                else
                {
                    user.Role = roles.FirstOrDefault(x => x.Id == role.RoleId).Name;

                }
            }
            return View(userList);
        }


        [HttpGet]
        public IActionResult UpdateUser(string UserId)
        {
            List<string> listRole = new List<string>();
            var roles = _context.Roles.ToList();
            foreach (var item in roles)
            {
                listRole.Add(item.Name);
            }

            ViewBag.AllRoles = listRole;


            var applicationUser = _context.ApplicationUser.FirstOrDefault(x => x.Id == UserId);

            return View(applicationUser);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateUser(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                var applicationUser = _context.ApplicationUser.FirstOrDefault(x => x.Id == model.Id);
                // If the user exists
                if (applicationUser != null)
                {
                    var userRole = _context.UserRoles.FirstOrDefault(x => x.UserId == applicationUser.Id);
                    // If the user already has a role
                    if (userRole != null)
                    {
                        var removeRole = await _userManager.RemoveFromRoleAsync(applicationUser, (await _roleManager.FindByIdAsync(userRole.RoleId)).Name);
                        if (removeRole.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(applicationUser, model.Role);
                        }
                    }
                    // If the user has no role
                    else
                    {
                        await _userManager.AddToRoleAsync(applicationUser, model.Role);
                    }
                    applicationUser.Name = model.Name;
                    var result = await _userManager.UpdateAsync(applicationUser);
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    return NotFound();
                }

            }

            List<string> listRole = new List<string>();
            var roles = _context.Roles.ToList();
            foreach (var item in roles)
            {
                listRole.Add(item.Name);
            }

            ViewBag.AllRoles = listRole;
            return View(model);
        }



        [HttpGet]
        public IActionResult DeleteUser(string UserId)
        {
            var applicationUser = _context.ApplicationUser.FirstOrDefault(x => x.Id == UserId);

            return View(applicationUser);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(ApplicationUser model)
        {
            var applicationUser = _context.ApplicationUser.FirstOrDefault(x => x.Id == model.Id);
            if (applicationUser != null)
            {
                var result = await _userManager.DeleteAsync(applicationUser);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "User");
                }
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> LockUnlockUser(string UserId)
        {
            var applicationUser = _context.ApplicationUser.FirstOrDefault(x => x.Id == UserId);
            if (applicationUser != null)
            {
                if (applicationUser.LockoutEnd == null )
                {
                    applicationUser.LockoutEnd = DateTime.Now.AddMinutes(2);
                }
                else
                {
                    applicationUser.LockoutEnd = DateTime.Now;
                }
            }

            await _userManager.UpdateAsync(applicationUser);
            return RedirectToAction("Index", "User");
        }
    }
}
