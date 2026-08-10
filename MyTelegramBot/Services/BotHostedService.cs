using Microsoft.Extensions.Hosting;
using MyTelegramBot.Handlers;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace MyTelegramBot.Services;

public class BotHostedService : IHostedService
{
    private readonly ITelegramBotClient _botClient;
    private readonly UpdateHandler _updateHandler;
    private CancellationTokenSource? _cts;

    public BotHostedService(ITelegramBotClient botClient, UpdateHandler updateHandler)
    {
        _botClient = botClient;
        _updateHandler = updateHandler;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _botClient.DeleteWebhook(cancellationToken: cancellationToken);
        _cts = new CancellationTokenSource();

        _botClient.StartReceiving(
            updateHandler: _updateHandler,
            receiverOptions: new ReceiverOptions
            {
                AllowedUpdates = { } // получаем все типы обновлений
            },
            cancellationToken: _cts.Token
        );
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts?.Cancel();

        return Task.CompletedTask;
    }
}
