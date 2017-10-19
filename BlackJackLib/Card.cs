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

		public override string ToString()
		{
			if (!Seen_Blind)
				return "**";

			string ret = "";
			switch (Suit)
			{
				case Suit.Club:
					ret = "♣";
					break;
				case Suit.Diamond:
					ret = "♦";
					break;
				case Suit.Heart:
					ret = "♥";
					break;
				case Suit.Spade:
					ret = "♠";
					break;
			}
			switch (Rank)
			{
				case Rank.Ace:
					ret += "A";
					break;
				case Rank.Jack:
					ret += "J";
					break;
				case Rank.Queen:
					ret += "Q";
					break;
				case Rank.King:
					ret += "K";
					break;
				default:
					ret += (int)Rank;
					break;
			}
			return ret;
		}
	}
}
