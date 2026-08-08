using System.ComponentModel;

namespace WelconeToBot
{
    public enum CartEffect
    {
        [Description("Бассейн")]
        Pool,
        [Description("Сквер")]
        Forest,
        [Description("Забор")]
        Fence,
        [Description("Агент")]
        Cost,
        [Description("Рабочий")]
        Worker,
        [Description("Строение")]
        Post,
    }

}