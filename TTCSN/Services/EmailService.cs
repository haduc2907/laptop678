
using MimeKit;
using MailKit.Net.Smtp;

namespace TTCSN.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration config;
        public EmailService(IConfiguration config)
        {
            this.config = config;
        }

        public async Task SendOtpEmailAsync(string toEmail, string otp)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Your App", config["Email:Username"]));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Mã OTP xác thực";

            message.Body = new TextPart("html")
            {
                Text = $@"
                <h2>Mã OTP của bạn</h2>
                <p>Mã OTP: <strong>{otp}</strong></p>
                <p>Mã có hiệu lực trong 5 phút.</p>
            "
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(config["Email:Host"],
                int.Parse(config["Email:Port"]), true);
            await client.AuthenticateAsync(config["Email:Username"],
                config["Email:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
