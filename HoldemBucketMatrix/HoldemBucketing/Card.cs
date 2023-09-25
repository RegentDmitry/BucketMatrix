using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldemBucketing
{
    public enum CardValue
    {
        A = 14,
        K = 13,
        Q = 12,
        J = 11,
        T = 10,
        _9 = 9,
        _8 = 8,
        _7 = 7,
        _6 = 6,
        _5 = 5,
        _4 = 4,
        _3 = 3,
        _2 = 2,
        UNKNOWN = 0
    }

    public enum CardSuit
    {
        C = 0, D = 1, H = 2, S = 3
    }

    public enum CardSuitGeneric
    {
        X = 0, Y = 1, W = 2, Z = 3
    }

    public class Card : IComparable
    {
        public CardValue Value { get; set; }
        public CardSuit Suit { get; set; }

        public int GetValueBinaryCode()
        {
            switch (Value)
            {
                case CardValue._2: return 0b01000000000000;
                case CardValue._3: return 0b00100000000000;
                case CardValue._4: return 0b00010000000000;
                case CardValue._5: return 0b00001000000000;
                case CardValue._6: return 0b00000100000000;
                case CardValue._7: return 0b00000010000000;
                case CardValue._8: return 0b00000001000000;
                case CardValue._9: return 0b00000000100000;
                case CardValue.T: return 0b00000000010000;
                case CardValue.J: return 0b00000000001000;
                case CardValue.Q: return 0b00000000000100;
                case CardValue.K: return 0b00000000000010;
                case CardValue.A: return 0b00000000000001;
                default: return 0;
            }
        }

        public override string ToString()
        {
            return Value.ToString().Replace("_", "") + Suit.ToString().ToLower();
        }

        public int CompareTo(object obj)
        {
            var c = obj as Card;

            if (c.Value > this.Value)
                return 1;

            if (c.Value < this.Value)
                return -1;

            if (c.Suit > this.Suit)
                return -1;

            if (c.Suit < this.Suit)
                return 1;

            return 0;
        }

        public override bool Equals(object obj)
        {
            return CompareTo(obj) == 0;
        }

        public static List<Card> ParseCards(string text)
        {
            var l = new List<Card>();
            var t = text.Replace(" ", "");

            while (!string.IsNullOrEmpty(t))
            {
                var c = t.Substring(0, 1).ToUpper();
                var s = t.Substring(1, 1).ToLower();
                var newcard = new Card();

                switch (c)
                {
                    case "A": newcard.Value = CardValue.A; break;
                    case "K": newcard.Value = CardValue.K; break;
                    case "Q": newcard.Value = CardValue.Q; break;
                    case "J": newcard.Value = CardValue.J; break;
                    case "T": newcard.Value = CardValue.T; break;
                    case "9": newcard.Value = CardValue._9; break;
                    case "8": newcard.Value = CardValue._8; break;
                    case "7": newcard.Value = CardValue._7; break;
                    case "6": newcard.Value = CardValue._6; break;
                    case "5": newcard.Value = CardValue._5; break;
                    case "4": newcard.Value = CardValue._4; break;
                    case "3": newcard.Value = CardValue._3; break;
                    case "2": newcard.Value = CardValue._2; break;
                    default: break;
                }

                switch (s)
                {
                    case "h": newcard.Suit = CardSuit.H; break;
                    case "d": newcard.Suit = CardSuit.D; break;
                    case "s": newcard.Suit = CardSuit.S; break;
                    case "c": newcard.Suit = CardSuit.C; break;
                    default: break;
                }

                l.Add(newcard);

                t = t.Substring(2);
            }

            return l;
        }
    }
}
