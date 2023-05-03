using System;

namespace DemoCenter.Models.Foundations.Emails
{
    public class Email
    {
        public Guid Id { get; set; }
        public string SenderAddress { get; set; }
        public string ReceiverAddress { get; set; }
        public string Subject { get; set; }
        public string TextBody { get; set; }
        public string HtmlBody { get; set; }
        public string Cc { get; set; }
        public bool TrackOpens { get; set; }
    }
}
