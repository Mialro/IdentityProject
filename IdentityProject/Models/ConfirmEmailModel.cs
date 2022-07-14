using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityProject.Models
{
    public class ConfirmEmailModel
    {
        [Required(ErrorMessage ="You Must Enter your Email")]
        [Display(Name ="Enter Your Registered Email"), DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
