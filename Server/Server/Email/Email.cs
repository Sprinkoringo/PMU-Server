using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Server.Email
{
    class Email
    {
        /// <summary>
        /// Checks whether the given Email-Parameter is a valid E-Mail address.
        /// </summary>
        /// <param name="email">Parameter-string that contains an E-Mail address.</param>
        /// <returns>True, when Parameter-string is not null and 
        /// contains a valid E-Mail address; otherwise false.</returns>
        public static bool IsValidEmail(string email) {
            string MatchEmailPattern =
                  @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
           + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
                [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
           + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
                [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
           + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

            if (!string.IsNullOrEmpty(email)) {
                return Regex.IsMatch(email, MatchEmailPattern);
            } else {
                return false;
            }
        }

        public static void SendEmail(MailMessage message) {
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "mail.pmuniverse.net";
            smtp.EnableSsl = false;
            System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
            NetworkCred.UserName = "noreply@pmuniverse.net";
            NetworkCred.Password = "U75RO[1KhP&^6l#jhQ";
            smtp.Credentials = NetworkCred;

            smtp.Port = 26;//Specify your port No;

            smtp.Send(message);
        }

        public static void SendEmail(string to, string subject, string body) {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("noreply@pmuniverse.net");
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = false;

            SendEmail(mail);
        }
    }
}
