using IntegrationMaker.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace IntegrationMaker.BusinessLogic.TaskConfiguration
{
    public abstract class ProvisioningTask
    {
        public Job Job { get; set; }
        public ProvisioningTask(Job job)
        {
            this.Job = job;
        }

        public abstract Task<ProvisioningResult> ExecuteJobAsync();
        public abstract Task StartAsync();
        public abstract Task CompleteAsync(string message = "");
        public abstract Task FailAsync(string errormessage);
        public abstract Task FailAsync(string errormessage, ErpIntegrationErrorCodes errorCode);

        public async Task SendMailNotificationAsync(string subject, string body)
        {
            //Send the mail
            var msg = new MailMessage();
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = true;
            msg.To.Add(new MailAddress("schadepedersen@gmail.com"));
            

            //Functions.SendMail(msg);
        }

        public async Task NotifyFreeYourNumbersThatDataIsImportedAtFirstRunAsync(string subject, string body)
        {
            //Send the mail
            var msg = new MailMessage();
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = true;
            //msg.To.Add(new MailAddress("support@toolpack.net")); //TODO: Add supplied email param here
            //msg.To.Add(new MailAddress("marketing@toolpack.net")); //TODO: Add supplied email param here

            //msg.Bcc.Add(new MailAddress("msp@capworks.eu"));//TODO: remove or change CC address
            //Functions.SendMail(msg);
        }

        public async Task NotifySubscriberAsync(string subject, string body, string email)
        {
            //Send the mail
            var msg = new MailMessage();
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = true;
            msg.To.Add(new MailAddress(email)); //TODO: Add supplied email param here
       
            //msg.Bcc.Add(new MailAddress("msp@capworks.eu"));//TODO: remove or change CC address
            //Functions.SendMail(msg);
        }
    }
}
