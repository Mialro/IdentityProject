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
    public class RolesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(UserManager<IdentityUser> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }
        public IActionResult Index(bool isSuccess)
        {
            ViewBag.IsSuccess = isSuccess;
            var rolesList = _context.Roles.ToList();
            return View(rolesList);
        }


        public IActionResult CreateRole()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateRole(IdentityRole model)
        {
            if (ModelState.IsValid)
            {
                if (await _roleManager.RoleExistsAsync(model.Name))
                {
                    ModelState.AddModelError("", "There is an Existing Role");
                    return View(model);
                }
                var role = new IdentityRole(model.Name);
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    ViewBag.IsSuccess = true;
                    return RedirectToAction("Index", "Roles", new { isSuccess = ViewBag.IsSuccess });
                }
            }
            return View(model);
        }


        public IActionResult UpdateRole(string id)
        {
            var role = _context.Roles.FirstOrDefault(x => x.Id == id);
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(IdentityRole model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("", "You Must Enter a Role");
                return View(model);
            }
            else
            {
                var role = _context.Roles.FirstOrDefault(x => x.Id == model.Id);
                role.Name = model.Name;
                await _roleManager.UpdateAsync(role);
                role.NormalizedName = model.Name.ToUpper();
                return RedirectToAction("Index", "Roles");
            }
        }


        public IActionResult DeleteRole(string id)
        {
            var role = _context.Roles.FirstOrDefault(x => x.Id == id);
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(IdentityRole model)
        {
            var role = _context.Roles.FirstOrDefault(x => x.Id == model.Id);
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Roles");
            }
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }

            return View(model);
        }


    }
}
