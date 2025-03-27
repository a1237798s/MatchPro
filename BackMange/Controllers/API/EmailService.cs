using System.Net.Mail;
using System.Net;

namespace BackMange.Controllers.API
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendVerificationEmailAsync(string email, string code)
        {
            try
            {
                using var smtpClient = new SmtpClient
                {
                    Host = _configuration["Email:SmtpHost"],
                    Port = int.Parse(_configuration["Email:SmtpPort"]),
                    EnableSsl = true,
                    Credentials = new NetworkCredential(
                        _configuration["Email:Username"],
                        _configuration["Email:Password"]
                    )
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["Email:FromAddress"]),
                    Subject = "驗證您的信箱",
                    Body = $"您的驗證碼是: {code}",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // 記錄錯誤，避免程式崩潰
                Console.WriteLine($"[錯誤] 郵件發送失敗: {ex.Message}");
                throw;
            }
        }
    }
}
