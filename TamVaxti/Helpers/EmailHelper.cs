using System.Net;
using System.Net.Mail;

namespace TamVaxti.Helpers
{
    public class EmailHelper
    {
        public static string Server { get; set; }
        public static int Port { get; set; }
        public static string SenderName { get; set; }
        public static string SenderEmail { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }
        public static bool EnableSsl { get; set; }

        public static void Configure(IConfiguration configuration)
        {
            Server = configuration["EmailSettings:Server"];
            Port = int.Parse(configuration["EmailSettings:Port"]);
            SenderEmail = configuration["EmailSettings:SenderEmail"];
            SenderName = configuration["EmailSettings:SenderName"];
            Username = configuration["EmailSettings:Username"];
            Password = configuration["EmailSettings:Password"];
        }
        public static Task SendEmailAsync(string recipientEmail, string recipientName, string subject, string message,string bannerImage)
        {

            var fromAddress = new MailAddress(SenderEmail, SenderName);
            var toAddress = new MailAddress(recipientEmail, recipientName);
            string fromPassword = Password;
            string sub = subject;
            string body = message;

            var smtp = new SmtpClient
            {
                Host = Server,
                Port = Port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 20000
            };

            using (var msg = new MailMessage(fromAddress, toAddress)
            {
                /*Subject = sub,
                Body = body,
                IsBodyHtml = true*/
            })
            {
                msg.Subject = sub;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");

                var imageBytes = System.IO.File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/assets/images/{bannerImage}");
                LinkedResource inlineImage = new LinkedResource(new MemoryStream(imageBytes), "image/jpeg");
                inlineImage.ContentId = "InlineImageId";
                htmlView.LinkedResources.Add(inlineImage);

                var imagefbBytes = System.IO.File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/img/icons/fb.png");
                LinkedResource inlinefbImage = new LinkedResource(new MemoryStream(imagefbBytes), "image/png");
                inlinefbImage.ContentId = "InlineImagefbId";
                htmlView.LinkedResources.Add(inlinefbImage);

                var imageinsBytes = System.IO.File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/img/icons/insta.png");
                LinkedResource inlineinsImage = new LinkedResource(new MemoryStream(imageinsBytes), "image/png");
                inlineinsImage.ContentId = "InlineImageinsId";
                htmlView.LinkedResources.Add(inlineinsImage);

                var imagetwBytes = System.IO.File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/img/icons/tw.png");
                LinkedResource inlinetwImage = new LinkedResource(new MemoryStream(imagetwBytes), "image/png");
                inlinetwImage.ContentId = "InlineImagetwId";
                htmlView.LinkedResources.Add(inlinetwImage);

                var imagegpBytes = System.IO.File.ReadAllBytes($"{Directory.GetCurrentDirectory()}/wwwroot/img/icons/fb.png");
                LinkedResource inlinegpImage = new LinkedResource(new MemoryStream(imagegpBytes), "image/png");
                inlinegpImage.ContentId = "InlineImagegpId";
                htmlView.LinkedResources.Add(inlinegpImage);

                msg.AlternateViews.Add(htmlView);
                smtp.Send(msg);
            }

            return Task.CompletedTask;

        }

        
    }
}
