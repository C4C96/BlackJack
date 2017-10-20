using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Linq;

namespace BlackJackLib
{
	public class Game
	{
		private const int DEFAULT_BALANCE = 1000;

		private Dealer dealer;
		private List<Player> players;
		private Deck deck;

		public Game(int playerNumber = 1)
		{
			if (playerNumber < 1)
				throw new ArgumentOutOfRangeException("playerNumber", "playerNumber should be a positive integer");

			dealer = new Dealer(DEFAULT_BALANCE);
			players = new List<Player>(playerNumber);
			for (int i = 1; i <= playerNumber; i++)
				players.Add(new Player(i, DEFAULT_BALANCE));
			deck = new Deck();
		}

		/// <summary>
		/// 开始下一轮赌局
		/// </summary>
		public void NextTurn()
		{
			// 每局开始的初始化
			deck.Shuffle();
			dealer.Init();
			players.ForEach(player => player.Init());

			Console.WriteLine("开始新的一轮");
			Console.WriteLine("-----------------------------------------------------------");

			// 玩家下注
			foreach (var player in players)
			{
				Console.WriteLine($"{player.Id}号玩家清下注：");
				int bet = int.Parse(Console.ReadLine());
				player.Stake = bet;
				player.Balance -= bet;
			}

			// 依次发初始两张牌（庄家一明一暗，闲家均为明，后续的发牌均为明牌）
			players.ForEach(player => player.AchieveCard(deck.DrawACard(true)));
			players.ForEach(player => player.AchieveCard(deck.DrawACard(true)));
			Card dealer_SeenCard = deck.DrawACard(true);
			Card dealer_BlindCard = deck.DrawACard(false);
			dealer.AchieveCard(dealer_SeenCard);
			dealer.AchieveCard(dealer_BlindCard);
			
			Console.WriteLine("-----------------------------------------------------------");

			// 若庄家明T(10,J,Q或K)暗A，则亮牌
			if (dealer_SeenCard.Rank >= Rank.Ten && dealer_BlindCard.Rank == Rank.Ace)
				dealer_BlindCard.Seen_Blind = true;
			// 若庄家明A
			else if (dealer_SeenCard.Rank == Rank.Ace)
			{
				// 询问玩家是否买保险金
				foreach (var player in players)
				{
					Console.WriteLine($"{player.Id}号玩家是否需要购买保险金(Y/N)：");
					string input = Console.ReadLine();
					bool wantInsurance = input.Contains("Y") || input.Contains("y");
					if (wantInsurance)
						player.BuyInsurance();
				}
				//若此时庄家暗T，构成BlackJack，亮牌
				if (dealer_BlindCard.Rank >= Rank.Ten)
					dealer_BlindCard.Seen_Blind = true;
			}
			
			Console.WriteLine("-----------------------------------------------------------");

			if (!dealer.HasBlackJack)
			{
				foreach (var player in players)
					// 玩家BlackJack，庄家不是
					if (player.HasBlackJack)
						PlayerWin(player, 1.5f); // 玩家以1.5倍率赢
			}
			else
				foreach (var player in players)
					if (player.HasBlackJack) // 都有BlackJack
						Draw(player); // 平手
					else // 庄家有BlackJack，玩家没有
					{
						if (player.Insurance == null) // 没买保险
							DealerWin(player);
						else // 买了保险
							Draw(player); // 按平局处理
					}

			Console.WriteLine("-----------------------------------------------------------");
					
			// 询问剩余玩家是否double
			foreach (var player in players.Where(player => player.Active))
			{
				Console.WriteLine($"{player.Id}号玩家是否需要double(Y/N)：");
				string input = Console.ReadLine();
				bool wantToDouble = input.Contains("Y") || input.Contains("y");
				if (wantToDouble)
				{
					player.Double();
					player.AchieveCard(deck.DrawACard());
					if (player.SumPoint > 21)
					{
						Console.WriteLine($"{player.name}的手牌：{player.HandCardsToString()}，爆了");
						DealerWin(player);
					}
				}
			}
			
			Console.WriteLine("-----------------------------------------------------------");

			// 每轮的选择
			foreach (var player in players.Where(player => player.Active))
			{
				do
				{
                    //判断玩家是否五小龙
                    if(player.HandCards.Count==5)
                    {
                        Console.WriteLine($"{player.name}已经有五张手牌：{player.HandCardsToString()}");
                        int if_five_dragon = 0;
                        foreach (var cards in player.HandCards)
                        {
                            if_five_dragon += (int)cards.Rank;
                        }

                        if (if_five_dragon<21)
                        {
                            Console.WriteLine($"{player.name}五小龙了！");
                            PlayerWin(player);
                        }
                    }



					Console.WriteLine($"{player.Id}号玩家请选择：1.拿牌	2.停牌");
					int input = int.Parse(Console.ReadLine());
					// 拿牌
					if (input == 1)
					{
						player.AchieveCard(deck.DrawACard());
						// 玩家爆了，则庄家直接赢
						if (player.SumPoint > 21)
						{
							Console.WriteLine($"{player.name}的手牌：{player.HandCardsToString()}，爆了");
							DealerWin(player);
						}
					}
					// 停牌
					else
						player.Active = false;
				} while (player.Active);
			}

			Console.WriteLine("-----------------------------------------------------------");
			
			// 庄家亮暗牌，并持续拿牌直到点数不小于17
			dealer_BlindCard.Seen_Blind = true;
			while (dealer.SumPoint < 17 && dealer.HandCards.Count<5)    //最多只能摸五张牌
				dealer.AchieveCard(deck.DrawACard());

			Console.WriteLine("-----------------------------------------------------------");

            // 结算
            // 若庄家爆了
            if (dealer.SumPoint > 21)
            {
                Console.WriteLine($"{dealer.name}的手牌：{dealer.HandCardsToString()}，爆了");
                foreach (var player in players.Where(player => !player.Finish))
                    PlayerWin(player); // 玩家赢
            }
            else // 否则先判断五小龙，再比大小
            {
                if (dealer.HandCards.Count == 5 && dealer.SumPoint < 21)  //判断庄家五小龙
                {
                    Console.WriteLine($"{dealer.name}五小龙了！玩家全都炸了");
                    foreach (var player in players)
                    {
                        DealerWin(player);
                    }
                }

                foreach (var player in players.Where(player => !player.Finish))  //庄家没有五小龙，依次比大小
                {
                    if (player.SumPoint > dealer.SumPoint)
                        PlayerWin(player);
                    else if (player.SumPoint < dealer.SumPoint)
                        DealerWin(player);
                    else
                        Draw(player);
                }

            }
				
			Console.WriteLine("-----------------------------------------------------------");
		}

