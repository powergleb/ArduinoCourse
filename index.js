const TelegramBot = require("node-telegram-bot-api");
const {TOKEN} = require('./secret');

const bot = new TelegramBot(TOKEN, {polling: true});
  
bot.on('message', (msg) => {
    const chatId = msg.chat.id;
  
    // send a message to the chat acknowledging receipt of their message
    bot.sendMessage(chatId, 'Hello');
});