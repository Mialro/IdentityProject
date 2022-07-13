using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityProject.Models
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage ="You Must Enter Your Email Address")]
        [Display(Name ="Email Address"), DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
