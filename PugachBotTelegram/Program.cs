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

        static void Main(string[] args)
        {
            //Рабочий прокси можно найти на http://spys.one/proxys/DE/
            var proxy = new HttpToSocks5Proxy("188.40.22.206", 1080);

            // Some proxies limit target connections to a single IP address
            // If that is the case you have to resolve hostnames locally
            proxy.ResolveHostnamesLocally = true;

            PugachBotTelegram pugachBot = new PugachBotTelegram(Properties.Settings.Default.TelegramToken, proxy);
            pugachBot.RunBot();

            
            //ConsoleKeyInfo cki;
            //do
            //{
            //    string str = Console.ReadLine();
            //    var word = str.Split(':');
            //    telegramBotClient.SendTextMessageAsync(
            //                   word[0],
            //                   word[1]
            //                   );
            //    cki = Console.ReadKey();
            //}
            //while (cki.Key != ConsoleKey.Escape);
            Console.ReadKey();


        }

        
    }
}
