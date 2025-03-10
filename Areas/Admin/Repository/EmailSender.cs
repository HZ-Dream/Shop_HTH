﻿using System.Net;
using System.Net.Mail;

namespace Shopping_HTH.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("mancitysgp@gmail.com", "yxfgtjxwcdaesdor")
            };

            return client.SendMailAsync(
                new MailMessage(from: "mancitysgp@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}