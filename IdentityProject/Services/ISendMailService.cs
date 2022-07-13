using IdentityProject.Models;
using System.Threading.Tasks;

namespace IdentityProject.Services
{
    public interface ISendMailService
    {
        Task SendEmail(MailOptions mailOptions);
    }
}