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
        private static Logger logger;

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
            logger = LogManager.GetCurrentClassLogger();
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
            switch (e.Message.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.Text:
                    {
                        ProcessingTextMessageAsync(e.Message, telegramBotClient);
                        break;
                    }

                case Telegram.Bot.Types.Enums.MessageType.Sticker:
                    {
                        logger.Info($"{e.Message.Chat.FirstName} {e.Message.Chat.LastName} ({e.Message.Chat.Id},{e.Message.MessageId}): Отправил стикер {e.Message.Sticker.SetName}");
                        Console.WriteLine($"{e.Message.Chat.FirstName} {e.Message.Chat.LastName} ({e.Message.Chat.Id}): Отправил стикер {e.Message.Sticker.SetName}");
                        Message msg;
                        using (var stream = System.IO.File.OpenRead(@"D:\C#\Skillbox\SkillBoxDays\Day022_SkillBox\Foto\CrXpXRnLVEc.jpg"))
                        {
                            msg = await telegramBotClient.SendPhotoAsync(
                              chatId: e.Message.Chat,
                              photo: stream
                            );
                            logger.Info($@"Отправляем фото {e.Message.MessageId}: D:\C#\Skillbox\SkillBoxDays\Day022_SkillBox\Foto\CrXpXRnLVEc.jpg - {e.Message.Chat.FirstName} {e.Message.Chat.LastName} ({e.Message.Chat.Id})");
                        }

                        break;
                    }

                default:
                    {
                        logger.Info($"{e.Message.Chat.FirstName} {e.Message.Chat.LastName} ({e.Message.Chat.Id},{e.Message.MessageId}): {e.Message.Text}");
                        Console.WriteLine($"{e.Message.Chat.FirstName} {e.Message.Chat.LastName} ({e.Message.Chat.Id}): {e.Message.Text}");
                        await telegramBotClient.SendTextMessageAsync(e.Message.Chat.Id, "Я не в курсе");
                        logger.Info($"Отправляем ответ {e.Message.MessageId}: {e.Message.Text} - {e.Message.Chat.FirstName} {e.Message.Chat.LastName} ({e.Message.Chat.Id})");
                        break;
                    }
            }
        }

        private async void ProcessingTextMessageAsync(Message message, TelegramBotClient telegramBotClient)
        {
            logger.Info($"{message.Chat.FirstName} {message.Chat.LastName} ({message.Chat.Id},{message.MessageId}): {message.Text}");
            Console.WriteLine($"{message.Chat.FirstName} {message.Chat.LastName} ({message.Chat.Id},{message.MessageId}): {message.Text}");

            string mesType = "";

            if (message.Text == "/start")
            {
                mesType = "start";
                string mes = "Привет. Это PugachBot.\r\n Ты можешь написать мне любой город в формате {Погода [город]} (Например: Погода Москва), и я отправлю тебе погодную информацию в этом городе.\r\n Также мне можно попробовать отправить стикер или просто пообщаться";
                await telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                mes
                );
                logger.Info($"Отправляем ответ {message.MessageId}: {mes} - {message.Chat.FirstName} {message.Chat.LastName} ({message.Chat.Id})");
            }

            if (message.Text.ToLower().Contains("погода"))
            {
                mesType = "погода";
                string city = message.Text.ToLower().Replace("погода", "").Trim();
                string mes = OpenWeatherMap.GetTemperature(city);
                await telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                mes
                );
                logger.Info($"Отправляем ответ {message.MessageId}: {mes} - {message.Chat.FirstName} {message.Chat.LastName} ({message.Chat.Id})");
            }

            
            if (IsGreeting(message.Text, out string mesGreeting))
            {
                mesType = "greet";
                await telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                mesGreeting
                );
                logger.Info($"Отправляем ответ {message.MessageId}: {mesGreeting} - {message.Chat.FirstName} {message.Chat.LastName} ({message.Chat.Id})");
            }

            if(String.IsNullOrEmpty(mesType))
            {
                await telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Не знаю что ответить"
                );
                logger.Info($"Отправляем ответ {message.MessageId}: {"Не знаю что ответить"} - {message.Chat.FirstName} {message.Chat.LastName} ({message.Chat.Id})");
            }
        }

        private bool IsGreeting(string text, out string mes)
        {
            bool flag = false;
            FillGreetingsDic();
            foreach (KeyValuePair<string, string> greeting in Greetings)
            {
                if (text.ToLower().Contains(greeting.Key))
                {
                    flag = true;
                    mes = greeting.Value;
                    return flag;
                }
            }
            mes = "Хм, что бы ответить";
            return flag;
        }

        public void SendTextFromConsole(string chatId, string text)
        {
            telegramBotClient.SendTextMessageAsync(chatId, text);
            logger.Info($"Отправляем через консоль {chatId}: {text}");
        }

        Dictionary<string, string> Greetings = new Dictionary<string, string>();

        void FillGreetingsDic()
        {
            if (Greetings.Count == 0)
            {
                Greetings.Add("hello", "Hello");
                Greetings.Add("привет", "Привет");
                Greetings.Add("хай", "Хай");
                Greetings.Add("добрый день", "И тебе доброго дня!");
                Greetings.Add("добрый вечер", "Что-то ты поздно. Привет!");
                Greetings.Add("доброе утро", "Доброе утро! Хорошего дня!");
                Greetings.Add("здравствуй", "Здравствуйте!");
            }
        }

    }
}
