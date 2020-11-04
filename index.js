const TelegramBot = require("node-telegram-bot-api");
const { TOKEN } = require('./items/secret');

const bot = new TelegramBot(TOKEN, { polling: true });

const {Menu} = require('./items/main_menu');

const timeout = 500;

var users = [];

const Prefixes = {
    ToMenu: 'mm',
    LessonAtMenu: 'lm',
    LessonTheory: 'lt',
    LessonTest: 'lq',
    LessonTestAnswer: 'la'
}

function GetUser(id)
{
    for(var i in users)
    {
        var cur = users[i];
        if(cur.id == id)
        {
            return cur;
        }
    }
    return null;
}

function CreateUser(id)
{
    var user = {
        id: id,
        actual_lesson: 0,
        actual_lesson_actual_theory: 0,
        actual_lesson_actual_test: -1,
        current_lesson: null,
        current_lesson_actual_theory: -1,
        current_lesson_actual_test: -1,
        timeout: 0
    }

    users[users.length] = user;

    return user;
}

function ButtonsToOptions(buttons)
{
    return {
        reply_markup: JSON.stringify({
            inline_keyboard: buttons,
        })
    }
}

function SendMainMenu(user)
{
    var buttons = [];

    for(var i = 0; i <= user.actual_lesson; i++)
    {
        var button = {
            text: Menu.Lessons[i].Title, 
            callback_data: Prefixes.LessonAtMenu + '_' + i 
        }

        buttons.push([button]);
    }

    var options = ButtonsToOptions(buttons);

    setTimeout(() => bot.sendMessage(user.id, Menu.Text, options), timeout * user.timeout++);
}

bot.onText(/\/start/, (msg, match) => {
    var user = GetUser(msg.from.id);
    if(user == null)
    {        
        bot.sendMessage(msg.from.id, 'Добро пожаловать, ' + msg.from.first_name + ' ' + msg.from.last_name)
        user = CreateUser(msg.from.id);
    }

    SendMainMenu(user);
});

function SetCurrentLesson(user, lesson_id)
{
    if(lesson_id < 0)
    {
        bot.sendMessage(user.id, 'Некорректный id урока.');
    }

    if (lesson_id > user.actual_lesson)
    {
        bot.sendMessage(user.id, 'Этот урок пока что не доступен Вам.');
        return;
    }
    else 
    {
        const { Lesson } = require(Menu.Lessons[lesson_id].Path); 
        user.current_lesson = Lesson;

        if (lesson_id == user.actual_lesson)
        {
            user.current_lesson_actual_theory = user.actual_lesson_actual_theory;
            user.current_lesson_actual_test = user.actual_lesson_actual_test;
        }
        else
        {
            user.current_lesson_actual_theory = Lesson.Theory.length - 1;
            user.current_lesson_actual_test = Lesson.Tests.length - 1;
        }
    }
}

function SendLessonMenu(user, lesson_id)
{
    var text = user.current_lesson.Title;

    var buttons = [];

    var theory_callback_base = Prefixes.LessonTheory + '_' + lesson_id + '_';
    
    for(var i = 0; i <= user.current_lesson_actual_theory && i < user.current_lesson.Theory.length; i++)
    {
        var button = {
            text: 'Теория: ' + user.current_lesson.Theory[i].Title, 
            callback_data: theory_callback_base + i 
        }

        buttons.push([button]);
    }

    var test_callback_base = Prefixes.LessonTest + '_' + lesson_id + '_';
    
    for(var i = 0; i <= user.current_lesson_actual_test && i < user.current_lesson.Tests.length; i++)
    {
        var button = {
            text: 'Вопрос: ' + user.current_lesson.Tests[i].Title, 
            callback_data: test_callback_base + i 
        }

        buttons.push([button]);
    }
    
    buttons.push([{
        text: 'Назад', 
        callback_data: Prefixes.ToMenu
    }]);

    var options = ButtonsToOptions(buttons);

    setTimeout(() => bot.sendMessage(user.id, text, options), timeout * user.timeout++);
}

function SendTheory(user, lesson_id, part)
{
    var theory_item = user.current_lesson.Theory[part];

    setTimeout(() => bot.sendMessage(user.id, theory_item.Title), timeout * user.timeout++);
    setTimeout(() => bot.sendMessage(user.id, theory_item.Text), timeout * user.timeout++);
    for(var i in theory_item.Pic)
    {
        var t = theory_item.Pic[i];
        setTimeout(() => bot.sendPhoto(user.id, t), timeout * user.timeout++);
    }

    var last_theory_flag = false;

    if(lesson_id == user.actual_lesson && part == user.actual_lesson_actual_theory)
    {
        user.actual_lesson_actual_theory++;
        user.current_lesson_actual_theory++;

        if(user.actual_lesson_actual_theory == user.current_lesson.Theory.length)
        {
            last_theory_flag = true;
            user.actual_lesson_actual_test = 0;
            user.current_lesson_actual_test = 0;
        }
    }

    var next_button = {
        text: 'Далее', 
        callback_data: null, 
    }

    if(last_theory_flag)
    {
        next_button.callback_data = Prefixes.LessonTest + '_' + lesson_id + '_0';
    }
    else
    {
        next_button.callback_data = Prefixes.LessonTheory + '_' + lesson_id + '_' + (part + 1);
    }

    var options = ButtonsToOptions([
        [next_button],
        [{
            text: 'Назад', 
            callback_data: Prefixes.LessonAtMenu + '_' + lesson_id 
        }],
    ]);

    setTimeout(() => bot.sendMessage(user.id, 'Далее:', options), timeout * user.timeout++);
}

