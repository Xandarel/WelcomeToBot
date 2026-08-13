using WelcomeTo.DAL.Entities;

namespace WelconeToBot
{
    public interface IGame<T>
    {
        IEnumerable<T> CurrentCart { get; }
        IEnumerable<Quest> CurrentQuest { get; }
        void NewGame(int gameMode = 0);
        IEnumerable<T> NextTurn();
        void ShufleDecks();
    }
}