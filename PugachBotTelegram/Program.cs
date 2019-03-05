using CryptographyLib;
using MihaZupan;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Net.Http;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;


namespace PugachBotTelegram
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static HttpClient httpClient = new HttpClient(); // httpClient позволяет делать запросы к интернет-ресурсам

        /// <summary>
        /// http://api.openweathermap.org/
        /// Для проверки полученных сообщений https://api.telegram.org/bot749326751:AAFW67Hm4XkKENl2RVf7OQJEe22e828XDpk/getUpdates
        /// </summary>
        /// <param name="Title"></param>
        /// <returns></returns>
        static public string GetTemperature(string Title)
        {
            try
            {
                //Pacшифровываем appid для openweathermap
                string appId = Protector.Decrypt(Properties.Settings.Default.OpenweathermapAppId);
                string url = $"http://api.openweathermap.org/data/2.5/weather?q={Title}&units=metric&appid={appId}";
                string data = httpClient.GetStringAsync(url).Result;
                dynamic r = JObject.Parse(data);
                return $"{r.main.temp}°c";
            }
            catch (Exception)
            {
                return "Ошибка запроса";
            }
        }

        static void Main(string[] args)
        {
            string token = Protector.Decrypt(Properties.Settings.Default.TelegramToken);
            // Секретный токен бота

            //Рабочий прокси можно найти на http://spys.one/proxys/DE/
            var proxy = new HttpToSocks5Proxy("188.40.22.206", 1080);

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


                };


            telegramBotClient.StartReceiving();//Начинаем получать обновления
            ConsoleKeyInfo cki;
            do
            {
                string str = Console.ReadLine();
                var word = str.Split(':');
                telegramBotClient.SendTextMessageAsync(
                               word[0],
                               word[1]
                               );
                cki = Console.ReadKey();
            }
            while (cki.Key != ConsoleKey.Escape);
            Console.ReadKey();


        }

        private static async void ProcessingTextMessageAsync(Message message, TelegramBotClient telegramBotClient)
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
                string mes = GetTemperature(message.Text);
                await telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                mes
                );
                logger.Info($"Отправляем ответ {message.MessageId}: {mes} - {message.Chat.FirstName} {message.Chat.LastName} ({message.Chat.Id})");
            }
        }
    }
}
