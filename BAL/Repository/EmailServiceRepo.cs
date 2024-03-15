using BAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DAL.DataContext;
using DAL.ViewModels;

namespace BAL.Repository
{
    public class EmailServiceRepo : IEmailService
    {
        private readonly ApplicationDbContext _context;
        public EmailServiceRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public void SendEmailForPasswordReset(ForgotPasswordViewModel fvm,string ResetLink)
        {
            var smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("tatva.dotnet.rahulshah@outlook.com", "@08RahulTatvA"),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("tatva.dotnet.rahulshah@outlook.com"),
                Subject = "Subject",
                Body = "<h1>Hello , Good morning!!</h1><a href=\"" + ResetLink + "\" >Reset your password</a>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(fvm.email);
            smtpClient.Send(mailMessage);
        }

        public void SendEmailWithAttachments(int requestid,string path)
        {
            var smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("tatva.dotnet.rahulshah@outlook.com", "@08RahulTatvA"),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };
            //smtpClient.Send("tatva.dotnet.rahulshah@outlook.com", "rahul0810shah@gmail.com", "This is a trial email for smtpClient.", "this is token ->" + resetLink);
            var mailMessage = new MailMessage
            {
                From = new MailAddress("tatva.dotnet.rahulshah@outlook.com"),
                Subject = "Subject",
                Body = "<p> Hello, All selected attachments are listed below!!! </p> ",
                IsBodyHtml = true
            };
            var request = _context.Requestwisefiles.Where(r => r.Requestid == requestid && r.Isdeleted != true).ToList();
            for (int i = 0; i < request.Count; i++)
            {
                string filePath = "Content/" + request[i].Filename;
                string fullPath = Path.Combine(path, filePath);

                byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
                MemoryStream ms = new MemoryStream(fileBytes);
                mailMessage.Attachments.Add(new Attachment(ms, request[i].Filename));
            }

            var user = _context.Requests.FirstOrDefault(r => r.Requestid == requestid);

            mailMessage.To.Add(user.Email);
            smtpClient.Send(mailMessage);
        }

        public void SendAgreementLink(int requestid, string link,string email)
        {
            var smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("tatva.dotnet.rahulshah@outlook.com", "@08RahulTatvA"),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress("tatva.dotnet.rahulshah@outlook.com"),
                Subject = "Subject",
                Body = "<p>Hello, we have attached a link to the agreement that need to accepted before moving forward to the treatment procedure.</p><a href=\"" + link + "\" >Agreement Link</a>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);
            smtpClient.Send(mailMessage);
        }
    }
}
