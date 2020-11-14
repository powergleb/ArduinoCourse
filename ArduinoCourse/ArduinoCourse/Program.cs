using ArduinoCourse.Entities.Common;
using ArduinoCourse.Entities.Lessons;
using ArduinoCourse.Entities.Menu;
using System;
using System.IO;
using System.Xml.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace ArduinoCourse
{
    class Program
    {
        static TelegramBotClient bot;
        static string token = System.IO.File.ReadAllText("secret.txt");

        static void Main(string[] args)
        {
            MainMenu menu = (Environment.CurrentDirectory + "\\lessons\\main_menu.xml").ToMainMenu();

            bot = new TelegramBotClient(token);

            bot.OnMessage += TestHandler;
            bot.OnCallbackQuery += HandleCallbackQuery;
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

            Console.WriteLine($"{msg.From.FirstName} {msg.From.LastName} : {msg.Text}");

            await bot.SendPhotoAsync(msg.Chat.Id, new InputOnlineFile(new FileStream(Environment.CurrentDirectory + "\\lessons\\test_lesson_1\\pic\\pic1.jpg", FileMode.Open)));

            await bot.SendTextMessageAsync(msg.Chat.Id, "1");

            await bot.SendTextMessageAsync(msg.Chat.Id, "2");

            await bot.SendTextMessageAsync(msg.Chat.Id, "3");

            await bot.SendTextMessageAsync(msg.Chat.Id, "4");

            await bot.SendTextMessageAsync(msg.Chat.Id, "5");
        }

        static async void HandleCallbackQuery(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            Message msg = callbackQueryEventArgs.CallbackQuery.Message;
            

            //await bot.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id, "Received message " + callbackQueryEventArgs.CallbackQuery.Data);

            //await bot.EditMessageReplyMarkupAsync(callbackQueryEventArgs.CallbackQuery.Message.Chat.Id, callbackQueryEventArgs.CallbackQuery.Message.MessageId, null);

            Console.WriteLine($"{callbackQueryEventArgs.CallbackQuery.From.FirstName} {callbackQueryEventArgs.CallbackQuery.From.LastName} : {callbackQueryEventArgs.CallbackQuery.Data}");

            await bot.SendTextMessageAsync(msg.Chat.Id, callbackQueryEventArgs.CallbackQuery.Data);
        }
    }
}
