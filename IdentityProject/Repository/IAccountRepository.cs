using IdentityProject.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityProject.Repository
{
    public interface IAccountRepository
    {
        public Task<IdentityResult> RegisterAsync(ApplicationUser user);
        public Task<SignInResult> LogInAsync(RegisterViewModel model);
    }
}
