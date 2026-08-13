using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MyTelegramBot.Handlers;
using MyTelegramBot.Options;
using MyTelegramBot.Services;
using Telegram.Bot;
using WelcomeTo.DAL.Contexts;
using WelcomeToBot.BL.Options;
using WelconeToBot;

IHost host = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(config =>
        {
            config.AddUserSecrets<Program>(); // подцепит UserSecretsId из сборки
        })
    .ConfigureServices(services =>
    {
        services.AddHttpClient();
        services.AddOptions<TelegramSettingsOptions>().BindConfiguration(nameof(TelegramSettingsOptions));
        services.AddOptions<CartPathOptions>().BindConfiguration(nameof(CartPathOptions));

        // Регистрируем TelegramBotClient как Singleton — один экземпляр на всё приложение
        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            TelegramSettingsOptions options = sp.GetRequiredService<IOptions<TelegramSettingsOptions>>().Value;
            return new TelegramBotClient(options.ApiKey);
        });

        // UpdateHandler — может быть Singleton или Transient (Singleton рекомендую)
        services.AddSingleton<UpdateHandler>();
        services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

        // Фоновый сервис, который запускает поллинг
        services.AddHostedService<BotHostedService>();
        services.AddTransient<IGame<CartView>, CardsManager>();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        // Применяем миграции
        using IServiceScope scope = serviceProvider.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    })
    .Build();

await host.RunAsync();
