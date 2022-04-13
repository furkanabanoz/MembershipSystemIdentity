using System.Net;
using System.Net.Mail;

namespace WebApplication3.Helpers
{
    public static class EmailConfirmation
    {
        public static void SendEmail(string link, string email)
        {


            SmtpClient smtpClient = new SmtpClient();
            smtpClient.UseDefaultCredentials = false;
            smtpClient.EnableSsl = true;
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential("furkanabanoz98@gmail.com", "learnmore1");

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("furkanabanoz98@gmail.com");
            mail.To.Add(email);
            mail.Subject = "www.bidibidi.com::Email Dogrulama";
            mail.Body = $"<h2>Emailinizi dogrulamak icin lutfen asagidaki linke tiklayiniz!</h2><hr/>";
            mail.Body += $"<a href='{link}'>Email dogrulama linki</a>";
            mail.IsBodyHtml = true;

            smtpClient.Send(mail);

        }
    }
}
