using CryptographyLib;
using MihaZupan;
using NLog;
using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace PugachBotTelegram
{
    /// <summary>
    ///
    /// </summary>
    // Для проверки полученных сообщений https://api.telegram.org/bot749326751:AAFW67Hm4XkKENl2RVf7OQJEe22e828XDpk/getUpdates
    class PugachBotTelegram
    {
        private readonly string token;
        private static Logger logger= LogManager.GetCurrentClassLogger();

        public HttpToSocks5Proxy proxy;
        /// <summary>
        /// Клиент для Telegram Bot API https://telegrambots.github.io/book/index.html
        /// </summary>
        public TelegramBotClient telegramBotClient;
        public User me;

        public PugachBotTelegram(string encryptToken, HttpToSocks5Proxy proxy)
        {
            this.token = Protector.Decrypt(encryptToken);
            this.proxy = proxy;
            telegramBotClient = new TelegramBotClient(token, proxy);
            logger.Info("Создаем TelegramBotClient");
        }

        public void RunBot()
        {
            try
            {
                telegramBotClient.OnMessage += Bot_OnMessage;
                telegramBotClient.StartReceiving();
                me = telegramBotClient.GetMeAsync().Result;
                logger.Info($"Запущен бот {me.Id} {me.FirstName}");
                Console.WriteLine($"Запущен бот {me.Id} {me.FirstName}");
            }
            catch(Exception ex)
            {
                logger.Error($"Ошибка при запуске бота: {ex.Message}");
                throw new Exception($"Ошибка при запуске бота {ex.Message}");
                //Console.WriteLine($"Ошибка при запуске бота {ex.Message}");
            }
        }

        public void StopBot()
        {
            telegramBotClient.StopReceiving();
            logger.Info($"Бот остановлен {me.Id} {me.FirstName}");
            Console.WriteLine($"Бот остановлен {me.Id} {me.FirstName}");
        }

        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            MessageManager messageManager = new MessageManager(telegramBotClient);
            await messageManager.Process(e.Message);
        }

        public void SendTextFromConsole(string chatId, string text)
        {
            telegramBotClient.SendTextMessageAsync(chatId, text);
            logger.Info($"Отправляем через консоль {chatId}: {text}");
        }

       

    }
}
