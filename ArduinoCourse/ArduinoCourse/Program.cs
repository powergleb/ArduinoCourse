﻿using ArduinoCourse.Entities.Common;
using ArduinoCourse.Entities.Lessons;
using ArduinoCourse.Entities.Menu;
using ArduinoCourse.Entities.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace ArduinoCourse
{
    class Program
    {
        static object loker = new object();

        static string MenuPath;
        static string UsersPath;

        static MainMenu menu;
        static UserList users;
        static TelegramBotClient bot;
        static string token = System.IO.File.ReadAllText("secret.txt");

        static void Main(string[] args)
        {
            MenuPath = (Environment.CurrentDirectory + "\\lessons\\main_menu.xml");
            UsersPath = (Environment.CurrentDirectory + "\\lessons\\users.xml");

            menu = MenuPath.ToMainMenu();
            users = UsersPath.ToUserList();

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

#pragma warning disable CS1998
        static async void MessageHandler(object sender, MessageEventArgs e)
        {
            lock (loker)
            {
                Message msg = e.Message;
                long id = msg.From.Id;
                Log(msg);

                if (msg.Text == "/save")
                {
                    users.ToFile(UsersPath);
                    bot.SendTextMessageAsync(id, "Список пользователей сохранен!").Wait();
                    return;
                }

                Entities.Users.User user = users.GetUserById(msg.From.Id);

                if (msg.Text == "/start")
                {
                    if (user == null)
                    {
                        user = users.CreateUser(id);

                        bot.SendTextMessageAsync(id, $"Добро пожаловать, {msg.From.FirstName} {msg.From.LastName}!").Wait();
                    }

                    SendMainMenu(user);

                    return;
                }

                bot.SendTextMessageAsync(id, "Используйте команду /start").Wait();
            }
        }
#pragma warning restore CS1998

        static void SendMainMenu(Entities.Users.User user)
        {
            lock (loker)
            {
                string text = "Выберите курс:";

                List<InlineKeyboardButton[]> list = new List<InlineKeyboardButton[]>();

                for (int i = 0; i <= user.ActualLesson; i++)
                {
                    Lesson lesson = menu.Lessons[i];
                    InlineKeyboardButton button = new InlineKeyboardButton()
                    {
                        Text = lesson.Title,
                        CallbackData = string.Format("lm_{0}", i)
                    };
                    list.Add(new InlineKeyboardButton[] { button });
                }

                bot.SendTextMessageAsync(user.Id, text, replyMarkup: new InlineKeyboardMarkup(list.ToArray())).Wait();
            }
        }

        static void SendIncorrectCallback(long id)
        {
            bot.SendTextMessageAsync(id, "Некорректный callback.").Wait();
        }

#pragma warning disable CS1998
        static async void HandleCallbackQuery(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            lock (loker)
            {
                CallbackQuery msg = callbackQueryEventArgs.CallbackQuery;
                long id = msg.From.Id;
                Log(msg);

                Entities.Users.User user = users.GetUserById(id);

                if (user == null)
                {
                    bot.SendTextMessageAsync(id, "Зарегистрируйтесь при помощи команды \\start.").Wait();
                    return;
                }

                string data = msg.Data;
                string[] parts = data.Split("_");

                if (parts.Length < 1)
                {
                    SendIncorrectCallback(id);
                    return;
                }

                Prefixes prefix = parts[0].ToPrefix();

                if (prefix == Prefixes.Error || parts.Length != prefix.Parts())
                {
                    SendIncorrectCallback(id);
                    return;
                }

                int[] parts_int = new int[3];

                for (int i = 1; i < parts.Length; i++)
                {
                    parts_int[i - 1] = int.Parse(parts[i]);
                }

                if (prefix != Prefixes.LessonTestAnswer)
                {
                    bot.EditMessageReplyMarkupAsync(callbackQueryEventArgs.CallbackQuery.Message.Chat.Id, callbackQueryEventArgs.CallbackQuery.Message.MessageId, null).Wait();
                }

                switch (prefix)
                {
                    case Prefixes.ToMenu:
                        SendMainMenu(user);
                        return;
                    case Prefixes.LessonAtMenu:
                        SetCurrentLesson(user, parts_int[0]);
                        SendLessonMenu(user, parts_int[0]);
                        return;
                    case Prefixes.LessonTheory:
                        SendTheory(user, parts_int[0], parts_int[1]);
                        return;
                    case Prefixes.LessonTest:
                        SendTest(user, parts_int[0], parts_int[1]);
                        return;
                    case Prefixes.LessonTestAnswer:
                        SendTestAnswer(user, parts_int[0], parts_int[1], parts_int[2]);
                        return;
                }

                SendIncorrectCallback(id);
            }
        }
#pragma warning restore CS1998

        static void SetCurrentLesson(Entities.Users.User user, int lesson)
        {
            lock (loker)
            {
                if (lesson < 0)
                {
                    bot.SendTextMessageAsync(user.Id, "Некорректный id урока.").Wait();
                }

                if (lesson > user.ActualLesson)
                {
                    bot.SendTextMessageAsync(user.Id, "Этот урок пока что не доступен Вам.").Wait();
                    return;
                }
                else
                {
                    user.CurrentLesson = menu.Lessons[lesson];

                    if (lesson == user.ActualLesson)
                    {
                        user.CurrentLessonActualTheory = user.ActualLessonActualTheory;
                        user.CurrentLessonActualTest = user.ActualLessonActualTest;
                    }
                    else
                    {
                        user.CurrentLessonActualTheory = user.CurrentLesson.Theories.Count - 1;
                        user.CurrentLessonActualTest = user.CurrentLesson.Tests.Count - 1;
                    }
                }
            }
        }

        static void SendLessonMenu(Entities.Users.User user, int lesson_id)
        {
            lock (loker)
            {
                Lesson lesson = user.CurrentLesson;

                if (lesson == null)
                {
                    bot.SendTextMessageAsync(user.Id, "У Вас не выбран урок, воспользуйтесь командой \\start").Wait();
                    return;
                }

                //await bot.SendTextMessageAsync(user.Id, string.Format("Урок №{0}", lesson_id));


                List<InlineKeyboardButton[]> list = new List<InlineKeyboardButton[]>();

                for (int i = 0; i <= user.CurrentLessonActualTheory && i < lesson.Theories.Count; i++)
                {
                    InlineKeyboardButton button = new InlineKeyboardButton()
                    {
                        Text = string.Format("Теория: {0}", lesson.Theories[i].Title),
                        CallbackData = string.Format("lt_{0}_{1}", lesson_id, i)
                    };
                    list.Add(new InlineKeyboardButton[] { button });
                }

                for (int i = 0; i <= user.CurrentLessonActualTest && i < lesson.Tests.Count; i++)
                {
                    InlineKeyboardButton button = new InlineKeyboardButton()
                    {
                        Text = string.Format("Вопрос: {0}", lesson.Tests[i].Title),
                        CallbackData = string.Format("lq_{0}_{1}", lesson_id, i)
                    };
                    list.Add(new InlineKeyboardButton[] { button });
                }

                list.Add(new InlineKeyboardButton[]
                {
                new InlineKeyboardButton()
                {
                    Text = "Назад",
                    CallbackData = "mm"
                }
                });

                bot.SendTextMessageAsync(user.Id, user.CurrentLesson.Title, replyMarkup: new InlineKeyboardMarkup(list.ToArray())).Wait();
            }
        }

        static void SendTheory(Entities.Users.User user, int lesson_id, int theory_id)
        {
            lock (loker)
            {
                Lesson lesson = user.CurrentLesson;
                if (lesson == null)
                {
                    bot.SendTextMessageAsync(user.Id, "У Вас не выбран урок, воспользуйтесь командой \\start").Wait();
                    return;
                }

                Theory theory = lesson.Theories[theory_id];
                //await bot.SendTextMessageAsync(user.Id, string.Format("Теория №{0}", theory_id));
                bot.SendTextMessageAsync(user.Id, theory.Title).Wait();
                bot.SendTextMessageAsync(user.Id, theory.Text).Wait();
                foreach (var pic in theory.Pics.GetPics())
                {
                    bot.SendPhotoAsync(user.Id, new InputOnlineFile(pic)).Wait();
                }

                List<InlineKeyboardButton[]> list = new List<InlineKeyboardButton[]>();

                bool last_theory_flag = false;

                if(theory_id + 1 == user.CurrentLesson.Theories.Count)
                {
                    last_theory_flag = true;
                }

                if (lesson_id == user.ActualLesson && theory_id == user.ActualLessonActualTheory)
                {
                    user.ActualLessonActualTheory++;
                    user.CurrentLessonActualTheory++;

                    if (user.ActualLessonActualTheory == user.CurrentLesson.Theories.Count)
                    {
                        last_theory_flag = true;
                        user.ActualLessonActualTest = 0;
                        user.CurrentLessonActualTest = 0;
                    }
                }

                InlineKeyboardButton next_button = new InlineKeyboardButton()
                {
                    Text = "Далее",
                };

                if (last_theory_flag)
                {
                    next_button.CallbackData = string.Format("lq_{0}_0", lesson_id);
                }
                else
                {
                    next_button.CallbackData = string.Format("lt_{0}_{1}", lesson_id, theory_id + 1);
                }

                list.Add(new InlineKeyboardButton[] { next_button });
                list.Add(new InlineKeyboardButton[]
                {
                new InlineKeyboardButton()
                {
                    Text = "Назад",
                    CallbackData = string.Format("lm_{0}", lesson_id)
                }
                });

                bot.SendTextMessageAsync(user.Id, "Далее:", replyMarkup: new InlineKeyboardMarkup(list.ToArray())).Wait();
            }
        }

        static void SendTest(Entities.Users.User user, int lesson_id, int test_id)
        {
            lock (loker)
            {
                Lesson lesson = user.CurrentLesson;
                if (lesson == null)
                {
                    bot.SendTextMessageAsync(user.Id, "У Вас не выбран урок, воспользуйтесь командой \\start").Wait();
                    return;
                }

                Test test = lesson.Tests[test_id];
                //await bot.SendTextMessageAsync(user.Id, string.Format("Тест №{0}", test_id));
                bot.SendTextMessageAsync(user.Id, test.Title).Wait();
                bot.SendTextMessageAsync(user.Id, test.Text).Wait();
                foreach (var pic in test.Pics.GetPics())
                {
                    bot.SendPhotoAsync(user.Id, new InputOnlineFile(pic)).Wait();
                }

                List<InlineKeyboardButton[]> list = new List<InlineKeyboardButton[]>();

                for (int i = 0; i < test.Variants.Count; i++)
                {
                    list.Add(new InlineKeyboardButton[]
                    {
                    new InlineKeyboardButton()
                    {
                        Text = test.Variants[i],
                        CallbackData = string.Format("la_{0}_{1}_{2}", lesson_id, test_id, i)
                    }
                    });
                }

                list.Add(new InlineKeyboardButton[]
                {
                new InlineKeyboardButton()
                {
                    Text = "Назад",
                    CallbackData = string.Format("lm_{0}", lesson_id)
                }
                });

                bot.SendTextMessageAsync(user.Id, "Ответ:", replyMarkup: new InlineKeyboardMarkup(list.ToArray())).Wait();
            }
        }

        static void SendTestAnswer(Entities.Users.User user, int lesson_id, int test_id, int answer)
        {
            lock (loker)
            {
                Lesson lesson = user.CurrentLesson;
                if (lesson == null)
                {
                    bot.SendTextMessageAsync(user.Id, "У Вас не выбран урок, воспользуйтесь командой \\start").Wait();
                    return;
                }

                if (lesson.Tests[test_id].Answer != answer)
                {
                    bot.SendTextMessageAsync(user.Id, "Не верно!").Wait();
                    return;
                }

                bot.SendTextMessageAsync(user.Id, "Верно!").Wait();

                if (test_id + 1 < lesson.Tests.Count)
                {
                    if (lesson_id == user.ActualLesson)
                    {
                        user.ActualLessonActualTest++;
                        user.CurrentLessonActualTest++;
                    }

                    SendTest(user, lesson_id, test_id + 1);
                    return;
                }

                if (lesson_id != user.ActualLesson)
                {
                    SetCurrentLesson(user, lesson_id + 1);
                    SendLessonMenu(user, lesson_id + 1);
                    return;
                }

                if (user.ActualLesson + 1 == menu.Lessons.Count)
                {
                    bot.SendTextMessageAsync(user.Id, "Вы завершили этот курс! Мои поздравления!").Wait();
                    bot.SendTextMessageAsync(user.Id, "Курс подготовили учащиеся СГУ КНиИТ: \nРазработка: Петров Алексей \nУроки: Роках Глеб.").Wait();
                    SendMainMenu(user);
                    return;
                }

                user.ActualLesson++;
                user.ActualLessonActualTheory = 0;
                user.ActualLessonActualTest = -1;

                SetCurrentLesson(user, lesson_id + 1);
                SendLessonMenu(user, lesson_id + 1);
                return;
            }
        }
    }
}
