using MihaZupan;
using NLog;
using System;



namespace PugachBotTelegram
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                //Рабочий прокси можно найти на http://spys.one/proxys/DE/
                var proxy = new HttpToSocks5Proxy("188.40.22.206", 1080);

                // Some proxies limit target connections to a single IP address
                // If that is the case you have to resolve hostnames locally
                proxy.ResolveHostnamesLocally = true;

                PugachBotTelegram pugachBot = new PugachBotTelegram(Properties.Settings.Default.TelegramToken, proxy);
                pugachBot.RunBot();


                ConsoleKeyInfo cki;
                do
                {
                    Console.WriteLine("Введите текст для отправки:");
                    string str = Console.ReadLine();
                    if (str.Contains(":"))
                    {
                        var word = str.Trim().Split(':');
                        pugachBot.SendTextFromConsole(word[0], word[1]);
                    }
                    else
                    {
                        Console.WriteLine("Текст введен в неверном формате. Необходим разделитель ':'. Ничего не отправляем");
                    }

                    
                    cki = Console.ReadKey();
                }
                while (cki.Key != ConsoleKey.Escape);

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}
