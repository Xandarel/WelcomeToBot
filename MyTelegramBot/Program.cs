using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MyTelegramBot.Handlers;
using MyTelegramBot.Options;
using MyTelegramBot.Services;
using Telegram.Bot;
using WelconeToBot;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHttpClient();
        services.Configure<TelegramSettingsOptions>(
            context.Configuration.GetSection(nameof(TelegramSettingsOptions)));

        // Регистрируем TelegramBotClient как Singleton — один экземпляр на всё приложение
        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            TelegramSettingsOptions options = sp.GetRequiredService<IOptions<TelegramSettingsOptions>>().Value;
            return new TelegramBotClient(options.ApiKey);
        });

        // UpdateHandler — может быть Singleton или Transient (Singleton рекомендую)
        services.AddSingleton<UpdateHandler>();

        // Фоновый сервис, который запускает поллинг
        services.AddHostedService<BotHostedService>();
        services.AddTransient<IGame<CartView>, CardsManager>();
    })
    .Build();

await host.RunAsync();
