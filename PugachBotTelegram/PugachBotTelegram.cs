using CryptographyLib;
using MihaZupan;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace PugachBotTelegram
{
    class PugachBotTelegram
    {
        private readonly string token;
        public HttpToSocks5Proxy proxy;
        public TelegramBotClient telegramBotClient;
        private static Logger logger;

        public PugachBotTelegram(string encryptToken, HttpToSocks5Proxy proxy)
        {
            this.token = Protector.Decrypt(encryptToken);
            this.proxy = proxy;
            telegramBotClient = new TelegramBotClient(token, proxy);
            logger = LogManager.GetCurrentClassLogger();
            logger.Info("Создаем TelegramBotClient");
        }

        public void RunBot()
        {
            telegramBotClient.OnMessage += Bot_OnMessage;
            telegramBotClient.StartReceiving();
            var me = telegramBotClient.GetMeAsync().Result;
            logger.Info($"Запущен бот {me.Id} {me.FirstName}");
        }

        public void StopBot()
        {
            telegramBotClient.StopReceiving();
        }

        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {

        }

    }
}
