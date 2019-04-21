using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PugachBotTelegram.Message
{
    abstract public class TextMessageType : IMessageType
    {

        abstract public string Message { get; set; }

        abstract public IMessageType MessageType { get; set; }

        abstract public string Answer();
    }
}
