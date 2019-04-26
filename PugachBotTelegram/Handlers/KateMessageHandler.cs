using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PugachBotTelegram
{
    public class KateMessageHandler : IMessageHandler
    {
        private Message message;
        private TelegramBotClient telegramBotClient;

        public KateMessageHandler(Message message, TelegramBotClient telegramBotClient)
        {
            this.message = message;
            this.telegramBotClient = telegramBotClient;
        }

        public Message Message => throw new System.NotImplementedException();

        public MessageType Type => throw new System.NotImplementedException();

        public TelegramBotClient BotClient => throw new System.NotImplementedException();

        public Task AnswerAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}