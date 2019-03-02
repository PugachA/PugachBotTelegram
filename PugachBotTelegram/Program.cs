using MihaZupan;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace PugachBotTelegram
{
    class Program
    {
        static HttpClient httpClient = new HttpClient(); // httpClient позволяет делать запросы к интернет-ресурсам

        /// <summary>
        /// http://api.openweathermap.org/
        /// </summary>
        /// <param name="Title"></param>
        /// <returns></returns>
        static public string GetTemperature(string Title)
        {
            try
            {
                string url = $"http://api.openweathermap.org/data/2.5/weather?q={Title}&units=metric&appid=f045766916db5de3e8a6a1dbd6187125";
                string data = httpClient.GetStringAsync(url).Result;
                dynamic r = JObject.Parse(data);
                return $"{r.main.temp}°c";
            }
            catch (Exception)
            {
                return "ошибка запроса";
            }
        }

        static void Main(string[] args)
        {
            string token = "749326751:AAFW67Hm4XkKENl2RVf7OQJEe22e828XDpk";
            // Секретный токен бота

            var proxy = new HttpToSocks5Proxy("94.130.1.45", 1080);

            // Some proxies limit target connections to a single IP address
            // If that is the case you have to resolve hostnames locally
            proxy.ResolveHostnamesLocally = true;

            // Создание telegramBotClient, для работы с API телеграм
            TelegramBotClient telegramBotClient = new TelegramBotClient(token, proxy);

            // Подписываемся на событие "Получение сообщений"
            telegramBotClient.OnMessage +=
                async delegate (object sender, MessageEventArgs e)
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
                                Console.WriteLine($"{e.Message.Chat.FirstName} {e.Message.Chat.LastName}: Отправил стикер {e.Message.Sticker.SetName}");
                                Message msg;
                                using (var stream = System.IO.File.OpenRead(@"D:\C#\Skillbox\SkillBoxDays\Day022_SkillBox\Foto\CrXpXRnLVEc.jpg"))
                                {
                                    msg = await telegramBotClient.SendPhotoAsync(
                                      chatId: e.Message.Chat,
                                      photo: stream
                                    );
                                }
                                break;
                            }

                        default:
                            {
                                Console.WriteLine(e.Message.Text);
                                await telegramBotClient.SendTextMessageAsync(
                               e.Message.Chat.Id,
                               "Я не в курсе"
                               );
                                break;
                            }
                    }


                };


            telegramBotClient.StartReceiving();//Начинаем получать обновления
            Console.ReadKey();

        }

        private static async void ProcessingTextMessageAsync(Message message, TelegramBotClient telegramBotClient)
        {
            Console.WriteLine($"{message.Chat.FirstName} {message.Chat.LastName}: {message.Text}");
            if (message.Text == "/start")
            {
                string mes = "Привет. Это PugachBot. Ты можешь написать мне любой город, и я отправлю тебе погодную информацию в этом городе. Также мне можно попробовать отправить стикер или просто пообщаться";
                await telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                mes
                );
            }
            else

                await telegramBotClient.SendTextMessageAsync(
                    message.Chat.Id,
                    GetTemperature(message.Text)
                    );
        }
    }
}
