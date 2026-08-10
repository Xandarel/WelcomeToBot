using WelcomeToBot.BL.Enums;
using WelcomeToBot.BL.Extension;

namespace WelconeToBot
{
    public class CartView(Cart numberPath, Cart propertyPath)
    {
        public int Number { get => numberPath.Number; }
        public string Property { get => GetPropertyString(propertyPath.Type); }

        public override string ToString() => $"{Number} - {Property}";

        private static string GetPropertyString(CartEffect effect) => effect.GetDescription();
    }
}
