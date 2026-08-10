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
        Shuffle,

        [Description("Базовая Версия")]
        BaseGame = 0,

        [Description("Пасхальные яйца")]
        EasterEggsGame = 4,

        [Description("Фургон с мороженным")]
        IceCreamTruckGame = 5,

        [Description("Хеллоуин")]
        HalloweenGame = 6,

        [Description("Рождественские огоньки")]
        ChristmasLightsGame = 7,

        [Description("Судный день")]
        JudgementDayGame = 8,
    }
}
