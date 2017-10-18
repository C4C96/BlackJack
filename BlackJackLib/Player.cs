using System;
using System.Collections.Generic;
using System.Text;

namespace BlackJackLib
{
	public class Player : Gamer
	{
        private int stake; //当局游戏赌金
        private int insurance; //当局保险金

        public int Stake
        {
            get
            {
                return stake;
            }
            set
            {
                stake = value;
            }
        }

        public int Insurance
        {
            get
            {
                return insurance;
            }
            set
            {
                insurance = value;
            }
        }
	}
}
