using Telegram.Bot;

namespace TelegramBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Task.Run(BotStart);
            BotStart();
        }

        static void BotStart()
        {
            var botClient = new TelegramBotClient("7314075768:AAGriKB8GNlW9xSfpbYb45IV-lbbbjmSMg4");
            var me = botClient.GetMeAsync();
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {11}.");
        }
    }
}
