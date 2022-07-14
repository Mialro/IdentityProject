using IdentityProject.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IdentityProject.Services
{
    public class SendMailService : ISendMailService
    {
        private readonly IOptions<STMPConfigModel> _configOptions;

        public SendMailService(IOptions<STMPConfigModel> configOptions)
        {
            _configOptions = configOptions;
        }

        string templatePath = @"Templates/{0}.html";
        public async Task SendEmail(MailOptions mailOptions, string templateToUse)
        {
            //var getBodyContent = GetBody("bodyTemplate");
            var getBodyContent = GetBody(templateToUse);
            mailOptions.Body = UpdatePlaceHolder(getBodyContent, mailOptions.PlaceHolder);

            MailMessage mailMessage = new MailMessage()
            {
                Body = mailOptions.Body,
                From = new MailAddress(_configOptions.Value.SenderAddress, _configOptions.Value.SenderDisplayName),
                IsBodyHtml = _configOptions.Value.IsBodyHtml,
                BodyEncoding = Encoding.Default,
                Subject = mailOptions.Subject,
            };

            foreach (var emaiTo in mailOptions.MailTo)
            {
                mailMessage.To.Add(emaiTo);
            }

            SmtpClient smtpClient = new SmtpClient()
            {
                Port = _configOptions.Value.Port,
                Host = _configOptions.Value.Host,
                EnableSsl = _configOptions.Value.EnableSSl,
                UseDefaultCredentials = _configOptions.Value.UseDefaultCredentials,
                Credentials = new NetworkCredential(_configOptions.Value.UserName, _configOptions.Value.Password),
            };

            await smtpClient.SendMailAsync(mailMessage);
        }

        private string GetBody(string templateName)
        {
            var body = File.ReadAllText(string.Format(templatePath, templateName));
            return body;
        }

        private string UpdatePlaceHolder(string text, List<KeyValuePair<string, string>> keyValuePairs)
        {
            if(!string.IsNullOrEmpty(text) && keyValuePairs != null)
            {
                foreach (var item in keyValuePairs)
                {
                    if (text.Contains(item.Key))
                    {
                        text = text.Replace(item.Key, item.Value);
                    }
                }
            }
            return text;
        }
    }
}
