using Microsoft.Extensions.Logging;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WelconeToBot;

namespace MyTelegramBot.Handlers;

public class UpdateHandler(ILogger<UpdateHandler> logger, IGame<CartView> deck) : IUpdateHandler
{
    private Dictionary<long, IGame<CartView>> _clientGame = new();

    public async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {

        long chatId = update.Message.Chat.Id;
        IGame<CartView> manager;
        if (_clientGame.ContainsKey(chatId))
        {
            manager = _clientGame[chatId];
        }
        else
        {
            manager = deck;
            _clientGame.Add(chatId, deck);
        }

        // Обработка в зависимости от типа обновления
        Task handler = update.Type switch
        {
            UpdateType.Message => HandleMessageAsync(botClient, update.Message!, cancellationToken, manager),
            UpdateType.CallbackQuery => HandleCallbackAsync(botClient, update.CallbackQuery!, cancellationToken),
            UpdateType.InlineQuery => HandleInlineQuery(botClient, update.InlineQuery!, cancellationToken),
            _ => Task.CompletedTask
        };

        await handler;
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Ошибка при обработке обновления");
        await Task.CompletedTask;
    }

    private ReplyKeyboardMarkup CreateKeyboard()
    {
        return new(
        [
            ["New Game"],
            ["Next Turn"],
            ["Quests"],
            ["Shuffle Deck"]
        ])
        {
            ResizeKeyboard = true
        };
    }

    // ── Обработчики конкретных типов ──
    private async Task HandleMessageAsync(
            ITelegramBotClient botClient,
        Message message,
        CancellationToken ct,
        IGame<CartView> game)
    {
        // Игнорируем не-текстовые сообщения
        if (message.Text is not { } text)
            return;

        // Простая обработка команд
        Task action = text.Trim(' ') switch
        {
            "New Game" => HandleNewGameCommand(botClient, message, game, ct),
            "Next Turn" => HandleNextTurnCommand(botClient, message, game, ct),
            "Quests" => HandleQuestCommand(botClient, message, game, ct),
            "Shuffle Deck" => HandkeShuffleCommand(botClient, message, game, ct),
            _ => EchoAsync(botClient, message, ct)
        };

        await action;
    }

    private async Task HandkeShuffleCommand(ITelegramBotClient botClient, Message message, IGame<CartView> game, CancellationToken cancellationToken)
    {
        game.ShufleDecks();
        game.NextTurn();
        StringBuilder sb = new();
        foreach (CartView item in game.CurrentCart)
            sb.AppendLine(item.ToString());
        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: sb.ToString(),
            replyMarkup: CreateKeyboard(),
            cancellationToken: cancellationToken);
    }

    private async Task HandleQuestCommand(ITelegramBotClient botClient, Message message, IGame<CartView> game, CancellationToken cancellationToken)
    {
        StringBuilder sb = new();
        foreach (Quest item in game.CurrentQuest)
            sb.AppendLine(item.ToString());
        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: sb.ToString(),
            replyMarkup: CreateKeyboard(),
            cancellationToken: cancellationToken);
    }

    private async Task HandleNewGameCommand(ITelegramBotClient bot, Message message, IGame<CartView> game, CancellationToken cancellationToken)
    {
        game.NewGame();
        game.NextTurn();
        StringBuilder sb = new();
        foreach (CartView item in game.CurrentCart)
            sb.AppendLine(item.ToString());
        foreach (Quest item in game.CurrentQuest)
            sb.AppendLine(item.ToString());
        await bot.SendMessage(
            chatId: message.Chat.Id,
            text: sb.ToString(),
            replyMarkup: CreateKeyboard(),
            cancellationToken: cancellationToken);
    }

    private async Task HandleNextTurnCommand(ITelegramBotClient bot, Message message, IGame<CartView> game, CancellationToken cancellationToken)
    {
        game.NextTurn();
        StringBuilder sb = new();
        foreach (CartView item in game.CurrentCart)
            sb.AppendLine(item.ToString());
        await bot.SendMessage(
            chatId: message.Chat.Id,
            text: sb.ToString(),
            replyMarkup: CreateKeyboard(),
            cancellationToken: cancellationToken);
    }

    private async Task EchoAsync(ITelegramBotClient bot, Message msg, CancellationToken ct)
    {
        await bot.SendMessage(
            chatId: msg.Chat.Id,
            text: $"Вы сказали: {msg.Text}",
            cancellationToken: ct);
    }

    private async Task HandleCallbackAsync(
        ITelegramBotClient bot,
        CallbackQuery callback,
        CancellationToken ct)
    {
        await bot.AnswerCallbackQuery(callback.Id, "Кнопка нажата!", cancellationToken: ct);
        await bot.SendMessage(
            chatId: callback.Message!.Chat.Id,
            text: $"Вы нажали: {callback.Data}",
            cancellationToken: ct);
    }

    private Task HandleInlineQuery(
        ITelegramBotClient bot,
        InlineQuery inlineQuery,
        CancellationToken ct)
    {
        // Обработка inline-запросов
    }
}
