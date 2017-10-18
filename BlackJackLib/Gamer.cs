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

        public int Balance
        {
            get
            {
                return balance;
            }
            set
            {
                balance = value;
            }
        }

		/// <summary>
		/// 当前手牌的总点数
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
		/// 玩家获得一张手牌
		/// </summary>
		/// <param name="card">玩家要获得的牌</param>
		public void AchieveCard(Card card)   
        {
            handCards.Add(card);
        }

        public Gamer(int balance)
        {
            Balance = balance;
        }
    }
}
