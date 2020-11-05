using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace ArduinoCourse
{
    class Program
    {
        static TelegramBotClient bot;
        static string token = System.IO.File.ReadAllText("secret.txt"); 

        static void Main(string[] args)
        {
            bot = new TelegramBotClient(token);
            
            bot.OnMessage += TestHandler;
            bot.DeleteWebhookAsync();

            var me = bot.GetMeAsync().Result;
            Console.Title = me.Username;
            Console.WriteLine("Start '{0}' listening...", me);

            bot.StartReceiving();
            Console.ReadLine();
            bot.StopReceiving();

            Console.WriteLine("\nend.");
        }
        static async void TestHandler(object sender, MessageEventArgs e)
        {
            Message msg = e.Message;

            await bot.SendTextMessageAsync(msg.Chat.Id, "Hello, " + msg.From.FirstName + "!");
        }
    }
}
