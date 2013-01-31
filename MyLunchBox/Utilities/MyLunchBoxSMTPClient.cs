using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Configuration;
using System.Net;

namespace MyLunchBox.Utilities
{
    public class MyLunchBoxSMTPClient : SmtpClient
    {
        public MyLunchBoxSMTPClient()
            : base()
        {
            EnableSsl = true;
            Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SmtpUserName"], ConfigurationManager.AppSettings["SmtpPassword"]);
        }
    }
}