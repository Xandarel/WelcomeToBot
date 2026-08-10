using Microsoft.Extensions.Logging;
using System.Text;
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
    private readonly ReplyKeyboardMarkup _gameVersionKeyboardMarkup = new(
        [
            [Buttons.BaseGame.GetDescription()],
            [Buttons.EasterEggsGame.GetDescription()],
            [Buttons.IceCreamTruckGame.GetDescription()],
            [Buttons.HalloweenGame.GetDescription()],
            [Buttons.ChristmasLightsGame.GetDescription()],
            [Buttons.JudgementDayGame.GetDescription()]
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

        Buttons? button = EnumExtensions.ParseByDescription<Buttons>(text);
        if (button is not null)
        {
            await (button.Value switch
            {
                Buttons.NewGame => HandleNewGameCommand(botClient, message, game, ct),
                Buttons.NextTurn => HandleNextTurnCommand(botClient, message, game, ct),
                Buttons.Quests => HandleQuestCommand(botClient, message, game, ct),
                Buttons.Shuffle => HandleShuffleCommand(botClient, message, game, ct),
                Buttons.BaseGame or
                Buttons.EasterEggsGame or
                Buttons.IceCreamTruckGame or
                Buttons.HalloweenGame or
                Buttons.ChristmasLightsGame or
                Buttons.JudgementDayGame
                    => HandleStartGame(botClient, message, game, (int)button.Value, ct),
                _ => RepeatComand(botClient, message, ct),
            });
            return;
        }
    }

    private async Task HandleStartGame(ITelegramBotClient botClient, Message message, IGame<CartView> game, int gameMode, CancellationToken cancellationToken)
    {
        game.NewGame(gameMode);
        game.NextTurn();
        StringBuilder sb = new();
        foreach (CartView item in game.CurrentCart)
            sb.AppendLine(item.ToString());
        foreach (Quest item in game.CurrentQuest)
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

    private async Task RepeatComand(ITelegramBotClient bot, Message msg, CancellationToken ct)
    {
        await bot.SendMessage(
            chatId: msg.Chat.Id,
            text: $"Неизвестная команда. Пожуйста, повторите запрос",
            replyMarkup: _replyKeyboardMarkup,
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
