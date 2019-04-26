using Telegram.Bot;
using Telegram.Bot.Types;

namespace PugachBotTelegram
{
    internal class KateMessageHandler : MessageHandler
    {
        private Message message;
        private TelegramBotClient telegramBotClient;

        public KateMessageHandler(Message message, TelegramBotClient telegramBotClient)
        {
            this.message = message;
            this.telegramBotClient = telegramBotClient;
        }
    }
}