using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WelconeToBot;


var clientGame = new Dictionary<long, CardsManager>();
var botClient = new TelegramBotClient("7314075768:AAGriKB8GNlW9xSfpbYb45IV-lbbbjmSMg4");

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();
Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;
    CardsManager manager;
    if (clientGame.ContainsKey(chatId))
    {
        manager = clientGame[chatId];
    }
    else
    {
        manager = new CardsManager();
        clientGame.Add(chatId, manager);
    }

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
{
    new KeyboardButton[] { "New Game" },
    new KeyboardButton[] { "NextTurn" },
    new KeyboardButton[] { "Quests"}
})
    {
        ResizeKeyboard = true
    };
    var sb = new StringBuilder();
    Message sentMessage;
    switch (messageText)
    {
        case "New Game":
            manager.NewGame();
            manager.NextTurn();
            foreach (var item in manager.CurrentCart)
                sb.AppendLine(item.ToString());
            foreach (var item in manager.CurrentQuest)
                sb.AppendLine(item.ToString());
            sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: sb.ToString(),
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
            break;
        case "NextTurn":
            manager.NextTurn();
            foreach (var item in manager.CurrentCart)
                sb.AppendLine(item.ToString());
            sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: sb.ToString(),
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
            break;
        case "Quests":
            foreach (var item in manager.CurrentQuest)
                sb.AppendLine(item.ToString());
            sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: sb.ToString(),
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
            break;
        default:
            sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "This is no command",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
            break;


    }
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}