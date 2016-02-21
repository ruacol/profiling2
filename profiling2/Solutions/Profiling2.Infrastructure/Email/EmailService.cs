using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using log4net;
using Profiling2.Domain.Contracts.Services;

namespace Profiling2.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        protected readonly static ILog log = LogManager.GetLogger(typeof(EmailService));

        public EmailService() { }

        /// <summary>
        /// We keep a static copy of SmtpClient in order to maintain an open connection with the
        /// SMTP server.
        /// </summary>
        private static SmtpClient _smtpClient;
        protected static SmtpClient SmtpClient
        {
            get
            {
                if (_smtpClient == null)
                {
                    // blank constructor gets settings from Web.config system.net/mailSettings/smtp.
                    _smtpClient = new SmtpClient();
                }
                return _smtpClient;
            }
        }

        /// <summary>
        /// Low level method - caller should check inputs.
        /// </summary>
        /// <param name="recipients"></param>
        /// <param name="from">Sender's email address - falls back on Web.config parameter if empty.</param>
        /// <param name="subject"></param>
        /// <param name="body">Contents of email - if HTML, will be wrapped with body and html tags.</param>
        /// <param name="isHtml">Set ContentType to text/html.</param>
        public void SendMailMessage(string[] recipients, string from, string subject, string body, bool isHtml)
        {
            using (MailMessage m = new MailMessage())
            {
                if (string.IsNullOrEmpty(from))
                {
                    // use Web.config 'from' address
                    var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                    if (smtpSection != null && !string.IsNullOrEmpty(smtpSection.From))
                    {
                        m.From = new MailAddress(smtpSection.From);
                    }
                }
                else
                {
                    m.From = new MailAddress(from);
                }
                foreach (string recipient in recipients.Distinct().OrderBy(x => x))
                    if (!string.IsNullOrEmpty(recipient))
                        m.To.Add(recipient);
                m.Subject = subject;
                m.Body = body;

                if (isHtml)
                {
                    string htmlBody = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">";
                    htmlBody += "<HTML><HEAD><META http-equiv=Content-Type content=\"text/html; charset=utf-8\">";
                    htmlBody += "</HEAD><BODY>";
                    htmlBody += body;
                    htmlBody += "</BODY></HTML>";

                    AlternateView alternate = AlternateView.CreateAlternateViewFromString(htmlBody, new ContentType("text/html"));
                    m.AlternateViews.Add(alternate);
                }

                log.Debug("Sending email with subject: " + m.Subject);
                log.Debug("From: " + m.From + ", to: " + string.Join(", ", m.To));

                if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.IsLocal)
                {
                    log.Info("Not sending email because request is local.");
                }
                else
                {
                    lock (SmtpClient)
                    {
                        // We lock when sending mail, otherwise we are vulnerable to InvalidOperationException if called again 
                        // while another thread is still communicating with SMTP server: 
                        // http://stackoverflow.com/questions/20129933/sendmailasync-use-in-mvc?
                        SmtpClient.Send(m);
                    }
                }
            }
        }
    }
}
