// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WelconeToBot;

App.Run();

public static class App
{
    private static IServiceCollection _services;
    public static IServiceProvider ServiceProvider { get => _services.BuildServiceProvider(); }
    public static void Run()
    {
        IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            .AddJsonFile("appSettings.json")
            .Build();
        _services = new ServiceCollection()
        .AddSingleton<IGame<CartView>, CardsManager>()
        .AddSingleton<IConfiguration>(configuration);
    }
}
