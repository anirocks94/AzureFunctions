using System;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SendEmailCust
{
    public static class Regmail
    {
        [FunctionName("Regmail")]
        public static void Run([QueueTrigger("userdata", Connection = "AzureWebJobsStorage")]string myQueueItem, TraceWriter log, Microsoft.Azure.WebJobs.ExecutionContext context)
        {

            log.Info($"C# Queue trigger function processed: {myQueueItem}");
            var config = new ConfigurationBuilder()
       .SetBasePath(context.FunctionAppDirectory)
       .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
       .AddEnvironmentVariables()
       .Build();

            byte[] data = Convert.FromBase64String(myQueueItem);
            string decodedString = Encoding.UTF8.GetString(data);
            string username = decodedString.Split(' ')[0].ToString();
            string emailaddress = decodedString.Split(' ')[1].ToString();
            string fromEmail = config["SmtpUser"];
            
            string toEmail = emailaddress;
            int smtpPort = Int32.Parse(config["SmtpPort"]);
            bool smtpEnableSsl = true;
            string smtpHost = config["SmtpHost"]; // your smtp host
            string smtpUser = config["SmtpUser"]; // your smtp user
            string smtpPass = config["SmtpPassword"]; // your smtp password
            string subject = config["EmailSubject"];
            // string message = "Hi/n"+username+"Thanks for registering with us /n Thanks, /n Anirban";
            StringBuilder message = new StringBuilder();

            message.AppendFormat("Dear {0},", username);
            message.AppendFormat("\n");
            message.AppendFormat("Thanks for registering with us.Please visit the below links for offers");
            message.AppendFormat("\n");
            message.AppendFormat("https://rentogameui.azurewebsites.net/Ps4");
            message.AppendFormat("\n");
            message.AppendFormat("Regards,");
            message.AppendFormat("\n");
            message.AppendFormat("Team RentOGame");




            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient();
            client.Port = smtpPort;
            client.EnableSsl = smtpEnableSsl;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = smtpHost;
            client.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPass);
            mail.Subject = subject;
            mail.From = new MailAddress(fromEmail);
            mail.Body = message.ToString();
            mail.To.Add(new MailAddress(toEmail));
            try
            {
                client.Send(mail);
                log.Verbose("Email sent");
            }
            catch (Exception ex)
            {
                log.Verbose(ex.ToString());
            }

        }
    }
}
