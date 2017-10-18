using System;
using System.Collections.Generic;
using System.Text;

namespace BlackJackLib
{
    public class Card
    {
		private Suit suit;
		private Rank rank;
		private bool seen_blind;

		public Suit Suit
		{
			get
			{
				return suit;
			}
			private set
			{
				suit = value;
			}
		}

		public Rank Rank
		{
			get
			{
				return rank;
			}
			private set
			{
				rank = value;
			}
		}

		/// <summary>
		/// true表示明牌，false表示暗牌
		/// </summary>
		public bool Seen_Blind
		{
			get
			{
				return seen_blind;
			}
			set
			{
				seen_blind = value;
			}
		}

		public Card(Suit suit, Rank rank, bool seen_blind = true)
		{
			Suit = suit;
			Rank = rank;
			Seen_Blind = seen_blind;
		}
    }
}
