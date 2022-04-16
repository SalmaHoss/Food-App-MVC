using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FoodApp.Models
{
    public class SendMail:IEmailSender
    {
        public async Task SendEmailAsync(string email,string subject,string htmlMessage)
        {
            using(MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(email);

                mailMessage.Subject = subject;
                mailMessage.Body = email + htmlMessage;
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add(new MailAddress("salmahossam639@gmail.com"));

                SmtpClient smtp = new("smtp.gmail.com", 587);
                smtp.UseDefaultCredentials = false;

                smtp.EnableSsl = true;
                //Enter email and password to recieve message at
                System.Net.NetworkCredential networkCredential = new System.Net.NetworkCredential("","");
              
                smtp.Credentials = networkCredential;
                await smtp.SendMailAsync(mailMessage);
            }
        }
    }
}
