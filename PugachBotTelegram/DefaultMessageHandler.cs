using NLog;
using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PugachBotTelegram
{
    internal class DefaultMessageHandler : IMessageHandler
    {
        private static Logger logger;

        public DefaultMessageHandler(Message message, TelegramBotClient telegramBotClient)
        {
            Message = message;
            BotClient = telegramBotClient;
            Type = message.Type;
        }

        public Message Message { get; }

        public MessageType Type { get; }

        public TelegramBotClient BotClient { get; }

        public async void AnswerAsync()
        {
            switch (Type)
            {
                case MessageType.Text:
                    {
                        ProcessingTextMessageAsync();
                        break;
                    }

                case Telegram.Bot.Types.Enums.MessageType.Sticker:
                    {
                        logger.Info($"{Message.Chat.FirstName} {Message.Chat.LastName} ({Message.Chat.Id},{Message.MessageId}): Отправил стикер {Message.Sticker.SetName}");
                        Console.WriteLine($"{Message.Chat.FirstName} {Message.Chat.LastName} ({Message.Chat.Id}): Отправил стикер {Message.Sticker.SetName}");
                        Message msg;
                        using (var stream = System.IO.File.OpenRead(@"D:\C#\Skillbox\SkillBoxDays\Day022_SkillBox\Foto\CrXpXRnLVEc.jpg"))
                        {
                            msg = await BotClient.SendPhotoAsync(
                              chatId: Message.Chat,
                              photo: stream
                            );
                            logger.Info($@"Отправляем фото {Message.MessageId}: D:\C#\Skillbox\SkillBoxDays\Day022_SkillBox\Foto\CrXpXRnLVEc.jpg - {Message.Chat.FirstName} {Message.Chat.LastName} ({Message.Chat.Id})");
                        }

                        break;
                    }

                default:
                    {
                        logger.Info($"{Message.Chat.FirstName} {Message.Chat.LastName} ({Message.Chat.Id},{Message.MessageId}): {Message.Text}");
                        Console.WriteLine($"{Message.Chat.FirstName} {Message.Chat.LastName} ({Message.Chat.Id}): {Message.Text}");
                        await BotClient.SendTextMessageAsync(Message.Chat.Id, "Я не в курсе");
                        logger.Info($"Отправляем ответ {Message.MessageId}: {Message.Text} - {Message.Chat.FirstName} {Message.Chat.LastName} ({Message.Chat.Id})");
                        break;
                    }
            }
        }

        private async void ProcessingTextMessageAsync()
        {
            logger.Info($"{Message.Chat.FirstName} {Message.Chat.LastName} ({Message.Chat.Id},{Message.MessageId}): {Message.Text}");
            Console.WriteLine($"{Message.Chat.FirstName} {Message.Chat.LastName} ({Message.Chat.Id},{Message.MessageId}): {Message.Text}");

            string mesType = "";

            if (Message.Text == "/start")
            {
                mesType = "start";
                string mes = "Привет. Это PugachBot.\r\n Ты можешь написать мне любой город в формате {Погода [город]} (Например: Погода Москва), и я отправлю тебе погодную информацию в этом городе.\r\n Также мне можно попробовать отправить стикер или просто пообщаться";
                await BotClient.SendTextMessageAsync(
                Message.Chat.Id,
                mes
                );
                logger.Info($"Отправляем ответ {Message.MessageId}: {mes} - {Message.Chat.FirstName} {Message.Chat.LastName} ({Message.Chat.Id})");
            }

            if (Message.Text.ToLower().Contains("погода"))
            {
                mesType = "погода";
                string city = Message.Text.ToLower().Replace("погода", "").Trim();
                string mes = OpenWeatherMap.GetTemperature(city);
                await BotClient.SendTextMessageAsync(
                Message.Chat.Id,
                mes
                );
                logger.Info($"Отправляем ответ {Message.MessageId}: {mes} - {Message.Chat.FirstName} {Message.Chat.LastName} ({Message.Chat.Id})");
            }


            if (IsGreeting(Message.Text, out string mesGreeting))
            {
                mesType = "greet";
                await BotClient.SendTextMessageAsync(
                Message.Chat.Id,
                mesGreeting
                );
                logger.Info($"Отправляем ответ {Message.MessageId}: {mesGreeting} - {Message.Chat.FirstName} {Message.Chat.LastName} ({Message.Chat.Id})");
            }

            if (String.IsNullOrEmpty(mesType))
            {
                await BotClient.SendTextMessageAsync(
                Message.Chat.Id,
                "Не знаю что ответить"
                );
                logger.Info($"Отправляем ответ {Message.MessageId}: {"Не знаю что ответить"} - {Message.Chat.FirstName} {Message.Chat.LastName} ({Message.Chat.Id})");
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