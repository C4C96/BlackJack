using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace BlackJackLib
{
	public class Game
	{
		public const int DEFAULT_BALANCE = 1000;
		public const int MAX_PLAYER = 5;
		public const int MIN_PLAYER = 1;

		private IGameInteraction interaction; // 用于从用户获取信息的接口实例
		
		public delegate void FinishHandler(object sender, Player player, bool? win);

		// 用于反馈的事件
		public event EventHandler NewTurnStart;
		public event Action<object, Gamer, Card> AchieveCard;
		public event EventHandler<Gamer> GamerBoom;
		public event EventHandler<Gamer> GamerBlackJack;
		public event EventHandler<Gamer> GamerFiveDragon;
		public event FinishHandler Finish;
		public event EventHandler TurnFinish;

		private Dealer dealer;			// 庄家
		private List<Player> players;	// 闲家
		private Deck deck;				// 牌堆

		public Dealer Dealer
		{
			get => dealer;
		}

		public IEnumerable<Player> Players
		{
			get => players;
		}

		public Game(IGameInteraction gameInteraction, int playerNumber = 1)
		{
			if (playerNumber < MIN_PLAYER || playerNumber > MAX_PLAYER)
				throw new ArgumentOutOfRangeException("playerNumber", $"playerNumber should between {MIN_PLAYER} and {MAX_PLAYER}");
			
			// 初始化变量
			interaction = gameInteraction ?? 
				throw new ArgumentOutOfRangeException("gameInteraction", "gameInteraction should not be null");
			dealer = new Dealer(DEFAULT_BALANCE);
			players = new List<Player>(playerNumber);
			for (int i = 1; i <= playerNumber; i++)
				players.Add(new Player(i, DEFAULT_BALANCE));
			deck = new Deck();

			// 绑定事件监听，将事件往上传递
			dealer.AchieveCardEvent += (o, card) =>
			{
				if (AchieveCard != null)
					AchieveCard.Invoke(this, dealer, card);
			};
			players.ForEach(player => player.AchieveCardEvent += (o, card) => 
			{
				if (AchieveCard != null)
					AchieveCard.Invoke(this, player, card);
			});
		}

		/// <summary>
		/// 开始下一轮赌局
		/// </summary>
		public async Task NextTurnAsync()
		{
			// 每局开始的初始化
			deck.Shuffle();
			dealer.Init();
			players.ForEach(player => player.Init());

			if (NewTurnStart != null)
				NewTurnStart.Invoke(this, null);

			// 玩家下注
			foreach (var player in players)
			{
				int bet = await interaction.Bet(player.Id);
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

			// 若庄家明T(10,J,Q或K)暗A，则亮牌
			if (dealer_SeenCard.Rank >= Rank.Ten && dealer_BlindCard.Rank == Rank.Ace)
				dealer_BlindCard.Seen_Blind = true;
			// 若庄家明A
			else if (dealer_SeenCard.Rank == Rank.Ace)
			{
				// 询问玩家是否买保险金
				foreach (var player in players)
				{
					if (await interaction.WantInsurance(player.Id))
						player.BuyInsurance();
				}
				//若此时庄家暗T，构成BlackJack，亮牌
				if (dealer_BlindCard.Rank >= Rank.Ten)
					dealer_BlindCard.Seen_Blind = true;
			}

			if (!dealer.HasBlackJack)
			{
				foreach (var player in players)
					// 玩家BlackJack，庄家不是
					if (player.HasBlackJack)
					{
						if (GamerBlackJack != null)
							GamerBlackJack.Invoke(this, player);
						PlayerWin(player, 1.5f); // 玩家以1.5倍率赢
					}
			}
			else
			{
				if (GamerBlackJack != null)
					GamerBlackJack.Invoke(this, dealer);
				foreach (var player in players)
					if (player.HasBlackJack) // 都有BlackJack
					{
						if (GamerBlackJack != null)
							GamerBlackJack.Invoke(this, player);
						Draw(player); // 平手
					}
					else // 庄家有BlackJack，玩家没有
					{
						if (player.Insurance == null) // 没买保险
							DealerWin(player);
						else // 买了保险
							Draw(player); // 按平局处理
					}
			}

			// 询问剩余玩家是否double
			foreach (var player in players.Where(player => player.Active))
			{
				if (await interaction.WantToDouble(player.Id))
				{
					player.Double();
					player.AchieveCard(deck.DrawACard());
					if (player.SumPoint > 21)
					{
						if (GamerBoom != null)
							GamerBoom.Invoke(this, player);
						DealerWin(player);
					}
				}
			}

			// 每轮的选择
			foreach (var player in players.Where(player => player.Active))
			{
				do
				{
					// 拿牌
					if (await interaction.WantToHitMe(player.Id))
					{
						player.AchieveCard(deck.DrawACard());
                        //如果玩家五小龙，则该玩家直接赢
                        if(player.HandCards.Count == 5 && player.SumPoint <= 21)
                        {
                            if (GamerFiveDragon != null)
                                GamerFiveDragon.Invoke(this,player);
                            PlayerWin(player);
                        }

						// 玩家爆了，则庄家直接赢
						if (player.SumPoint > 21)
						{
							if (GamerBoom != null)
								GamerBoom.Invoke(this, player);
							DealerWin(player);
						}
					}
					// 停牌
					else
						player.Active = false;
				} while (player.Active);
			}

			// 庄家亮暗牌
			// 若还有玩家未结算，则庄家持续拿牌直到点数不小于17
			if (!dealer_BlindCard.Seen_Blind)
				dealer_BlindCard.Seen_Blind = true;
			if (players.Where(player => !player.Finish).Count() == 0)
			{
				if (TurnFinish != null)
					TurnFinish.Invoke(this, null);
				return;
			}
			while (dealer.SumPoint < 17 && dealer.HandCards.Count < 5)    //最多只能摸五张牌
				dealer.AchieveCard(deck.DrawACard());

			// 结算
			// 若庄家爆了
			if (dealer.SumPoint > 21)
			{
				if (GamerBoom != null)
					GamerBoom(this, dealer);
				foreach (var player in players.Where(player => !player.Finish))
					PlayerWin(player); // 玩家赢
			}
			else // 否则比大小
			{
				foreach (var player in players.Where(player => !player.Finish))
				{
					// 优先点数大的赢
					if (player.SumPoint > dealer.SumPoint)
						PlayerWin(player);
					else if (player.SumPoint < dealer.SumPoint)
						DealerWin(player);
					// 相同则牌少的赢
					else if (player.HandCards.Count > dealer.HandCards.Count)
						DealerWin(player);
					else if (player.HandCards.Count < dealer.HandCards.Count)
						PlayerWin(player);
					// 平局
					else
						Draw(player);
				}
			}
			if (TurnFinish != null)
				TurnFinish.Invoke(this, null);
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

			if (Finish != null)
				Finish.Invoke(this, player, false);
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

			if (Finish != null)
				Finish.Invoke(this, player, true);
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

			if (Finish != null)
				Finish.Invoke(this, player, null);
		}
	}
}
