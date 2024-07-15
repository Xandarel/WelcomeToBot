// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WelconeToBot;

IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName).AddJsonFile("appSettings.json").Build();
IServiceCollection services = new ServiceCollection()
    .AddSingleton<IGame<CartView>, CardsManager>()
    .AddSingleton<IConfiguration>(configuration);

var testservice = services.BuildServiceProvider();
var method = testservice.GetService<IGame<CartView>>();
