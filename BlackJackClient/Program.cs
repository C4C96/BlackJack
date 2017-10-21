using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackJackLib;

namespace BlackJackClient
{
	class Program
	{
		static void Main(string[] args)
		{
			Game game = new Game(new Interaction(), 2);
			game.NewTurnStart += OnNewTurnStart;
			game.AchieveCard += OnAchieveCard;
			game.GamerBoom += OnGamerBoom;
			game.Finish += OnFinish;
			while (true)
				game.NextTurn();
		}

		static void OnNewTurnStart(object o, EventArgs e)
		{
			Console.WriteLine("-------------------------------------");
			Console.WriteLine("开始新的一局");
		}

		private static void OnAchieveCard(object sender, Gamer gamer, Card card)
		{
			Console.WriteLine($"{GetGamerName(gamer)}拿到了{card}");
			Console.WriteLine($"{GetGamerName(gamer)}现在的手牌是{gamer:C}");
		}

		static void OnGamerBoom(object sender, Gamer e)
		{
			Console.WriteLine($"{GetGamerName(e)}爆了");
		}

		private static void OnFinish(object sender, Player player, bool? win)
		{
			switch (win)
			{
				case null:
					Console.WriteLine($"{GetGamerName(player)}平了庄家");
					break;
				case true:
					Console.WriteLine($"{GetGamerName(player)}赢了庄家");
					break;
				case false:
					Console.WriteLine($"{GetGamerName(player)}输给了庄家");
					break;
			}
			Console.WriteLine($"{GetGamerName(player)}还有余额：{player.Balance}");
			Console.WriteLine($"庄家还有余额：{(sender as Game).Dealer.Balance}");
		}

		static string GetGamerName(Gamer gamer)
		{
			if (gamer is Dealer)
				return "庄家";
			else
				return (gamer as Player).Id + "号玩家";
		}

		class Interaction : IGameInteraction
		{
			public int Bet(int playerId)
			{
				Console.WriteLine($"{playerId}号玩家请输入赌金：");
				int bet = int.Parse(Console.ReadLine());
				// 判断合法且为正数
				return bet;
			}

			public bool WantInsurance(int playerId)
			{
				Console.WriteLine($"{playerId}号玩家是否需要购买保险金(Y/N)：");
				string input = Console.ReadLine();
				return input.Contains("Y") || input.Contains("y");
			}

			public bool WantToDouble(int playerId)
			{
				Console.WriteLine($"{playerId}号玩家是否需要加倍(Y/N)：");
				string input = Console.ReadLine();
				return input.Contains("Y") || input.Contains("y");
			}

			public bool WantToHitMe(int playerId)
			{
				Console.WriteLine($"{playerId}号玩家：1.要牌  2.停牌");
				string input = Console.ReadLine();
				return input.Contains("1");
			}
		}
	}
}
