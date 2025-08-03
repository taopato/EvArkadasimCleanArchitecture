// Persistence/Services/MailService.cs
using Core.Interfaces;
using Domain.Entities;
using Persistence.Contexts;
using System.Net;
using System.Net.Mail;

namespace Persistence.Services
{
    public class MailService : IMailService
    {
        private readonly AppDbContext _context;
        private const string SmtpServer = "smtp.gmail.com";
        private const int SmtpPort = 587;
        private const string SenderEmail = "info.ev.arkadasim@gmail.com";
        private const string SenderPassword = "lujs brjg mojt tuze";

        public MailService(AppDbContext context)
        {
            _context = context;
        }

        public void SendVerificationCode(string toEmail, string code)
        {
            // 1) DB’ye ekle/güncelle
            var existing = _context.VerificationCodes.FirstOrDefault(vc => vc.Email == toEmail);
            if (existing != null)
            {
                existing.Code = code;
                existing.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.VerificationCodes.Add(new VerificationCode
                {
                    Email = toEmail,
                    Code = code,
                    CreatedAt = DateTime.UtcNow
                });
            }
            _context.SaveChanges();

            // 2) SMTP ile mail at
            using var client = new SmtpClient(SmtpServer, SmtpPort)
            {
                Credentials = new NetworkCredential(SenderEmail, SenderPassword),
                EnableSsl = true
            };

            var msg = new MailMessage
            {
                From = new MailAddress(SenderEmail),
                Subject = "EvArkadaşım Doğrulama Kodu",
                Body = $"Doğrulama kodunuz: <b>{code}</b>",
                IsBodyHtml = true
            };
            msg.To.Add(toEmail);

            client.Send(msg);
        }
    }
}
