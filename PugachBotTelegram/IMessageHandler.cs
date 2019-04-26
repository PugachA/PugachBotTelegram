using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PugachBotTelegram
{
    interface IMessageHandler
    {
        Message Message { get;}
        MessageType Type { get; }
        TelegramBotClient BotClient { get; } 
        void AnswerAsync();
    }
}
