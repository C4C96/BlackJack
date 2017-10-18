using System;
using System.Collections.Generic;
using System.Text;

namespace BlackJackLib
{
    public class Deck
    {
        private int remains; //牌库中剩余几张牌
        private List<Card> cards = new List<Card>();  
        public int Remains
        {
            get
            {
                return remains;
            }
            private set
            {
                remains = Remains;
            }
        }

        public void shuffle()  //洗牌+初始化牌库
        {
            cards.Clear();
            bool[,] ready_card = new bool[4,13];
            for(int i=0;i<4;i++)
            {
                for(int j=0;j<13;j++)
                {
                    ready_card[i][j]=false;
                }
            } 
            Random rd = new Random();   //随机发牌
            int count=0;    
            Suit fresh_suit=0;
            Rank fresh_rank=0;
            while(count<52)
            {
                fresh_suit=rd.Next(4);
                fresh_rank=rd.Next(13);
                if(ready_card[fresh_suit][fresh_rank]==false)  //没有发过
                {
                    cards.Add(new Card(fresh_suit,fresh_rank));
                    ready_card[fresh_suit][fresh_rank]=true;
                    count++;
                }
            }
            
        }

        public Card draw()
        {
            return cards[0];
            cards.Remove[0];
        }
    }
}
