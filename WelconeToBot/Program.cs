// See https://aka.ms/new-console-template for more information
using WelconeToBot;

var test = new CardsManager();
var turn = test.NextTurn();
var quest = test.CurrentQuest;
Console.WriteLine(quest.Item1);
Console.WriteLine(quest.Item2);
Console.WriteLine(quest.Item3);
Console.ReadKey();
