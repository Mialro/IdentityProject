using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage ="The Name is Required")]
        public string Name { get; set; }

        [NotMapped]
        public string Role { get; set; }

        [NotMapped]
        public string RoleId { get; set; }
    }
}
