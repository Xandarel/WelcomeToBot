
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public interface IBot
    {
        TelegramBotClient TelegramBotClient { get; }
        ReceiverOptions ReceiverOptions { get; }
        Task Start();
        Task<User> GetMeAsync();
        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken);
        Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
    }
}
