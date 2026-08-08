using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WelcomeToBot.BL.Enums;
using WelcomeToBot.BL.Extension;
using WelconeToBot;

namespace MyTelegramBot.Handlers;

public class UpdateHandler(ILogger<UpdateHandler> logger, IGame<CartView> deck) : IUpdateHandler
{
    private Dictionary<long, IGame<CartView>> _clientGame = new();
    private readonly ReplyKeyboardMarkup _gameVersionKeyboardMarkup = new(
        [
            ["Базовая Версия"],
            ["Пасхальные яйца"],
            ["Фургон с мороженным"],
            ["Хеллоуин"],
            ["Рождественские огоньки"],
            ["Судный день"]
        ])
        {
            ResizeKeyboard = true
        };

    private readonly ReplyKeyboardMarkup _replyKeyboardMarkup = new(
        [
            [Buttons.NewGame.GetDescription()],
            [Buttons.NextTurn.GetDescription()],
            [Buttons.Quests.GetDescription()],
            [Buttons.Shuffle.GetDescription()]
        ])
    {
        ResizeKeyboard = true
    };


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
            _ => Task.CompletedTask
        };

        await handler;
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Ошибка при обработке обновления");
        await Task.CompletedTask;
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
            "Shuffle Deck" => HandleShuffleCommand(botClient, message, game, ct),
            "Базовая Версия" => HandleStartGame(botClient, message, game, 0, ct),
            "Пасхальные яйца" => HandleStartGame(botClient, message,game, 4, ct),
            "Фургон с мороженным" => HandleStartGame(botClient, message, game, 5, ct),
            "Хеллоуин" => HandleStartGame(botClient, message, game, 6, ct),
            "Рождественские огоньки" => HandleStartGame(botClient, message, game, 7, ct),
            "Судный день" => HandleStartGame(botClient, message, game, 8, ct),
            _ => EchoAsync(botClient, message, ct)
        };

        await action;
    }

    private async Task HandleStartGame(ITelegramBotClient botClient, Message message, IGame<CartView> game, int gameMode, CancellationToken cancellationToken)
    {
        game.NewGame(gameMode);
        game.NextTurn();
        StringBuilder sb = new();
        foreach (var item in game.CurrentCart)
            sb.AppendLine(item.ToString());
        foreach (var item in game.CurrentQuest)
            sb.AppendLine(item.ToString());
        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: sb.ToString(),
            replyMarkup: _replyKeyboardMarkup,
            cancellationToken: cancellationToken);
    }

    private async Task HandleShuffleCommand(ITelegramBotClient botClient, Message message, IGame<CartView> game, CancellationToken cancellationToken)
    {
        game.ShufleDecks();
        game.NextTurn();
        StringBuilder sb = new();
        foreach (CartView item in game.CurrentCart)
            sb.AppendLine(item.ToString());
        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: sb.ToString(),
            replyMarkup: _replyKeyboardMarkup,
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
            replyMarkup: _replyKeyboardMarkup,
            cancellationToken: cancellationToken);
    }

    private async Task HandleNewGameCommand(ITelegramBotClient bot, Message message, IGame<CartView> game, CancellationToken cancellationToken)
    {
        game.NewGame();
        game.NextTurn();
        StringBuilder sb = new();
        sb.Append("Выберите версию игры");
        await bot.SendMessage(
            chatId: message.Chat.Id,
            text: sb.ToString(),
            replyMarkup: _gameVersionKeyboardMarkup,
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
            replyMarkup: _replyKeyboardMarkup,
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
}
