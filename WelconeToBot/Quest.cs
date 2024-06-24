namespace WelconeToBot
{
    public class Quest
    {
        public string Description { get; set; }

        public int FirstCompletePointed { get; set; }
        public int SecondCompletePointed { get; set; }
        public int QuestType { get; set; }

        public override string ToString()
        {
            return $"Задание № {QuestType}. {Description}. Первое выполнение - {FirstCompletePointed} очков. Следующие - {SecondCompletePointed}";
        }
    }
}
