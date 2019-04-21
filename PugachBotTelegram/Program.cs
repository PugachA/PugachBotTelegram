using CryptographyLib;
using MihaZupan;
using NLog;
using System;
using System.Net;

namespace PugachBotTelegram
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                string proxyPassword = Protector.Decrypt(Properties.Settings.Default.ProxyPassword);
                HttpToSocks5Proxy proxy = new HttpToSocks5Proxy("196.18.12.170", 8000, "UWK8Gs", proxyPassword);

                string teleglamToken = Protector.Decrypt(Properties.Settings.Default.TelegramToken);
                PugachBotTelegram pugachBot = new PugachBotTelegram(teleglamToken, proxy);
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
