using ArduinoCourse.Entities.Common;
using ArduinoCourse.Entities.Lessons;
using ArduinoCourse.Entities.Menu;
using ArduinoCourse.Entities.Users;
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
        static MainMenu menu;
        static UserList users;
        static TelegramBotClient bot;
        static string token = System.IO.File.ReadAllText("secret.txt");

        static void Main(string[] args)
        {
            menu = (Environment.CurrentDirectory + "\\lessons\\main_menu.xml").ToMainMenu();
            users = (Environment.CurrentDirectory + "\\lessons\\users.xml").ToUserList();

            bot = new TelegramBotClient(token);

            bot.OnMessage += MessageHandler;
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

        #region Log
        static void Log(Message msg)
        {
            Console.WriteLine($"{DateTime.Now} : {msg.From.FirstName} {msg.From.LastName} ({msg.From.Id}) : {msg.Text}");
        }

        static void Log(CallbackQuery msg)
        {
            Console.WriteLine($"{DateTime.Now} : {msg.From.FirstName} {msg.From.LastName} ({msg.From.Id}) : {msg.Data}");
        }
        #endregion
        
        static async void MessageHandler(object sender, MessageEventArgs e)
        {
            Message msg = e.Message;
            long id = msg.From.Id;
            Log(msg);
            Entities.Users.User user = users.GetUserById(msg.From.Id);

            if (msg.Text == "\\start")
            {
                if(user == null)
                {
                    user = users.CreateUser(id);

                    await bot.SendTextMessageAsync(id, $"Добро пожаловать, {msg.From.FirstName} {msg.From.LastName}!");
                }

                SentMainMenu(user);

                return;
            }

            await bot.SendTextMessageAsync(id, "Используйте команду \\start");
        }

        static async void SentMainMenu(Entities.Users.User user)
        {
            await bot.SendTextMessageAsync(user.Id, "menu");

            // TO DO
        }

        static async void HandleCallbackQuery(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            CallbackQuery msg = callbackQueryEventArgs.CallbackQuery;
            long id = msg.From.Id;
            Log(msg);

            //await bot.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id, "Received message " + callbackQueryEventArgs.CallbackQuery.Data);

            //await bot.EditMessageReplyMarkupAsync(callbackQueryEventArgs.CallbackQuery.Message.Chat.Id, callbackQueryEventArgs.CallbackQuery.Message.MessageId, null);

            //Console.WriteLine($"{callbackQueryEventArgs.CallbackQuery.From.FirstName} {callbackQueryEventArgs.CallbackQuery.From.LastName} : {callbackQueryEventArgs.CallbackQuery.Data}");

            //await bot.SendPhotoAsync(chat, new InputOnlineFile(new FileStream(Environment.CurrentDirectory + "\\lessons\\test_lesson_1\\pic\\pic1.jpg", FileMode.Open)));

            await bot.SendTextMessageAsync(id, callbackQueryEventArgs.CallbackQuery.Data);
        }


    }
}
