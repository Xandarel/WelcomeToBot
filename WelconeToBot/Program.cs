// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WelconeToBot;

IServiceCollection services = new ServiceCollection()
    .AddSingleton<IGame<CartView>, CardsManager>()
    .AddSingleton<IConfigurationBuilder, ConfigurationBuilder>();
using var serviceProvider = services.BuildServiceProvider();
var builder = serviceProvider!.GetService<IConfigurationBuilder>().AddJsonFile(@"C:\Users\user\source\repos\welcomeToBot\WelcomeToBot\WelconeToBot\appSettings.json", false, true);