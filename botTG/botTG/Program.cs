using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace botTG
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new BotClient("TOKEN BOT");
            var updates = bot.GetUpdates();
            while (true)
            {




                if (updates?.Length > 0)
                {

                    Console.WriteLine("hay updates");
                    
                    foreach (var update in updates)
                    {

                        if (update?.Message != null)
                        {
                            var message = update.Message;
                            Console.WriteLine($"Message: {message.Text}");
                            if (message == null)
                                continue;
                            if (message.Text != null && message.Text.ToLower().Contains("-yt"))
                            {

                                HttpClient client;
                                client = new HttpClient();
                                string URL;
                                string TXT;
                                TXT = message.Text.Replace("-yt", "");
                                URL = "https://www.youtube.com/results?search_query=";
                                HttpResponseMessage response = client.GetAsync(URL + TXT).Result;
                                string responseStr = response.Content.ReadAsStringAsync().Result;
                                string ID = Regex.Match(responseStr, "videoId\":\"(.*?)\"(.*?)").Groups[1].Value;
                                bot.SendMessage(message.Chat.Id, "https://youtu.be/" + ID);

                            }
                            else if (message.Text != null && message.Text.ToLower().Contains("-go"))
                            {

                                HttpClient client;
                                client = new HttpClient();
                                string TXT;
                                TXT = message.Text.Replace("-go", "");
                                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36");
                                client.DefaultRequestHeaders.Add("Accept", "*/*");
                                var URL = $"https://www.google.com/search?q={TXT}&num=1";
                                HttpResponseMessage response = client.GetAsync(URL).Result;
                                string responseStr = response.Content.ReadAsStringAsync().Result;

                                string ID = Regex.Match(responseStr, "<div class=\"yuRUbf\"><a href=\"(.*?)\"(.*?)").Groups[1].Value;
                                bot.SendMessage(message.Chat.Id, "Toma: " + ID);

                            }
                            else if (message.Text != null && message.Text.ToLower().Contains("-help"))
                            {

                                HttpClient client;
                                client = new HttpClient();
                                string TXT;
                                TXT = message.Text;

                                bot.SendMessage(message.Chat.Id, "Comandos del bot\n1º= -go Busqueda en google\n2º= -yt Busqueda en youtube\n3º= -m3u Info lista M3u\n4º= -list m3u= Raspa listas m3u ");

                            }
                            else if (message.Text != null && message.Text.ToLower().Contains("-list m3u"))
                            {

                                HttpResponseMessage response;

                                HttpClient client;
                                client = new HttpClient();
                                string currenTime = DateTime.Now.ToString("dd/MM/yyyy");
                                var url2 = "http://iptvhit.com/freeiptv?";
                                response = client.GetAsync(url2 + currenTime).Result;


                                string responseStr = response.Content.ReadAsStringAsync().Result;

                                Regex rx = new Regex("playlist\\?(.*?)\"(.*?)", RegexOptions.Singleline);

                                foreach (Match match in rx.Matches(responseStr))
                                {


                                    var url = match.Groups[1];
                                    bot.SendMessage(message.Chat.Id, url + Environment.NewLine);


                                }




                            }
                            else if (message.Text != null && message.Text.ToLower().Contains("-m3u"))
                            {

                                HttpClient client;
                                client = new HttpClient();
                                string TXT;
                                TXT = message.Text.Replace("-m3u", "").Replace("get.php", "player_api.php").Replace("&type=m3u", "").Replace("_plus", "");

                                //TXT = TXT.Replace("get.php", "player_api.php").Replace("&type=m3u", "").Replace("_plus", "");
                                try
                                {


                                    HttpResponseMessage response = client.GetAsync(TXT).Result;

                                    string responseStr = response.Content.ReadAsStringAsync().Result;
                                    if (response.StatusCode == HttpStatusCode.OK)
                                    {




                                        string ESTADO = Regex.Match(responseStr, "status\":\"(.*?)\"(.*?)").Groups[1].Value;
                                        string ACTIVAS = Regex.Match(responseStr, "active_cons\":\"(.*?)\"(.*?)").Groups[1].Value;
                                        string CONEX = Regex.Match(responseStr, "max_connections\":\"(.*?)\"(.*?)").Groups[1].Value;
                                        var EXPIRA = Regex.Match(responseStr, "exp_date\":\"(.*?)\"(.*?)").Groups[1].Value;
                                        var humaDate = "NO VALIDO";
                                        if (long.TryParse(EXPIRA, out var longDate))
                                        {
                                            humaDate = longDate.FromUnixTime();
                                        }
                                        string CATE;
                                        CATE = "&action=get_live_categories";

                                        response = client.GetAsync(TXT + CATE).Result;


                                        string responseStr2 = response.Content.ReadAsStringAsync().Result;


                                        Regex rx2 = new Regex("\",\"category_name\":\"(.*?)\"(.*?)", RegexOptions.Multiline);
                                        string categoría = "";
                                        foreach (Match match in rx2.Matches(responseStr2))
                                        {


                                            //var urlb2 = match.Groups[1];
                                            categoría += match.Groups[1].Value + "-";

                                        }
                                        categoría = categoría.Trim('-');
                                        bot.SendMessage(message.Chat.Id, "Estado ✅" + ESTADO + Environment.NewLine + "Activas 👁" + ACTIVAS + Environment.NewLine + "Conexiones 👥" + CONEX + Environment.NewLine + "Caduca ⏲" + humaDate + Environment.NewLine + categoría);


                                    }
                                    else
                                    {
                                        string NOFOUNT;
                                        NOFOUNT = "LISTA CAIDA 👎 PRUEBA CON OTRA";
                                        bot.SendMessage(message.Chat.Id, " " + NOFOUNT);

                                    }
                                }
                                catch (Exception ex) //bloque catch para captura de error
                                {
                                    string NOFOUNT;
                                    NOFOUNT = "LISTA CAIDA 👎 PRUEBA CON OTRA";
                                    bot.SendMessage(message.Chat.Id, " " + NOFOUNT);


                                }
                            }
                        }

                        
                    }
                    updates = bot.GetUpdates(updates.Max(u => u.UpdateId) + 1);

                }
                else
                {
                    updates = bot.GetUpdates();
                }


            }
        }
    }
}

