using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PugachBotTelegram
{
    public class MessageManager
    {
        public TelegramBotClient telegramBotClient { get; set; }

        public MessageManager(TelegramBotClient telegramBotClient)
        {
            this.telegramBotClient = telegramBotClient;
        }

        public void Process(Message message)
        {
            MessageHandler messageHandler;

            if (message.Chat.Id == kateID)
            {
                messageHandler = new KateMessageHandler(message, telegramBotClient);
            }
            else messageHandler = new DefaultMessageHandler(message, telegramBotClient);

            messageHandler.Answer();



        }
    }
}
