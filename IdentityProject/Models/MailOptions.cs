using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityProject.Models
{
    public class MailOptions
    {
        public List<string> MailTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
