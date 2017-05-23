namespace SurveySystem.Services.Web
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using System.Web.Hosting;

    public class EmailService : IEmailService
    {
        private static readonly string Username;
        private static readonly string Password;
        private static readonly string InvitationTemplate;

        static EmailService()
        {
            Username = ConfigurationManager.AppSettings["Email"];
            Password = ConfigurationManager.AppSettings["Password"];

            InvitationTemplate = File.ReadAllText(HostingEnvironment.MapPath("~/App_Data/InvitationTemplate.txt"));
        }

        public void SendNewReservationEmail(string email, string surveyTitle, string surveyUrl)
        {
            var subject = "Покана за попълване на анкета";
            var body = string.Format(InvitationTemplate, surveyUrl, surveyTitle);

            Task.Run(() => Send(email, subject, body));
        }

        private static void Send(string receiver, string subject, string body)
        {
            try
            {
                var msg = new MailMessage
                {
                    From = new MailAddress(Username),
                    IsBodyHtml = true,
                    Subject = subject,
                    Body = body
                };

                msg.To.Add(receiver);

                using (var client = new SmtpClient())
                {
                    client.Credentials = new NetworkCredential(Username, Password);
                    client.Host = "smtp.gmail.com";
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(Username, Password);
                    client.Timeout = 20000;

                    // workaround for NBU's mail server - who needs security anyway?
                    // ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                    client.Send(msg);
                }
            }
            catch (Exception)
            {
                // TODO: log this!
            }
        }
    }
}
