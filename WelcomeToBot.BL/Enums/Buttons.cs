using System.ComponentModel;

namespace WelcomeToBot.BL.Enums
{
    public enum Buttons
    {
        [Description("New Game")]
        NewGame,
        [Description("Next Turn")]
        NextTurn,

        [Description("Quests")]
        Quests,
        [Description("Shuffle Deck")]
        Shuffle
    }
}
