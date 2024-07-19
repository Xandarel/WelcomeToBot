namespace WelconeToBot
{
    public interface IGame<T>
    {
        void NewGame();
        IEnumerable<T> NextTurn();
        IEnumerable<T> CurrentCart {  get; }
        IEnumerable<Quest> CurrentQuest { get; }
        void ShufleDecks();
    }
}