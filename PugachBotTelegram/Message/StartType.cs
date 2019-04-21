using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PugachBotTelegram
{
    public class StartType : TextMessageType
    {
        public StartType(string message)
        {
            Message = message;
        }

        public override IMessageType MessageType { get; set; }
        public override string Message { get; set; }

        public override string Answer()
        {
            throw new NotImplementedException();
        }
    }
}