function SendTest(user, lesson_id, part)
{
    var test_item = user.current_lesson.Tests[part];

    setTimeout(() => bot.sendMessage(user.id, test_item.Title), timeout * user.timeout++);
    setTimeout(() => bot.sendMessage(user.id, test_item.Text), timeout * user.timeout++);

    for(var i in test_item.Pic)
    {
        var t = test_item.Pic[i];
        setTimeout(() => bot.sendPhoto(user.id, t), timeout * user.timeout++);
    }

    var answer_base = Prefixes.LessonTestAnswer + '_' + lesson_id + '_' + part + '_';

    var buttons = [];

    for(var i in test_item.Variants)
    {
        buttons.push([{
            text: test_item.Variants[i], 
            callback_data: answer_base + i
        }]);
    }
    
    buttons.push([{
        text: 'Назад', 
        callback_data: Prefixes.LessonAtMenu + '_' + lesson_id 
    }]);

    var options = ButtonsToOptions(buttons);

    setTimeout(() => bot.sendMessage(user.id, 'Ответ:', options), timeout * user.timeout++);
}

function SendTestAnswer(user, lesson_id, part, answer)
{
    if(user.current_lesson.Tests[part].Answer != answer)
    {
        bot.sendMessage(user.id, 'Не верно!');
        return;
    }

    setTimeout(() => bot.sendMessage(user.id, 'Верно!'), timeout * user.timeout++);

    if(part + 1 < user.current_lesson.Tests.length)
    {
        SendTest(user, lesson_id, part + 1);
        return;
    }

    if(lesson_id != user.actual_lesson)
    {        
        SetCurrentLesson(user, lesson_id + 1);
        SendLessonMenu(user, lesson_id + 1);
        return;
    }

    if(user.actual_lesson + 1 == Menu.Lessons.length)
    {
        setTimeout(() => bot.sendMessage(user.id, 
            'Вы завершили этот курс! Мои поздравления!'), timeout * user.timeout++);
        setTimeout(() => bot.sendMessage(user.id, 
            'Курс подготовили учащиеся СГУ КНиИТ: \n' + 
            'Разработка: Петров Алексей \n' + 
            'Уроки: Роках Глеб.'), 
            timeout * user.timeout++);
        SendMainMenu(user);
        return;
    }
        
    user.actual_lesson++;
    user.actual_lesson_actual_theory = 0;
    user.actual_lesson_actual_test = -1; 

    SetCurrentLesson(user, lesson_id + 1);
    SendLessonMenu(user, lesson_id + 1);
    return;
}

bot.on('callback_query', (msg) => {
    var user_id = msg.from.id;
    var user = GetUser(user_id);
    if(user == null)
    {
        bot.sendMessage(user_id, 'Вы не зарегистрированы! Используйте комманду \'/start\'.');
        return;
    }

    user.timeout = 0;

    var message_data = msg.data;
    var data_parts = message_data.split('_');
    
    if(data_parts.length < 1) 
    {
        bot.sendMessage(user_id, 'Некорректный callback.');
        return;
    }

    if(data_parts[0] == Prefixes.ToMenu)
    {
        SendMainMenu(user);
        return;
    }
    else
    {
        if(data_parts.length < 2)
        {
            bot.sendMessage(user_id, 'Некорректный callback.');
            return;
        }

        var lesson_id = data_parts[1] - 0;

        if(data_parts[0] == Prefixes.LessonAtMenu)
        {
            SetCurrentLesson(user, lesson_id);
            SendLessonMenu(user, lesson_id);
            return;
        }
        else
        {
            if(data_parts.length < 3)
            {
                bot.sendMessage(user_id, 'Некорректный callback.');
                return;
            }
            
            var lesson_part_id = data_parts[2] - 0;

            switch(data_parts[0])
            {
                case Prefixes.LessonTheory:
                    SendTheory(user, lesson_id, lesson_part_id);
                    return;
                case Prefixes.LessonTest:
                    SendTest(user, lesson_id, lesson_part_id);
                    return;
            }

            if(data_parts[0] == Prefixes.LessonTestAnswer)
            {
                if(data_parts.length < 4)
                {
                    bot.sendMessage(user_id, 'Некорректный callback.');
                    return;
                }
                var answer = data_parts[3] - 0;
                SendTestAnswer(user, lesson_id, lesson_part_id, answer);
                return;
            }
        }        
    }

    bot.sendMessage(user_id, 'Некорректный callback.')
});