using System;
using System.Collections.Generic;
using System.Text;

namespace WebFramework.Message
{
    public class MessagesForView
    {
        public string Message { get; set; }
        public MessageStatus MessageStatus { get; set; }
    }

    public enum MessageStatus
    {
        info,danger,success,warning
    }
}
