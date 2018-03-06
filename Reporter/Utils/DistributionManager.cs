using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Reporter.Data.Services;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace Reporter.Utils
{
    public static class DistributionManager
    {
        public static void SendReport(string attachment, string env, string db)
        {
            var to = PersonService.GetAll().Select(person => person.EmailAddress).ToList();
            var body = $@"Hello, 
Attached the daily batches summary report for:
Date: {DateTime.Today:D}
Environment: {env}
DB: {db}";
            SendEmailFromAccount(
                new Outlook.Application(),
                $@"Daily Report - {env}, {db} - {
                    DateTime.Today:D}",
                to,
                body,
                "roni.axelrad@sapiens.com",
                attachment);
            MessageBox.Show(@"Mail Sent!");
        }

        public static void SendEmailFromAccount(Outlook.Application application, string subject, List<string> to, string body, string smtpAddress, string attachment)
        {

            // Create a new MailItem and set the To, Subject, and Body properties.
            Outlook.MailItem newMail = (Outlook.MailItem)application.CreateItem(Outlook.OlItemType.olMailItem);
            string toStr = to.Aggregate<string, string>(null, (current, rec) => current + rec + ";");
            newMail.To = toStr;
            newMail.Subject = subject;
            newMail.Body = body;
            newMail.Attachments.Add(attachment, Outlook.OlAttachmentType.olByValue, Type.Missing, Type.Missing);

            // Retrieve the account that has the specific SMTP address.
            Outlook.Account account = GetAccountForEmailAddress(application, smtpAddress);
            // Use this account to send the e-mail.
            newMail.SendUsingAccount = account;
            newMail.Send();
        }
        public static Outlook.Account GetAccountForEmailAddress(Outlook.Application application, string smtpAddress)
        {

            // Loop over the Accounts collection of the current Outlook session.
            Outlook.Accounts accounts = application.Session.Accounts;
            foreach (Outlook.Account account in accounts)
            {
                // When the e-mail address matches, return the account.
                if (account.SmtpAddress == smtpAddress)
                {
                    return account;
                }
            }
            throw new Exception($"No Account with SmtpAddress: {smtpAddress} exists!");
        }
    }
}
