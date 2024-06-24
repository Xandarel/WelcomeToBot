namespace WelconeToBot
{
    public class CartView
    {
        private Cart _numberPath;
        private Cart _propertyPath;

        public int Number { get => _numberPath.Number; }
        public CartEffect Property { get => _propertyPath.Type; }

        public CartView(Cart numberPath, Cart propertyPath)
        {
            _numberPath = numberPath;
            _propertyPath = propertyPath;
        }
    }
}
