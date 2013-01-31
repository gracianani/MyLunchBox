using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;

namespace MyLunchBox.Utilities
{
    public class MyLunchBoxEmailTemplateCollector
    {
        
        public static string DirectoryAppSettingsKey = "EmailTemplateDirectoryPath";

        public EmailTemplate FetchTemplate(string templateName)
        {
            string directory = ConfigurationManager.AppSettings[DirectoryAppSettingsKey];
            string fileName = ConfigurationManager.AppSettings[templateName + "Body"];
            string file = directory + fileName;

            if (!string.IsNullOrEmpty(directory) && !string.IsNullOrEmpty(fileName) && File.Exists(file))
            {
                string bodyText = File.ReadAllText(file);

                if (!string.IsNullOrEmpty(bodyText))
                {
                    string subject = ConfigurationManager.AppSettings[templateName + "Subject"];
                    if (string.IsNullOrEmpty(subject))
                    {
                        subject = string.Empty;
                    }
                    return new EmailTemplate { Message = bodyText, Subject = subject };
                }

            }
            return null;
        }
    }

    
}