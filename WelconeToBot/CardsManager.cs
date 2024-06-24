using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace WelconeToBot
{
    public class CardsManager
    {
        private List<Cart> _carts = [];
        private List<List<Cart>> _decks = [];

        public (Cart, Cart, Cart) Current { get; private set; }

        public IReadOnlyCollection<Cart> Carst { get => _carts.AsReadOnly(); }

        public CardsManager()
        {
            using (StreamReader cr =
            new StreamReader(@"C:\Users\Bordyug_ao\source\repos\WelconeToBot\WelconeToBot\WelcomeTo.json"))
            using (JsonReader r = new JsonTextReader(cr))
            {
                var json = cr.ReadToEnd();
                _carts = JsonConvert.DeserializeObject<List<Cart>>(json);
                Shufle();
            }
        }

        public Tuple<CartView, CartView, CartView> NextTurn()
        {
            if (_decks[0].Count < 2)
            {
                Shufle();
            }
            var turnCart = _decks.ConvertAll(x => x.Take(2))
                                 .ConvertAll(x => new CartView(x.Last(), x.First()));
            foreach (var carts in _decks)
            {
                carts.Remove(carts.FirstOrDefault());
            }

            return Tuple.Create(turnCart[0], turnCart[1], turnCart[2]);
        }

        public void Shufle()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            _carts = _carts.OrderBy(_ => random.Next()).ToList();
            for (var i = 0; i < 3; i++)
            {
                _decks.Add(_carts.Skip(27 * i).Take(27).ToList());
            }
        }


    }
}
