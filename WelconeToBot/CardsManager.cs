using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;

namespace WelconeToBot
{
    public class CardsManager : IGame<CartView>
    {
        private List<Cart> _carts = [];
        private List<List<Cart>> _decks = [];
        private List<Quest> _quests = [];
        private IConfiguration _configuration;

        public IEnumerable<CartView> CurrentCart { get; private set; }

        public IEnumerable<Quest> CurrentQuest { get; private set; }

        public CardsManager(IConfiguration configuration)
        {
            _configuration = configuration;
            //TODO: Дописать получение строки из json конфига
            //@"C:\Users\Bordyug_ao\source\repos\WelconeToBot\WelconeToBot\WelcomeTo.json"
            //@"C:\Users\Bordyug_ao\source\repos\WelconeToBot\WelconeToBot\QuestCarts.json"
            LoadCarts(_configuration.GetSection("CartsPath").Value);
            LoadQuests(_configuration.GetSection("QuestsPath").Value);
            NewGame();
        }

        public IEnumerable<CartView> NextTurn()
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
            CurrentCart = turnCart;

            return CurrentCart;
        }

        public void NewGame()
        {
            ShufleDecks();
            ChoseQuests();
        }

        public void ShufleDecks()
        {
            _decks.Clear();
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
        }


        private void LoadQuests(string path)
        {
            using StreamReader cr = new(path);
            using JsonReader r = new JsonTextReader(cr);
            var json = cr.ReadToEnd();
            _quests = JsonConvert.DeserializeObject<List<Quest>>(json);
        }

        private void ChoseQuests()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            var quests = new List<Quest>();
            for (var i = 1; i <= 3; i++)
            {
                quests.Add(_quests.Where(x => x.QuestType == i).OrderBy(_ => random.Next()).FirstOrDefault());
            }
            CurrentQuest = quests;
        }
    }
}
