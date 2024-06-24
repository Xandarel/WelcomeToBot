using Newtonsoft.Json;
using System;

namespace WelconeToBot
{
    public class CardsManager
    {
        private List<Cart> _carts = [];
        private List<List<Cart>> _decks = [];
        private List<Quest> _quests = [];

        public Tuple<CartView, CartView, CartView> CurrentCart { get; private set; }

        public Tuple<Quest, Quest, Quest> CurrentQuest { get; private set; }

        public CardsManager()
        {
            //TODO: Дописать получение строки из json конфига
            //@"C:\Users\Bordyug_ao\source\repos\WelconeToBot\WelconeToBot\WelcomeTo.json"
            //@"C:\Users\Bordyug_ao\source\repos\WelconeToBot\WelconeToBot\QuestCarts.json"
            LoadCarts(@"C:\Users\Bordyug_ao\source\repos\WelconeToBot\WelconeToBot\WelcomeTo.json");
            LoadQuests(@"C:\Users\Bordyug_ao\source\repos\WelconeToBot\WelconeToBot\QuestCarts.json");
        }

        public Tuple<CartView, CartView, CartView> NextTurn()
        {
            if (_decks[0].Count < 2)
            {
                ShufleDecks();
            }
            var turnCart = _decks.ConvertAll(x => x.Take(2))
                                 .ConvertAll(x => new CartView(x.Last(), x.First()));
            foreach (var carts in _decks)
            {
                carts.Remove(carts.FirstOrDefault());
            }
            CurrentCart = Tuple.Create(turnCart[0], turnCart[1], turnCart[2]);

            return CurrentCart;
        }

        public void ShufleDecks()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            _carts = _carts.OrderBy(_ => random.Next()).ToList();
            for (var i = 0; i < 3; i++)
            {
                _decks.Add(_carts.Skip(27 * i).Take(27).ToList());
            }
        }

        private void LoadCarts(string path)
        {
            using StreamReader cr = new(path);
            using JsonReader r = new JsonTextReader(cr);
            var json = cr.ReadToEnd();
            _carts = JsonConvert.DeserializeObject<List<Cart>>(json);
            ShufleDecks();
        }


        private void LoadQuests(string path)
        {
            using StreamReader cr = new(path);
            using JsonReader r = new JsonTextReader(cr);
            var json = cr.ReadToEnd();
            _quests = JsonConvert.DeserializeObject<List<Quest>>(json);
            ChoseQuests();
        }

        private void ChoseQuests()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            var quests = new List<Quest>();
            for (var i = 1; i <= 3; i++)
            {
                quests.Add(_quests.Where(x => x.QuestType == i).OrderBy(_ => random.Next()).FirstOrDefault());
            }
            CurrentQuest = Tuple.Create(quests[0], quests[1], quests[2]);
        }
    }
}
