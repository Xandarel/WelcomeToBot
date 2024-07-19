using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using WelconeToBot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace TelegramBot
{
    //TODO: NOT USED
    public class TelegtamBot //: IBot
    {
        private static Dictionary<long, IGame<CartView>> _clientGame = [];
        private IConfiguration _configuration;
        public static TelegramBotClient TelegramBotClient { get; private set; }
        public ReceiverOptions ReceiverOptions { get; private set; } = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };
        public TelegtamBot(IConfiguration configuration)
        {
            _configuration = configuration;
            TelegramBotClient = new TelegramBotClient(_configuration.GetSection("BotKey").Value);
        }

        public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;
            IGame<CartView> manager = null;
            if (_clientGame.ContainsKey(chatId))
            {
                manager = _clientGame[chatId];
            }
            else
            {
                manager = App.ServiceProvider.GetService<IGame<CartView>>();
                _clientGame.Add(chatId, manager);
            }

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "New Game" },
                new KeyboardButton[] { "NextTurn" },
                new KeyboardButton[] { "Quests"},
                new KeyboardButton[] { "Shufle Deck"}
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
                case "Shufle Deck":
                    manager.ShufleDecks();
                    manager.NextTurn();
                    foreach (var item in manager.CurrentCart)
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
        public async Task Start()
        {
            using CancellationTokenSource cancellationToken = new();

            TelegramBotClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: ReceiverOptions,
                cancellationToken: cancellationToken.Token
            );
            cancellationToken.Cancel();
        }

        public async Task<User> GetMeAsync()
        {
            return await TelegramBotClient.GetMeAsync();
        }
    }
}
