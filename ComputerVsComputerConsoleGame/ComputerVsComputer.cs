using System.Collections.Generic;
using Microsoft.FSharp.Core;
using System;

namespace ConsoleGame
{
    class Player
    {
        public string Name { get; set;}
        public List<Cards.Card> Cards { get; set; }
        public int Score { get; set; }

        public Player(string name)
        {
            this.Name = name;
            Cards = new List<Cards.Card>();
        }
    }

    class ComputerVsComputer
    {
        static string ToString(Cards.Rank rank)
        {
            switch (rank.Tag)
            {
                case Cards.Rank.Tags.Ace:   return "A";
                case Cards.Rank.Tags.Two:   return "2";
                case Cards.Rank.Tags.Three: return "3";
                case Cards.Rank.Tags.Four:  return "4";
                case Cards.Rank.Tags.Five:  return "5";
                case Cards.Rank.Tags.Six:   return "6";
                case Cards.Rank.Tags.Seven: return "7";
                case Cards.Rank.Tags.Eight: return "8";
                case Cards.Rank.Tags.Nine:  return "9";
                case Cards.Rank.Tags.Ten:   return "10";
                case Cards.Rank.Tags.Jack:  return "J";
                case Cards.Rank.Tags.Queen: return "Q";
                case Cards.Rank.Tags.King:  return "K"; 
                default:                    return "?";
            }
        }

        static string ToString(Cards.Suit suit)
        {
            switch (suit.Tag)
            {
                case Cards.Suit.Tags.Clubs:     return "C";
                case Cards.Suit.Tags.Diamonds:  return "D";
                case Cards.Suit.Tags.Hearts:    return "H";
                case Cards.Suit.Tags.Spades:    return "S";
                default: return "?";
            }
        }

        static string ToString(Cards.Card card)
        {
            return ToString(card.rank) + ToString(card.suit);
        }

        static Stack<Cards.Card> RemainingDeck = new Stack<Cards.Card>();
        static Stack<Cards.Card> Discards = new Stack<Cards.Card>();

        static void Main(string[] args)
        {
            var player1 = new Player("player 1");
            var player2 = new Player("player 2");

            while (true)
            {
                player1.Cards.Clear();
                player2.Cards.Clear();
                RemainingDeck.Clear();

                var deck = Cards.Shuffle(Cards.FullDeck);

                foreach (var card in deck)
                    RemainingDeck.Push(card);

                for (int i = 0; i < 10; i++)
                {
                    player1.Cards.Add(RemainingDeck.Pop());
                    player2.Cards.Add(RemainingDeck.Pop());
                }

                Discards.Push(RemainingDeck.Pop());

                while (true)
                {
                    if (Turn(player1, player2))
                        break;
                    if (Turn(player2, player1))
                        break;
                }

                Console.WriteLine("Player 1 score: {0}, Player 2 score: {1}", player1.Score, player2.Score);

                if (player1.Score > 100)
                {
                    Console.WriteLine("Player 1 wins {0} to {1}", player1.Score, player2.Score);
                    Console.ReadKey();
                    break;
                }

                if (player2.Score > 100)
                {
                    Console.WriteLine("Player 2 wins {0} to {1}", player2.Score, player1.Score);
                    Console.ReadKey();
                    break;
                }
            }
        }

        static bool Turn(Player player, Player opponent)
        {
            Console.WriteLine(player.Name);

            player.Cards.Sort();
            foreach (var card in player.Cards)
                Console.Write("{0} ", ToString(card));
            Console.WriteLine();

            Console.Write("Thinking ... ");
            var pickupDiscard = ComputerPlayer.ComputerPickupDiscard(player.Cards, Discards.Peek(), RemainingDeck);
            Console.WriteLine();      

            if (pickupDiscard)
            {
                var card = Discards.Pop();
                player.Cards.Add(card);
                Console.WriteLine("picks up {0} from Discard pile", ToString(card));
            }
            else
            {
                var card = RemainingDeck.Pop();
                player.Cards.Add(card);
                Console.WriteLine("picks up {0} from Remaining Deck", ToString(card));
            }

            var move = ComputerPlayer.ComputerMove(player.Cards);

            var discard = move.Item2;
            if (FSharpOption<Cards.Card>.get_IsSome(discard))
            {
                Discards.Push(discard.Value);
                player.Cards.Remove(discard.Value);
                Console.WriteLine("Discard {0}", ToString(discard.Value));
            }
            else
                Console.WriteLine("Discard nothing");

            Console.WriteLine("deadwood score = {0}", GinRummy.Deadwood(player.Cards));

            var action = move.Item1;
            if (action.IsGin)
                Console.WriteLine("Gin!");
            else if (action.IsKnock)
                Console.WriteLine("Knock!");

            if (!action.IsContinue)
            {
                Console.WriteLine(player.Name + " goes out first ...");
                var score = GinRummy.Score(player.Cards, opponent.Cards);
                if (score > 0)
                    player.Score += score;
                else
                    opponent.Score -= score;
            }

            return !action.IsContinue;

        }
    }
}