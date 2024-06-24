namespace WelconeToBot
{

    public enum CartEffect
    {
        Forest,
        Post,
        Worker,
        Fence,
        Pool,
        Cost
    }

    public class Cart
    {
        public int Number { get; set; }
        public CartEffect Type { get; set; }
    }
}