using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using Schadensapp.Services.Mail.Model;

namespace Schadensapp.Services.Mail
{
    public class MailService : IEmailSender
    {
        private readonly MailModel _mailSettings;
        public MailService(IOptions<MailModel> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            var builder = new BodyBuilder
            {
                HtmlBody = body
            };
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public async Task SendEmailAsyncWithAttachment(string toEmail, string subject, string body, PDFModel model)
        {
            MemoryStream memoryStream = new PDFService().GeneratePDF(model);

            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            var builder = new BodyBuilder
            {
                HtmlBody = body
            };

            // Erstellen des Anhangs für die PDF-Datei
            var attachment = new MimePart("application", "pdf")
            {
                Content = new MimeContent(memoryStream),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = "Schadensmeldung" + DateTime.Now.ToString()
            };

            var multipart = new Multipart("mixed");
            multipart.Add(builder.ToMessageBody());
            multipart.Add(attachment);
            email.Body = multipart;

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

    }
}

/*
 Die `MailService`-Klasse ist ein Implementierungsbeispiel des `IEmailSender`-Interfaces, das für das Senden von E-Mails in einer ASP.NET Core-Anwendung verwendet wird. Hier ist eine Erläuterung des Codes:

1. Der Konstruktor der `MailService`-Klasse erhält ein `IOptions<MailModel>`-Objekt, um die Mail-Einstellungen aus der Konfigurationsdatei zu erhalten.

2. Die Methode `SendEmailAsync` wird verwendet, um eine einfache E-Mail ohne Anhang zu senden. Es erstellt ein `MimeMessage`-Objekt und füllt es mit Absender, Empfänger, 
    Betreff und Nachrichtentext. Der SMTP-Client wird initialisiert und zur Übertragung verwendet, um die E-Mail zu senden.

3. Die Methode `SendEmailAsyncWithAttachment` wird verwendet, um eine E-Mail mit einem Anhang zu senden. Hier wird eine PDF-Datei als Anhang hinzugefügt. 
    Die PDF-Datei wird mithilfe des `PDFService` erstellt, der das `IGeneratePDF`-Interface implementiert. Der PDF-Inhalt wird als `MemoryStream` erstellt und in den Anhang eingefügt. 
    Ansonsten folgt der gleiche Ablauf wie bei der vorherigen Methode zum Senden der E-Mail.

Es ist wichtig zu beachten, dass die `MailService`-Klasse von `IEmailSender` abhängig ist, was bedeutet, dass sie in der Anwendung verwendet wird, indem sie das `IEmailSender`-Interface injiziert, 
anstatt eine direkte Instanz der `MailService`-Klasse zu verwenden. Dies ermöglicht die einfache Austauschbarkeit des E-Mail-Sende-Dienstes und erleichtert das Testen der Anwendung.
 */