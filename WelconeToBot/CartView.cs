namespace WelconeToBot
{
    public class CartView
    {
        private Cart _numberPath;
        private Cart _propertyPath;

        public int Number { get => _numberPath.Number; }
        public string Property { get => GetPropertyString(_propertyPath.Type); }

        public CartView(Cart numberPath, Cart propertyPath)
        {
            _numberPath = numberPath;
            _propertyPath = propertyPath;
        }

        public override string ToString()
        {
            return $"{Number} - {Property}";
        }

        private string GetPropertyString(CartEffect effect)
        {
            switch (effect)
            {
                case CartEffect.Pool:
                    return "Бассейн";
                case CartEffect.Forest:
                    return "Сквер";
                case CartEffect.Fence:
                    return "Забор";
                case CartEffect.Cost:
                    return "Агент";
                case CartEffect.Worker:
                    return "Рабочий";
                case CartEffect.Post:
                    return "Строение";
                default:
                    return string.Empty;
            }
        }
    }
}
