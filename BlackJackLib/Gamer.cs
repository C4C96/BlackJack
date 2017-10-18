using System;
using System.Collections.Generic;
using System.Text;

namespace BlackJackLib
{
    abstract class Gamer
    {
        private int balance;  //钱包余额
        private bool is_dead;  //false为没有爆牌，true为已经爆牌
        private List<Card> handcards = new List<Card>();  //玩家的手牌

        public int Balance
        {
            get
            {
                return balance;
            }
            private set
            {
                balance = value;
            }
        }

        public bool Is_Dead
        {
            get
            {
                return is_dead;
            }
            private set
            {
                is_dead = value;
            }
        }

        public void DrawCard(Card card)   //玩家获得一张手牌
        {
            handcards.Add(card);
        }
        public Gamer(int balance, bool is_dead = false)
        {
            Balance = balance;
            Is_Dead = is_dead;
        }
    }
}
