using IdentityProject.Data;
using IdentityProject.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityProject.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }



        public async Task<IdentityResult> RegisterAsync(ApplicationUser user)
        {
            var result = await _userManager.CreateAsync(user);
            return result;
        }


        public async Task<SignInResult> LogInAsync(RegisterViewModel model)
        {
            var applicationUser = new ApplicationUser()
            {
                Name = model.Name,
                Email = model.Email,
                UserName = model.Email
            };
            var res = await _signInManager.PasswordSignInAsync(applicationUser, model.Password, isPersistent: false, lockoutOnFailure: false);

            return res;
        }
    }
}
