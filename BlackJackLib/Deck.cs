using System;
using System.Collections.Generic;
using System.Text;

namespace BlackJackLib
{
	/// <summary>
	/// 表示一叠牌（全52张）的类
	/// </summary>
    public class Deck
    {
        private List<Card> cards = new List<Card>();  

		/// <summary>
		/// 牌库的剩余数量
		/// </summary>
        public int RemainCount
        {
            get
            {
                return cards.Count;
            }
        }

		/// <summary>
		/// 洗牌
		/// </summary>
		public void Shuffle()
        {
            cards.Clear();
            bool[,] ready_card = new bool[4,14];
            for(int i = 0; i < 4; i++)
            {
                for(int j = 0; j < 14; j++)
                {
                    ready_card[i, j]=false;
                }
            } 
            Random rd = new Random();   //随机发牌
            int count = 0;    
            Suit fresh_suit = 0;
            Rank fresh_rank = 0;
            while (count < 52)
            {
                fresh_suit = (Suit) rd.Next(4);
                fresh_rank = (Rank) (rd.Next(13) + 1);
                if (ready_card[(int)fresh_suit, (int)fresh_rank] == false)  //没有发过
                {
                    cards.Add(new Card(fresh_suit, fresh_rank));
                    ready_card[(int)fresh_suit, (int)fresh_rank] = true;
                    count++;
                }
            }
            
        }

		/// <summary>
		/// 取出一张牌，若牌库已空，则返回null
		/// </summary>
		/// <param name="seen_blind"></param>
		/// <returns></returns>
		public Card DrawACard(bool seen_blind = true)
		{
			if (cards.Count == 0) return null;
			Card ret = cards[0];
			ret.Seen_Blind = seen_blind;
			cards.RemoveAt(0);
			return ret;
		}

		public Deck()
		{
			Shuffle();
		}
    }
}
