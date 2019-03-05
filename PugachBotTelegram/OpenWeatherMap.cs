using CryptographyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PugachBotTelegram
{
    class OpenWeatherMap
    {
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
    }
}
