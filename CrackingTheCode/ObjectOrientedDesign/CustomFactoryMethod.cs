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

    /*public class CustomFactoryMethod
    {
        public static T getCardGame<T>(GameType type) where T:class
        {
            if (type == GameType.Poker)
                return new PokerGame();
            else if (type == GameType.BlackJack)
                return new BlackJackGame();
        }
    }*/

    public class PokerGame
    {

    }

    public class BlackJackGame
    {
    }

}
