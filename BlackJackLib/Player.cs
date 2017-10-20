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
		private int id;

        private int stake;		// 当局游戏赌金
        private int? insurance; // 当局保险金，若未买保险，则为null
		private bool active;	// 是否能继续游戏
		private bool finish;	// 是否已经结算

		public int Id
		{
			get
			{
				return id;
			}
		}

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

		/// <summary>
		/// 是否能继续操作
		/// </summary>
		public bool Active
		{
			get
			{
				return active;
			}
			set
			{
				active = value;
			}
		}

		/// <summary>
		/// 是否已经结算
		/// </summary>
		public bool Finish
		{
			get
			{
				return finish;
			}
			set
			{
				if ((finish = value) == true)
					Active = false;
			}
		}

		/// <summary>
		/// 购买保险金
		/// </summary>
		public void BuyInsurance()
		{
			Insurance = Stake / 2;
			Balance -= Stake / 2;
		}

		/// <summary>
		/// 玩家选择双倍，赌金翻倍
		/// </summary>
		public void Double()
		{
			Balance -= Stake;
			Stake *= 2;
			Active = false;
		}

		public override void Init()
		{
			base.Init();
			Stake = 0;
			Insurance = null;
			Active = true;
			Finish = false;
		}

		public Player(int id, int balance) : base(balance)
		{
			this.id = id;
		}
	}
}
