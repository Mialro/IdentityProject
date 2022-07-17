using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityProject.Models
{
    public class RoleModel
    {
        [Required(ErrorMessage ="Enter A Role Name"), Display(Name ="Role name")]
        public string Name { get; set; }
    }
}