		/// <summary>
		/// 庄家赢，则庄家获得保险金（若有）和赌金
		/// </summary>
		/// <param name="player">庄家赢的玩家</param>
		private void DealerWin(Player player)
		{
			if (player.Insurance != null)
				dealer.Balance += player.Insurance.Value;
			dealer.Balance += player.Stake;
			player.Finish = true;

			Console.WriteLine($"{player.name}输了，还有金额：{player.Balance}，庄家还有金额：{dealer.Balance}");
		}

		/// <summary>
		/// 玩家赢，则庄家获得保险金（若有），玩家收回赌金，并从庄家那获得赌金若干倍率的奖金
		/// </summary>
		/// <param name="player">赢庄家的玩家</param>
		/// <param name="rate">奖金倍率</param>
		private void PlayerWin(Player player, float rate = 1.0f)
		{
			if (player.Insurance != null)
				dealer.Balance += player.Insurance.Value;
			player.Balance += player.Stake;
			dealer.Balance -= (int)(player.Stake * rate);
			player.Balance += (int)(player.Stake * rate);
			player.Finish = true;

			Console.WriteLine($"{player.name}赢了，还有金额：{player.Balance}，庄家还有金额：{dealer.Balance}");
		}

		/// <summary>
		/// 平局，则庄家获得保险金（若有），玩家收回赌金
		/// </summary>
		/// <param name="player">和庄家平局的玩家</param>
		private void Draw(Player player)
		{
			if (player.Insurance != null)
				dealer.Balance += player.Insurance.Value;
			player.Balance += player.Stake;
			player.Finish = true;

			Console.WriteLine($"{player.name}平了，还有金额：{player.Balance}，庄家还有金额：{dealer.Balance}");
		}
	}
}
