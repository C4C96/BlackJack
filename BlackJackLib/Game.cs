using System;
using System.Collections.Generic;
using System.Text;

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
			dealer = new Dealer(DEFAULT_BALANCE);
			players = new List<Player>();
			for (int i = 0; i < playerNumber; i++)
				players.Add(new Player(DEFAULT_BALANCE));
			deck = new Deck();
		}

		public void Start()
		{
			
		}
    }
}
