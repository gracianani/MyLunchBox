using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.Mime;

namespace MyLunchBox.Utilities
{
    public class MyLunchBoxMailer
    {
        private static string _startDelimiter = "[[";
        private static string _endDelimiter = "]]";
        private static string ExecuteReplacements(string templateText, Dictionary<string, string> replacements)
        {
            return ExecuteReplacements(templateText, replacements, false);
        }
        private Dictionary<string, string> _customHeaders = new Dictionary<string, string>();
        public void SetCustomHeader(string headerName, string headerValue)
        {
            if (_customHeaders.ContainsKey(headerName))
            {
                _customHeaders[headerName] = headerValue;
            }
            else
            {
                //add key and value if not exist
                _customHeaders.Add(headerName, headerValue);
            }
        }
        private MyLunchBoxEmailTemplateCollector _collector;
        public void ClearCustomHeaders()
        {
            _customHeaders.Clear();
        }
        public MyLunchBoxMailer()
        {
            _collector = new MyLunchBoxEmailTemplateCollector();
        }
        public MyLunchBoxMailer(string startDelimiter, string endDelimiter)
            : this()
        {
            _startDelimiter = startDelimiter;
            _endDelimiter = endDelimiter;
        }
        private static string ExecuteReplacements(string templateText, Dictionary<string, string> replacements, bool removeExtraTemplateKeys)
        {

            foreach (KeyValuePair<string, string> replaceMe in replacements)
            {
                templateText = templateText.Replace(_startDelimiter + replaceMe.Key + _endDelimiter, replaceMe.Value);
            }
            if (removeExtraTemplateKeys)
            {
                Regex rg = new Regex(@"\[\[.*\]\]");
                templateText = rg.Replace(templateText, "");
            }
            return templateText;
        }

        public bool SendTemplateMail(Dictionary<string, string> replacements, string templateName, string toAddress, string fromAddress, string replyToAddress, string bccAddresses, SmtpClient client)
        {
            var emailTemplate = _collector.FetchTemplate(templateName);
            return SendTemplateMail(replacements, toAddress, fromAddress, replyToAddress, "", emailTemplate.Subject, emailTemplate.Message, client);
        }
        protected bool SendTemplateMail(Dictionary<string, string> replacements, string toAddress, string fromAddress, string replyToAddress, string bccAddresses,
                string subject, string bodyText, SmtpClient client)
        {

            subject = ExecuteReplacements(subject, replacements);
            bodyText = ExecuteReplacements(bodyText, replacements);
            bodyText = ExecuteReplacements(bodyText, replacements, true); //we do this twice to get any nested template fields!

            MailMessage mm;
            mm = new MailMessage(fromAddress, toAddress, subject, bodyText);
            mm.IsBodyHtml = true;

            if (!string.IsNullOrEmpty(replyToAddress))
            {
                mm.ReplyToList.Add( new MailAddress(replyToAddress) );
            }
            //add custom headers where appropriate
            foreach (KeyValuePair<string, string> headerNameAndValue in _customHeaders)
            {
                mm.Headers.Add(headerNameAndValue.Key, headerNameAndValue.Value);
            }
            try
            {
                client.Send(mm);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;

        }
    }
}