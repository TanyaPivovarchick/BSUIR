using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RSSReader.BL
{
    public class EmailMessage
    {
        private string FromEmail
        {
            get
            {
                return "special.for.bsuir@gmail.com";
            }
        }

        private string Password
        {
            get
            {
                return "SppLaba05.10";
            }
        }

        public string ToEmail { get; set; }

        private string Subject
        {
            get
            {
                return "Лабораторная работа по СПП";
            }
        }

        private string Body
        {
            get
            {
                // Getting the name of a file containing a text message.
                String filename = Environment.CurrentDirectory;
                int index = filename.IndexOf("RSSReader.UI");
                filename = filename.Remove(index) + "data.txt";

                String str = "";

                String[] readText = File.ReadAllLines(filename, Encoding.Unicode);
                foreach (String s in readText)
                {
                    str += s + "<br>";
                }

                return str;
            }
        }

        public async Task SendEmailAsync()
        {
            MailAddress from = new MailAddress(FromEmail);
            MailAddress to = new MailAddress(ToEmail);

            MailMessage message = new MailMessage(from, to);
            message.Subject = Subject;
            message.Body = Body;
            message.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new System.Net.NetworkCredential(FromEmail, Password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;
            await smtp.SendMailAsync(message);
        }
    }
}
