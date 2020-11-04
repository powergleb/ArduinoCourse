const TelegramBot = require("node-telegram-bot-api");
const { TOKEN } = require('./items/secret');

const bot = new TelegramBot(TOKEN, { polling: true });

var question = {
    title: 'Сколько параметров можно передать функции ?',
    buttons: [
        [{ text: 'Ровно столько, сколько указано в определении функции.', callback_data: '1' }],
        [{ text: 'Сколько указано в определении функции или меньше.', callback_data: '2' }],
        [{ text: 'Сколько указано в определении функции или больше.', callback_data: '3' }],
        [{ text: 'Любое количество.', callback_data: '4' }]
    ],
    right_answer: 4
}

const {Menu} = require('./items/main_menu');

var users = [];

const Prefixes = {
    LessonAtMenu: 'lm',
    // TO DO
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
    }

    users.push(user);

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

bot.onText(/\/start/, (msg, match) => {
    var address = msg.chat.id;
    var text = Menu.Text;
    var user = GetUser(msg.from.id);
    if(user == null)
    {        
        text = 'Добро пожаловать, ' + msg.from.first_name + ' ' + msg.from.last_name + '\n' + text;
        user = CreateUser(msg.from.id);
    }

    var buttons = [];

    for(var i = 0; i <= user.actual_lesson; i++)
    {
        var button = {
            text: Menu.Lessons[i].Title, 
            callback_data: i 
        }

        buttons.push([button]);
    }

    var options = ButtonsToOptions(buttons);

    bot.sendMessage(address, text, options)
});

bot.on('callback_query', (msg) => {
    var user_id = msg.from.id;
    var user = GetUser(user_id);
    if(user == null)
    {
        bot.sendMessage(user_id, 'You are not registered! User \'/start\' message.');
    }

    var message_data = msg.data;
    var data_parts = message_data.split('_');
    
    if(data_parts.length < 1) 
    {
        bot.sendMessage(user_id, 'Incorrect callback.');
    }

    // TO DO

    bot.sendMessage(msg.from.id, message_data)
});