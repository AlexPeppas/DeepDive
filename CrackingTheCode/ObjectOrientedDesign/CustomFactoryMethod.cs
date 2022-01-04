using System;
using System.Collections.Generic;
using System.Text;

namespace DeepDiveTechnicals.ObjectOrientedDesign
{
    public enum GameType 
    {
        Poker = 0,
        BlackJack = 1
    };

 /*   public class CustomFactoryMethod
    {
        public static CustomFactoryMethod getCardGame(GameType type)
        {
            if (type == GameType.Poker)
                return new PokerGame();
            else if (type == GameType.BlackJack)
                return new BlackJackGame();
        }
    }*/
}
