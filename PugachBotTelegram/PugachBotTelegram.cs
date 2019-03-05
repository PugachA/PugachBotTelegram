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
using Telegram.Bot.Types;

namespace PugachBotTelegram
{
    /// <summary>
    /// 
    /// </summary>
    class PugachBotTelegram
    {
        private readonly string token;
        private static Logger logger;

        public HttpToSocks5Proxy proxy;
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
            telegramBotClient.OnMessage += Bot_OnMessage;
            telegramBotClient.StartReceiving();
            me = telegramBotClient.GetMeAsync().Result;
            logger.Info($"Запущен бот {me.Id} {me.FirstName}");
            Console.WriteLine($"Запущен бот {me.Id} {me.FirstName}");
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
            if (message.Text == "/start")
            {
                string mes = "Привет. Это PugachBot. Ты можешь написать мне любой город, и я отправлю тебе погодную информацию в этом городе. Также мне можно попробовать отправить стикер или просто пообщаться";
                await telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                mes
                );
                logger.Info($"Отправляем ответ {message.MessageId}: {mes} - {message.Chat.FirstName} {message.Chat.LastName} ({message.Chat.Id})");
            }
            else
            {
                string mes = OpenWeatherMap.GetTemperature(message.Text);
                await telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                mes
                );
                logger.Info($"Отправляем ответ {message.MessageId}: {mes} - {message.Chat.FirstName} {message.Chat.LastName} ({message.Chat.Id})");
            }
        }

    }
}
