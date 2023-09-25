using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldemBucketing
{
    public static class Utils
    {
        public static List<string> Cards = new List<string>() { "A", "K", "Q", "J", "T", "9", "8", "7", "6", "5", "4", "3", "2" };
        public static List<string> Suits = new List<string>() { "c", "d", "h", "s" };
        public static List<string> UnifiedSymbols = new List<string>() { "x", "y", "z", "w" };
        public static List<Card> Deck = DeckGenerate();

        public static List<Range> GetRangesFromCsv(string filename)
        {
            if (!File.Exists(filename))
                return null;

            var ranges = new Dictionary<string, Range>();
            var actions = new Dictionary<string, int>();

            foreach (var line in File.ReadAllLines(filename))
            {
                var tokens = line.Split(new char[] { ',' }).ToList();
                if (line.StartsWith("Hand"))
                {
                    foreach(var t in tokens.Where(t => t != "Hand" && !t.EndsWith("EV")))
                    {
                        actions.Add(t, tokens.IndexOf(t));
                        ranges.Add(t, new Range() { Name = t });
                    }
                }
                else
                {
                    var hand = tokens[0];
                    foreach (var a in actions)
                    {
                        var r = ranges[a.Key];
                        r.Add(new RangeItem()
                        {
                            HandString = hand,
                            Hand = Card.ParseCards(hand),
                            Weight = double.Parse(tokens[a.Value])
                        });
                    }
                }
            }

            return ranges.Select(t => t.Value).ToList();
        }

        private static List<Card> DeckGenerate()
        {
            var list = new List<Card>();

            foreach (CardValue val in Enum.GetValues(typeof(CardValue)))
            {
                if (val == CardValue.UNKNOWN)
                    continue;
                foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
                    list.Add(new Card() { Value = val, Suit = suit });
            }

            return list;
        }

        public static (ulong code_major, uint code_minor, ulong mask_major, uint mask_minor) GetCodeAndMask(List<CustomBoolean> Values)
        {
            ulong major = 0, mask_major = 0;
            uint minor = 0, mask_minor = 0;

            ulong l_one = 1;
            for (int i = 0; i < 63; i++)
            {
                var temp = l_one << i;
                if (Values[i] != CustomBoolean.Ignore)
                    mask_major |= temp;

                if (Values[i] != CustomBoolean.True)
                    continue;

                major |= temp;
            }

            uint i_one = 1;
            for (int i = 0; i < 20; i++)
            {
                var temp = i_one << i;
                if (Values[i + 63] != CustomBoolean.Ignore)
                    mask_minor |= temp;

                if (Values[i + 63] != CustomBoolean.True)
                    continue;

                minor |= temp;
            }

            return (major, minor, mask_major, mask_minor);
        }

    }
}
