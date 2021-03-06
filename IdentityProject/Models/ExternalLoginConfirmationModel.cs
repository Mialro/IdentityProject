using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityProject.Models
{
    public class ExternalLoginConfirmationModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
