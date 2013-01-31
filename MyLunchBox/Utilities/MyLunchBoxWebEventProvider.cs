using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Reflection;
using System.Collections.Specialized;

namespace MyLunchBox.Utilities
{
    public class MyLunchBoxWebEventProvider : WebEventProvider
    {
        private SimpleMailWebEventProvider _simpleProvider;
        public MyLunchBoxWebEventProvider()
        {
            ConstructorInfo constructor = typeof(SimpleMailWebEventProvider)
                .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                                null, new Type[0], null);
            _simpleProvider = (SimpleMailWebEventProvider)constructor
                .Invoke(null);
        }
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            _simpleProvider.Initialize(name, config);

            FieldInfo field = typeof(MailWebEventProvider)
                .GetField("_smtpClient",
                            BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(_simpleProvider, new MyLunchBoxSMTPClient());
        }
        public static MailEventNotificationInfo CurrentNotification
        {
            get
            {
                return TemplatedMailWebEventProvider.CurrentNotification;
            }
        }
        public override void Flush()
        {
            _simpleProvider.Flush();
        }
        public override void ProcessEvent(WebBaseEvent raisedEvent)
        {
            _simpleProvider.ProcessEvent(raisedEvent);
        }
        public override void Shutdown()
        {
            _simpleProvider.Shutdown();
        }
    }
}