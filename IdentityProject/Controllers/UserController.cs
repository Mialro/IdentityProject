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

        public UserController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
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
    }
}
