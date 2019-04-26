using Telegram.Bot;
using Telegram.Bot.Types;

namespace PugachBotTelegram
{
    internal class DefaultMessageHandler : MessageHandler
    {
        private Message message;
        private TelegramBotClient telegramBotClient;

        public DefaultMessageHandler(Message message, TelegramBotClient telegramBotClient)
        {
            this.message = message;
            this.telegramBotClient = telegramBotClient;
        }
    }
}