using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PugachBotTelegram.Message
{
    public interface IMessageType
    {
        IMessageType MessageType { get; set; }
    }
}
