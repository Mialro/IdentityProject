using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityProject.Models
{
    public class ResetPasswordModel
    {
        public string userId { get; set; }
        public string Token { get; set; }

        [Required(ErrorMessage ="You Must Enter The New Password"), Display(Name ="New Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "You Must Confirm The New Password"), Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Password doesn't match")]
        public string ConfirmPassword { get; set; }
    }
}
