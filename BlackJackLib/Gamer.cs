using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BlackJackLib
{
	/// <summary>
	/// 表示游戏参与者的类
	/// </summary>
    public abstract class Gamer
    {
        protected int balance;	// 钱包余额
        protected List<Card> handCards = new List<Card>();  //玩家的手牌

		public string name = "Gamer";

        public int Balance
        {
            get
            {
                return balance;
            }
            set
            {
                balance = value;
				Console.WriteLine($"{name}还剩金额：{Balance}");
            }
        }

		/// <summary>
		/// 当前手牌的总点数，A按最大而不爆计算
		/// </summary>
		public int SumPoint
		{
			get
			{
				// 将Ace视作11的情况下的和
				int sum = handCards.Select((card) => card.Rank == Rank.Ace ? 11 
													: (int)card.Rank > 10 ? 10 
													: (int)card.Rank)
									.Sum();
				// Ace的数量
				int aceCount = handCards.Where((card) => card.Rank == Rank.Ace)
										.Count();
				// 若爆掉，则逐渐将Ace由11替换成1
				while (sum > 21 && aceCount > 0)
				{
					sum -= 10;
					aceCount--;
				}
				return sum;
			}
		}

		/// <summary>
		/// 获得一张手牌
		/// </summary>
		/// <param name="card">玩家要获得的牌</param>
		public void AchieveCard(Card card)   
        {
            handCards.Add(card);
			
			Console.WriteLine($"{name}获得了{card}");
			Console.WriteLine($"{name}当前手牌为{HandCardsToString()}");
        }

		public List<Card> HandCards
		{
			get
			{
				return handCards;
			}
		}

		/// <summary>
		/// 是否拥有黑杰克
		/// </summary>
		public bool HasBlackJack
		{
			get
			{
				return handCards.Exists(card => card.Rank == Rank.Ace)
					&& handCards.Exists(card => card.Rank >= Rank.Ten);
			}
		}

		/// <summary>
		/// 新的一局游戏开始时的初始化，初始牌与部分变量，但不改变余额
		/// </summary>
		public virtual void Init()
		{
			handCards.Clear();
		}

        public Gamer(int balance)
        {
            Balance = balance;
			Init();
        }

		public string HandCardsToString()
		{
			StringBuilder sb = new StringBuilder("{");
			handCards.ForEach(card => sb.Append(card).Append(" "));
			sb.Append("}");
			return sb.ToString();
		}
    }
}
