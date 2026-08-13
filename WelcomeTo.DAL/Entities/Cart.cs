using WelcomeTo.DAL.Enums;

namespace WelcomeTo.DAL.Entities
{
    public sealed class Cart
    {
        public long Id { get; set; }
        public int Number { get; set; }
        public CartEffect Type { get; set; }
    }
}