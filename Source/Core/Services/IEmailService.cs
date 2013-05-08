// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEmailService.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.Services
{
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Reflection;

    using RazorEngine;
    using RazorEngine.Configuration;
    using RazorEngine.Templating;

    using SendGrid;
    using SendGrid.Transport;

    public interface IEmailService
    {
        void Send<T>(string to, string templateName, string subject, T data);
    }

    public class EmailService : IEmailService
    {
        private readonly NetworkCredential credentials;

        public EmailService(NetworkCredential credentials)
        {
            Razor.SetTemplateService(new TemplateService(new TemplateServiceConfiguration
            {
                Resolver = new DelegateTemplateResolver(name =>
                {
                    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("App.EmailTemplates.{0}.cshtml", name)))
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                })
            }));

            this.credentials = credentials;
        }

        public void Send<T>(string to, string templateName, string subject, T data)
        {
            var mail = Mail.GetInstance();
            var transportSMTP = SMTP.GetInstance(this.credentials);
            var template = Razor.Resolve<T>(templateName, data);
            var context = new ExecuteContext();
            var message = template.Run(context);

            mail.From = new MailAddress("noreply@example.com");
            mail.Subject = subject;
            mail.AddTo(to);
            mail.Html = message;

            transportSMTP.Deliver(mail);
        }
    }
}
