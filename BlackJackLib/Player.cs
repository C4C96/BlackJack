using System;
using System.Collections.Generic;
using System.Text;

namespace BlackJackLib
{
	/// <summary>
	/// 表示闲家的类
	/// </summary>
	public class Player : Gamer
	{
        private int stake;		//当局游戏赌金
        private int? insurance;	//当局保险金，若未买保险，则为null

		/// <summary>
		/// 赌金
		/// </summary>
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

		/// <summary>
		/// 保险金,若未买保险，则为null
		/// </summary>
		public int? Insurance
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

		public Player(int balance) : base(balance)
		{ }
	}
}
