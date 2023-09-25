using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Combinatorics.Collections;
using Combinatorics;

namespace HoldemBucketing
{   
    public static class Categories
    {
        public static List<Card> Deck { get; set; } = DeckGenerate();

        private static List<CardValue> _cardValueList = new List<CardValue>() { CardValue.A, CardValue.K, CardValue.Q, CardValue.J, CardValue.T, CardValue._9,
                                                                     CardValue._8, CardValue._7, CardValue._6, CardValue._5, CardValue._4, CardValue._3, CardValue._2}; 

        public static bool Flush(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var boardSuits = board.GroupBy(t => t.Suit).Where(t => t.Count() >= 3).Select(t => t.Key).ToList();
            var holeSuits = holeCards.GroupBy(t => t.Suit).Where(t => t.Count() >= 2).Select(t => t.Key).ToList();
            return boardSuits.Any(t => holeSuits.Contains(t));
        }

        public static bool NutFlush2(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var boardSuits = board.GroupBy(t => t.Suit).Where(t => t.Count() >= 3).Select(t => t.Key).ToList();
            var holeSuits = holeCards.GroupBy(t => t.Suit).Where(t => t.Count() >= 2).Select(t => t.Key).ToList();
            var hasFlush = boardSuits.Any(t => holeSuits.Contains(t));
            if (!hasFlush)
                return false;

            var flushSuit = boardSuits.Where(t => holeSuits.Contains(t)).FirstOrDefault();
            var suitList = board.Where(t => t.Suit == flushSuit).Select(t => (int)t.Value).ToList();
            var firstCardValue = CardValue._2;
            var secondCardValue = CardValue._2;
            var firstFound = false;
            for (int i = 14; i > 1; i--)
            {
                if (!suitList.Contains(i))
                {
                    if (firstFound)
                    {
                        secondCardValue = (CardValue)i;
                        break;
                    }
                    else
                    {
                        firstCardValue = (CardValue)i;
                        firstFound = true;
                    }
                }
            }

            return holeCards.Count(t => t.Suit == flushSuit && t.Value == secondCardValue) == 1 &&
                   holeCards.Count(t => t.Suit == flushSuit && t.Value == firstCardValue) != 1;
        }

        public static bool NutFlush(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var boardSuits = board.GroupBy(t => t.Suit).Where(t => t.Count() >= 3).Select(t => t.Key).ToList();
            var holeSuits = holeCards.GroupBy(t => t.Suit).Where(t => t.Count() >= 2).Select(t => t.Key).ToList();
            var hasFlush = boardSuits.Any(t => holeSuits.Contains(t));
            if (!hasFlush)
                return false;

            var flushSuit = boardSuits.Where(t => holeSuits.Contains(t)).FirstOrDefault();
            var suitList = board.Where(t => t.Suit == flushSuit).Select(t => (int)t.Value).ToList();
            var maxCardValue = CardValue._2;
            for (int i = 14; i > 1; i--)
            {
                if (!suitList.Contains(i))
                {
                    maxCardValue = (CardValue)i;
                    break;
                }
            }

            return holeCards.Count(t => t.Suit == flushSuit && t.Value == maxCardValue) == 1;
        }

        public static bool NotNutFlush(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var boardSuits = board.GroupBy(t => t.Suit).Where(t => t.Count() >= 3).Select(t => t.Key).ToList();
            var holeSuits = holeCards.GroupBy(t => t.Suit).Where(t => t.Count() >= 2).Select(t => t.Key).ToList();
            var hasFlush = boardSuits.Any(t => holeSuits.Contains(t));
            if (!hasFlush)
                return false;

            var flushSuit = boardSuits.Where(t => holeSuits.Contains(t)).FirstOrDefault();
            var suitList = board.Where(t => t.Suit == flushSuit).Select(t => (int)t.Value).ToList();
            var firstCardValue = CardValue._2;
            var secondCardValue = CardValue._2;
            var firstFound = false;
            for (int i = 14; i > 1; i--)
            {
                if (!suitList.Contains(i))
                {
                    if (firstFound)
                    {
                        secondCardValue = (CardValue)i;
                        break;
                    }
                    else
                    {
                        firstCardValue = (CardValue)i;
                        firstFound = true;
                    }
                }
            }

            return !holeCards.Any(t => t.Suit == flushSuit && t.Value == firstCardValue) &&
                   !holeCards.Any(t => t.Suit == flushSuit && t.Value == secondCardValue);
        }

        private static int CountSD(IEnumerable<Card> board)
        {
            var boardVals = board.Select(t => (int)t.Value).ToList();
            if (boardVals.Contains(14))
                boardVals.Add(1);

            var sdCounter = 0;
            for (int i = 1; i <= 10; i++)
            {
                var counter = 0;
                for (int j = i; j < i + 5; j++)
                {
                    if (boardVals.Contains(j))
                        counter++;
                }
                if (counter >= 3)
                    return 0;

                if (counter == 2)
                    sdCounter++;
            }

            return sdCounter;
        }

        private static int MaxSD(IEnumerable<Card> board)
        {
            var boardVals = board.Select(t => (int)t.Value).ToList();
            if (boardVals.Contains(14))
                boardVals.Add(1);

            var maxSD = 0;
            for (int i = 1; i <= 10; i++)
            {
                var counter = 0;
                for (int j = i; j < i + 5; j++)
                {
                    if (boardVals.Contains(j))
                        counter++;
                }
                if (counter >= 3)
                    return 0;

                if (counter == 2)
                    maxSD = i + 4;
            }

            return maxSD;
        }

        public static int BDFD(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            if (board.Count() != 3)
                return 0;

            var holeSuits = holeCards.GroupBy(t => t.Suit).Where(t => t.Count() >= 2).Select(t => t.Key).ToList();
            if (holeSuits.Count() == 0)
                return 0;

            var boardSuits = board.GroupBy(t => t.Suit).Where(t => t.Count() == 1).Select(t => t.Key).ToList();
            if (boardSuits.Count() == 0)
                return 0;

            return holeSuits.Count(t => boardSuits.Contains(t));
        }

        public static bool BDFDN(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            if (board.Count() != 3)
                return false;

            var holeSuits = holeCards.GroupBy(t => t.Suit).Where(t => t.Count() >= 2).Select(t => new Card() { Suit = t.Key, Value = t.Max(tt => tt.Value) }).ToList();
            if (!holeSuits.Any())
                return false;

            var boardCandidates = board.GroupBy(t => t.Suit).Where(t => t.Count() == 1).Select(t => new Card() { Suit = t.Key, Value = t.Max(tt => tt.Value == CardValue.A) ? CardValue.K : CardValue.A }).ToList();
            if (!boardCandidates.Any())
                return false;

            return holeSuits.Any(hs => boardCandidates.Contains(hs));
        }

        public static bool FlushDraw(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var boardSuits = board.GroupBy(t => t.Suit).Where(t => t.Count() == 2).Select(t => t.Key).ToList();
            var holeSuits = holeCards.GroupBy(t => t.Suit).Where(t => t.Count() >= 2).Select(t => t.Key).ToList();
            return boardSuits.Any(t => holeSuits.Contains(t));
        }

        public static bool NutFlushDraw(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var boardSuits = board.GroupBy(t => t.Suit).Where(t => t.Count() == 2).Select(t => t.Key).ToList();
            var holeSuits = holeCards.GroupBy(t => t.Suit).Where(t => t.Count() >= 2).Select(t => t.Key).ToList();
            var fd = boardSuits.Any(t => holeSuits.Contains(t));
            if (!fd)
                return false;

            var flushSuit = boardSuits.Where(t => holeSuits.Contains(t)).FirstOrDefault();
            var suitList = board.Where(t => t.Suit == flushSuit).Select(t => (int)t.Value).ToList();
            var maxCardValue = CardValue._2;
            for (int i = 14; i > 1; i--)
            {
                if (!suitList.Contains(i))
                {
                    maxCardValue = (CardValue)i;
                    break;
                }
            }

            return holeCards.Count(t => t.Suit == flushSuit && t.Value == maxCardValue) == 1;
        }

        public static bool NutFlushDraw2(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var boardSuits = board.GroupBy(t => t.Suit).Where(t => t.Count() == 2).Select(t => t.Key).ToList();
            var holeSuits = holeCards.GroupBy(t => t.Suit).Where(t => t.Count() >= 2).Select(t => t.Key).ToList();
            var fd = boardSuits.Any(t => holeSuits.Contains(t));
            if (!fd)
                return false;

            var flushSuit = boardSuits.Where(t => holeSuits.Contains(t)).FirstOrDefault();
            var suitList = board.Where(t => t.Suit == flushSuit).Select(t => (int)t.Value).ToList();

            var firstCardValue = CardValue._2;
            var secondCardValue = CardValue._2;
            var firstFound = false;
            for (int i = 14; i > 1; i--)
            {
                if (!suitList.Contains(i))
                {
                    if (firstFound)
                    {
                        secondCardValue = (CardValue)i;
                        break;
                    }
                    else
                    {
                        firstCardValue = (CardValue)i;
                        firstFound = true;
                    }
                }
            }

            return holeCards.Count(t => t.Suit == flushSuit && t.Value == secondCardValue) == 1 &&
                   holeCards.Count(t => t.Suit == flushSuit && t.Value == firstCardValue) != 1;
        }

        public static bool NotNutFlushDraw(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var boardSuits = board.GroupBy(t => t.Suit).Where(t => t.Count() == 2).Select(t => t.Key).ToList();
            var holeSuits = holeCards.GroupBy(t => t.Suit).Where(t => t.Count() >= 2).Select(t => t.Key).ToList();
            var fd = boardSuits.Any(t => holeSuits.Contains(t));
            if (!fd)
                return false;

            var flushSuit = boardSuits.Where(t => holeSuits.Contains(t)).FirstOrDefault();
            var suitList = board.Where(t => t.Suit == flushSuit).Select(t => (int)t.Value).ToList();
            var maxCardValue = CardValue._2;
            var secondCardValue = CardValue._2;
            var firstFound = false;
            for (int i = 14; i > 1; i--)
            {
                if (!suitList.Contains(i))
                {
                    if (firstFound)
                    {
                        secondCardValue = (CardValue)i;
                        break;
                    }
                    else
                    {
                        maxCardValue = (CardValue)i;
                        firstFound = true;
                    }
                }
            }

            return !holeCards.Any(t => t.Suit == flushSuit && t.Value == maxCardValue) &&
                   !holeCards.Any(t => t.Suit == flushSuit && t.Value == secondCardValue);
        }

        public static bool TopSet(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            return TopSet(holeCards.Select(t => t.Value).ToList(), board.Select(t => t.Value).ToList());
        }

        public static bool TopSet(IEnumerable<CardValue> holeCards, IEnumerable<CardValue> board)
        {
            if (Paired(board))
                return false;
            var pairs = holeCards.GroupBy(t => t).Where(t => t.Count() >= 2).Select(t => t.Key).ToList();
            return pairs.Contains(board.Max());
        }

        public static bool BottomSet(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            return BottomSet(holeCards.Select(t => t.Value).ToList(), board.Select(t => t.Value).ToList());
        }

        public static bool BottomSet(IEnumerable<CardValue> holeCards, IEnumerable<CardValue> board)
        {
            if (Paired(board))
                return false;
            var pairs = holeCards.GroupBy(t => t).Where(t => t.Count() >= 2).Select(t => t.Key).ToList();
            return pairs.Contains(board.Min());
        }

        public static bool MiddleSet(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            return MiddleSet(holeCards.Select(t => t.Value).ToList(), board.Select(t => t.Value).ToList());
        }

        public static bool MiddleSet(IEnumerable<CardValue> holeCards, IEnumerable<CardValue> board)
        {
            if (Paired(board))
                return false;
            var pairs = holeCards.GroupBy(t => t).Where(t => t.Count() >= 2).Select(t => t.Key).ToList();
            return board.Any(t => pairs.Contains(t) && t != board.Min() && t != board.Max());
        }

        public static int Sets(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            return Sets(holeCards.Select(t => t.Value), board.Select(t => t.Value));
        }

        public static int Sets(IEnumerable<CardValue> holeCards, IEnumerable<CardValue> board)
        {
            if (Paired(board))
                return 0;

            var hashSet = new HashSet<CardValue>();
            var pairs = new HashSet<CardValue>();
            foreach (var h in holeCards)
            {
                if (!hashSet.Contains(h))
                {
                    hashSet.Add(h);
                    continue;
                }
                else
                {
                    pairs.Add(h);
                }
            }

            var sets = 0;
            foreach (var b in board)
            {
                if (pairs.Contains(b))
                    sets++;
            }

            //var pairs = holeCards.GroupBy(t => t).Where(t => t.Count() >= 2).Select(t => t.Key).ToList();
            //return board.Where(t => pairs.Contains(t)).Count();
            return sets;
        }


        public static bool Trips(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            if (FullHouse(holeCards, board))
                return false;
            var pairs = board.GroupBy(t => t.Value).Where(t => t.Count() == 2).Select(t => t.Key).ToList();
            return holeCards.Where(t => pairs.Contains(t.Value)).Count() == 1;
        }

        public static bool FourOfKind(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var boardtripple = board.GroupBy(t => t.Value).Where(t => t.Count() == 3).Select(t => t.Key).ToList();
            if (holeCards.Any(t => boardtripple.Contains(t.Value)))
                return true;

            var boardPairs = board.GroupBy(t => t.Value).Where(t => t.Count() == 2).Select(t => t.Key).ToList();
            var holePairs = holeCards.GroupBy(t => t.Value).Where(t => t.Count() == 2).Select(t => t.Key).ToList();

            if (holePairs.Any(t => boardPairs.Contains(t)))
                return true;

            return false;
        }

        private static bool Paired(IEnumerable<CardValue> board)
        {
            var brd = board.ToList();
            var count = board.Count();
            for (var i = 0; i < count; i++)
                for (var j = i + 1; j < count; j++)
                    if (brd[i] == brd[j])
                        return true;
            return false;
        }

        private static Tuple<CardValue, CardValue> FullHousePair(IEnumerable<CardValue> holeCards, IEnumerable<CardValue> board)
        {
            var c3 = CardValue.UNKNOWN;
            var c2 = CardValue.UNKNOWN;

            var m2 = new Combinations<CardValue>(holeCards.ToList(), 2, GenerateOption.WithoutRepetition);
            var m3 = new Combinations<CardValue>(board.ToList(), 3, GenerateOption.WithoutRepetition);

            foreach (var m2item in m2)
                foreach (var m3item in m3)
                {
                    var temp = new List<CardValue>();
                    temp.AddRange(m2item);
                    temp.AddRange(m3item);

                    var dic = new Dictionary<CardValue, int>();
                    foreach (var t in temp)
                    {
                        if (dic.ContainsKey(t))
                            dic[t]++;
                        else
                            dic[t] = 1;
                    }
                    if (dic.Count != 2)
                        continue;

                    var first = dic.First();
                    var last = dic.Last();

                    if (!(first.Value == 3 && last.Value == 2 ||
                        first.Value == 2 && last.Value == 3))
                        continue;

                    var cc3 = first.Value == 3 ? first.Key : last.Key;
                    var cc2 = first.Value == 2 ? first.Key : last.Key;

                    if (cc3 > c3)
                    {
                        c3 = cc3;
                        c2 = cc2;
                    }

                    if (cc3 == c3 && cc2 > c2)
                        c2 = cc2;
                }
            
            if (c3 == CardValue.UNKNOWN)
                return null;

            return new Tuple<CardValue, CardValue>(c3, c2);
        }

        public static bool FullHouse(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            return FullHouse(holeCards.Select(t => t.Value).ToList(), board.Select(t => t.Value).ToList());
        }

        public static bool FullHouse(IEnumerable<CardValue> holeCards, IEnumerable<CardValue> board)
        {
            var isPaired = Paired(board);
            if (isPaired)
            {
                var fullhousePair = FullHousePair(holeCards, board);
                if (fullhousePair != null)
                    return true;
            }
            return false;
        }

        public static bool FullHouseN(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var list = holeCards.ToList();
            return FullHouseN(holeCards.Select(t => t.Value).ToList(), board.Select(t => t.Value).ToList());
        }

        public static bool FullHouseN(IEnumerable<CardValue> holeCards, IEnumerable<CardValue> board)
        {
            var isPaired = Paired(board);
            if (isPaired)
            {
                var fullhousePair = FullHousePair(holeCards, board);
                var topCard = board.Max();
                if (fullhousePair != null)
                    return fullhousePair.Item1 > fullhousePair.Item2 && fullhousePair.Item1 >= topCard;
            }
            return false;
        }

        public static bool FullHouseNN(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            return FullHouseNN(holeCards.Select(t => t.Value).ToList(), board.Select(t => t.Value).ToList());
        }

        public static bool FullHouseNN(IEnumerable<CardValue> holeCards, IEnumerable<CardValue> board)
        {
            var isPaired = Paired(board);
            if (isPaired)
            {
                var fullhousePair = FullHousePair(holeCards, board);
                var topCard = board.Max();
                if (fullhousePair != null)
                    return fullhousePair.Item1 < fullhousePair.Item2 || fullhousePair.Item1 < topCard;
            }
            return false;
        }

        private static int CompareListCardValues(List<CardValue> listA, List<CardValue> listB)
        {
            if (listA.Count > listB.Count)
                return 1;

            if (listA.Count < listB.Count)
                return -1;

            for (int i = 0; i < listA.Count; i++)
            {
                if (listA[i] > listB[i])
                    return 1;

                if (listA[i] < listB[i])
                    return -1;
            }

            return 0;
        }

        private static List<CardValue> GetPairs(IEnumerable<CardValue> holeCards, IEnumerable<CardValue> board)
        {
            var dict = new Dictionary<CardValue, int>();
            foreach (var cv in holeCards)
            {
                if (dict.ContainsKey(cv))
                    dict[cv]++;
                else
                    dict[cv] = 1;
            }
            var clearValues = dict.Where(t => t.Value == 1).Select(t => t.Key).ToList();

            var dictBoard = new Dictionary<CardValue, int>();
            foreach (var c in board)
            {
                if (dictBoard.ContainsKey(c))
                    dictBoard[c]++;
                else
                    dictBoard[c] = 1;
            }
            var clearBoardValues = dictBoard.Where(t => t.Value == 1).Select(t => t.Key).ToList();

            var result = new List<CardValue>();
            foreach (var c in clearValues)
            {
                if (clearBoardValues.Contains(c))
                    result.Add(c);
            }
            return result;
        }

        public static bool Pair(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            return Pair(holeCards.Select(t => t.Value).ToList(), board.Select(t => t.Value).ToList());
        }

        public static bool PocketPair(List<Card> holeCards)
        {
            return holeCards[0].Value == holeCards[1].Value;
        }

        public static bool Pair(IEnumerable<CardValue> holeCards, IEnumerable<CardValue> board)
        {
            var p = GetPairs(holeCards, board);
            return p.Count == 1;
        }

        public static bool BottomPair(IEnumerable<CardValue> holeCards, IEnumerable<CardValue> board)
        {
            var p = GetPairs(holeCards, board);
            return p.Count == 1 && board.Min(t => t) == p[0];
        }

        public static bool BottomPair(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            return BottomPair(holeCards.Select(t => t.Value).ToList(), board.Select(t => t.Value).ToList());
        }

        public static bool BottomPairPlus(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            return BottomPair(holeCards.Select(t => t.Value).ToList(), board.Select(t => t.Value).ToList());
        }

        public static bool MiddlePair(IEnumerable<CardValue> holeCards, IEnumerable<CardValue> board)
        {
            var p = GetPairs(holeCards, board);
            var boardMin = board.Min(t => t);
            var boardMid = board.Where(t => t != boardMin).Min(t => t);

            return p.Count == 1 && p[0] == boardMid;
        }

        public static bool MiddlePair(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            return MiddlePair(holeCards.Select(t => t.Value).ToList(), board.Select(t => t.Value).ToList());
        }

        public static bool TopPair(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            return TopPair(holeCards.Select(t => t.Value).ToList(), board.Select(t => t.Value).ToList());
        }

        public static bool TopPair(IEnumerable<CardValue> holeCards, IEnumerable<CardValue> board)
        {
            var p = GetPairs(holeCards, board);
            return p.Contains(board.Max(t => t));
        }

        public static bool TwoPairs(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            return TwoPairs(holeCards.Select(t => t.Value).ToList(), board.Select(t => t.Value).ToList());
        }

        public static bool TwoPairs(IEnumerable<CardValue> holeCards, IEnumerable<CardValue> board)
        {
            if (Sets(holeCards, board) > 0)
                return false;

            var p = GetPairs(holeCards, board);
            return p.Count() >= 2;
        }

        public static bool TopTwoPairs(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            return TopTwoPairs(holeCards.Select(t => t.Value).ToList(), board.Select(t => t.Value).ToList());
        }

        public static bool TopTwoPairs(IEnumerable<CardValue> holeCards, IEnumerable<CardValue> board)
        {
            if (Sets(holeCards, board) > 0)
                return false;

            var p = GetPairs(holeCards, board);
            var sortedBoard = board.OrderByDescending(t => t).ToList();

            return p.Count() >= 2 && p.Contains(sortedBoard[0]) && p.Contains(sortedBoard[1]);
        }

        public static bool BottomTwoPairs(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            return BottomTwoPairs(holeCards.Select(t => t.Value).ToList(), board.Select(t => t.Value).ToList());
        }

        public static bool BottomTwoPairs(IEnumerable<CardValue> holeCards, IEnumerable<CardValue> board)
        {
            if (Sets(holeCards, board) > 0)
                return false;

            var p = GetPairs(holeCards, board);
            var sortedBoard = board.OrderBy(t => t).ToList();

            return p.Count() >= 2 && p.Contains(sortedBoard[0]) && p.Contains(sortedBoard[1]) && !p.Contains(sortedBoard.Last());
        }

        public static bool TopAndBottomPairs(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            return TopAndBottomPairs(holeCards.Select(t => t.Value), board.Select(t => t.Value));
        }

        public static bool TopAndBottomPairs(IEnumerable<CardValue> holeCards, IEnumerable<CardValue> board)
        {
            if (Sets(holeCards, board) > 0)
                return false;

            var p = GetPairs(holeCards, board);
            var sortedBoard = board.OrderByDescending(t => t).ToList();

            return p.Count() >= 2 && p.Contains(sortedBoard[0]) && !p.Contains(sortedBoard[1]) && p.Contains(sortedBoard.Last());
        }

        public static bool TpTk(IEnumerable<Card> holecards, IEnumerable<Card> board)
        {
            return TpTk(holecards.Select(t => t.Value).ToList(), board.Select(t => t.Value).ToList());
        }

        public static bool TpTk(IEnumerable<CardValue> holecards, IEnumerable<CardValue> board)
        {
            if (!TopPair(holecards, board))
                return false;

            var topBoardCard = board.Max(t => t);

            var kicker = CardValue.A;
            if (topBoardCard == CardValue.A)
                kicker = CardValue.K;

            return holecards.Any(t => t == kicker);
        }

        public static bool TwoOverPairs(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            return OverPairs(holeCards.Select(t => t.Value).ToList(), board.Select(t => t.Value).ToList()) == 2;
        }

        public static bool OverPair(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            return OverPairs(holeCards.Select(t => t.Value).ToList(), board.Select(t => t.Value).ToList()) >= 1;
        }

        public static int OverCards(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var maxBoard = board.Max(t => t.Value);
            var cnt = holeCards.Count(t => t.Value > maxBoard);
            return cnt;
        }

        public static int OverPairs(IEnumerable<CardValue> holeCards, IEnumerable<CardValue> board)
        {
            var boardMaxCard = board.Max(t => t);
            return holeCards.Where(t => t > boardMaxCard).GroupBy(t => t).Where(t => t.Count() > 1).Select(t => t.Key).Count();
        }

        private static List<int> SortValList(List<int> list)
        {
            var res = new List<int>();
            for (int i = 14; i >= 1; i--)
                if (list.Contains(i))
                    res.Add(i);

            return res;
        }

        private static bool FlashAvaiable(IEnumerable<Card> board)
        {
            return board.GroupBy(t => t.Suit).Any(t => t.Count() >= 3);
        }

        private static bool BoardPaired(IEnumerable<Card> board)
        {
            return board.GroupBy(t => t.Value).Any(t => t.Count() >= 2);
        }

        private static List<int> GetPossibleStraights(IEnumerable<Card> board)
        {
            var boardVals = board.Select(t => (int)t.Value).ToList();
            if (boardVals.Contains(14))
                boardVals.Add(1);

            var result = new List<int>();

            for (int i = 14; i > 3; i--)
            {
                var c = 0;
                for (int j = i; j > i - 5; j--)
                    if (boardVals.Contains(j))
                        c++;
                if (c >= 3)
                    result.Add(i);
            }

            return result;
        }

        private static List<int> CreateStartStraightVector(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var holeVals = holeCards.Select(t => (int)t.Value).ToList();
            if (holeVals.Contains(14))
                holeVals.Add(1);

            var boardVals = board.Select(t => (int)t.Value).ToList();
            if (boardVals.Contains(14))
                boardVals.Add(1);

            var startVector = new List<int>();
            for (int i = 0; i < 14; i++)
            {
                var h = holeVals.Contains(i + 1);
                var b = boardVals.Contains(i + 1);

                if (!h && !b)
                    startVector.Add(0);
                else if (h && !b)
                    startVector.Add(1);
                else if (!h && b)
                    startVector.Add(2);
                else
                    startVector.Add(3);
            }
            return startVector;
        }

        //0 - no, 1 - draw, >= 3 - straight
        private static int StraightValSingle(List<int> vector)
        {
            var hasDraw = false;
            for (int i = 13; i > 3; i--)
            {
                var c1 = 0;
                var c2 = 0;
                for (int j = i; j > i - 5; j--)
                {
                    if (vector[j] == 1)
                        c1++;
                    if (vector[j] == 2)
                        c2++;
                }
                if (c1 == 2 && c2 == 3)
                    return i+1;
                if (c1 >= 2 && c2 == 2)
                    hasDraw = true;
            }
            return hasDraw ? 1 : 0;
        }

        //0 - no or straigt
        private static int StraightDrawSingle(List<int> vector)
        {
            var res = 0;
            for (int i = 0; i < 10; i++)
            {
                var i0 = -1;
                var c1 = 0;
                var c2 = 0;
                var holecardsAdd = 0;
                for (int j = i; j < i + 5; j++)
                {
                    if (vector[j] == 0)
                        i0 = j;
                    else if (vector[j] == 1)
                    {
                        c1++;
                        holecardsAdd |= 0b10000000000000 >> j;
                    }
                    else if (vector[j] == 2)
                        c2++;
                }
                if (c1 == 2 && c2 == 3)
                    return 0;
                else if (c1 == 2 && c2 == 2)
                {
                    var add = 0b10000000000000 >> i0;
                    res |= add;
                }
                else if (c1 == 3 && c2 == 2)
                {
                    res |= holecardsAdd;
                }
            }
            return res;
        }

        private static int StraightDrawRecursive(List<int> vector)
        {
            var index = vector.IndexOf(3);
            if (index == -1)
                return StraightDrawSingle(vector);
            else
            {
                var vector1 = new List<int>();
                vector1.AddRange(vector);
                vector1[index] = 1;
                var s1 = StraightDrawRecursive(vector1);

                var vector2 = new List<int>();
                vector2.AddRange(vector);
                vector2[index] = 2;
                var s2 = StraightDrawRecursive(vector2);
                   
                return s1 | s2;
            }
        }

        private static int StraightValRecursive(List<int> vector)
        {
            var index = vector.IndexOf(3);
            if (index == -1)
                return StraightValSingle(vector);
            else
            {
                var vector1 = new List<int>();
                vector1.AddRange(vector);
                vector1[index] = 1;
                var s1 = StraightValRecursive(vector1);
                //if (s1 == 2)
                  //  return 2;

                var vector2 = new List<int>();
                vector2.AddRange(vector);
                vector2[index] = 2;
                var s2 = StraightValRecursive(vector2);

                return s2 > s1 ? s2 : s1;
            }
        }

        public static bool StraightFlush(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var boardSuits = board.GroupBy(t => t.Suit).Where(t => t.Count() >= 3).Select(t => t.Key).ToList();
            var holeSuits = holeCards.GroupBy(t => t.Suit).Where(t => t.Count() >= 2).Select(t => t.Key).ToList();
            if (!boardSuits.Any(t => holeSuits.Contains(t)))
                return false;

            var suit = boardSuits.First(t => holeSuits.Contains(t));
            holeCards = holeCards.Where(t => t.Suit == suit).ToList();
            board = board.Where(t => t.Suit == suit).ToList();

            return Straight(holeCards, board);
        }

        public static bool FlushRoyal(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var boardSuits = board.GroupBy(t => t.Suit).Where(t => t.Count() >= 3).Select(t => t.Key).ToList();
            var holeSuits = holeCards.GroupBy(t => t.Suit).Where(t => t.Count() >= 2).Select(t => t.Key).ToList();
            if (!boardSuits.Any(t => holeSuits.Contains(t)))
                return false;

            var suit = boardSuits.First(t => holeSuits.Contains(t));
            holeCards = holeCards.Where(t => t.Suit == suit).ToList();
            board = board.Where(t => t.Suit == suit).ToList();

            var startVector = CreateStartStraightVector(holeCards, board);
            var val = StraightValRecursive(startVector);

            return val == 14;
        }

        public static bool Straight(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var startVector = CreateStartStraightVector(holeCards, board);
            var val = StraightValRecursive(startVector);
            return val > 3;
        }

        public static bool StraightNut1(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var startVector = CreateStartStraightVector(holeCards, board);
            var val = StraightValRecursive(startVector);
            if (val < 3)
                return false;

            var possibleStraights = GetPossibleStraights(board);
            return val == possibleStraights[0];
        }

        public static bool StraightNut2(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var startVector = CreateStartStraightVector(holeCards, board);
            var val = StraightValRecursive(startVector);
            if (val < 3)
                return false;

            var possibleStraights = GetPossibleStraights(board);
            return possibleStraights.Count() > 1 && val == possibleStraights[1];
        }

        public static bool StraightDraw(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            if (board.Count() == 5)
                return false;

            var startVector = CreateStartStraightVector(holeCards, board);
            return StraightValRecursive(startVector) == 1;
        }

        public static int StraightDrawOuts(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            if (board.Count() == 5)
                return 0;

            var startVector = CreateStartStraightVector(holeCards, board);
            var res = StraightDrawRecursive(startVector);
            if (res == 0)
                return 0;

            var countOuts = 0;
            for (var i = 0; i <= 13; i++)
                countOuts += ((0b10000000000000 >> i) & res) > 0 ? 4 : 0;

            foreach (var h in holeCards)
            {
                if ((h.GetValueBinaryCode() & res) > 0)
                    countOuts--;
            }
            foreach (var b in board)
            {
                if ((b.GetValueBinaryCode() & res) > 0)
                    countOuts--;
            }

            return countOuts;
        }

        public static bool NoDraw(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            if (FlushDraw(holeCards, board))
                return false;

            if (StraightDraw(holeCards, board))
                return false;

            return true;
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

        public static int FlushBlockers(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var suitList = board.GroupBy(t => t.Suit).Where(t => t.Count() >= 3).Select(t => t.Key).ToList();
            if (!suitList.Any())
                return 0;

            var suit = suitList.First();
            return holeCards.Count(hc => hc.Suit == suit);
        }

        public static bool FlushBlockerNut(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var suitList = board.GroupBy(t => t.Suit).Where(t => t.Count() >= 3).Select(t => t.Key).ToList();
            if (!suitList.Any())
                return false;
            var suit = suitList.First();
            foreach (CardValue v in _cardValueList)
            {
                if (board.Any(b => b.Suit == suit && b.Value == v))
                    continue;
                return holeCards.Any(hc => hc.Suit == suit && hc.Value == v);
            }

            return false;
        }

        public static bool FlushBlockerNut2(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var suitList = board.GroupBy(t => t.Suit).Where(t => t.Count() >= 3).Select(t => t.Key).ToList();
            if (!suitList.Any())
                return false;
            var suit = suitList.First();
            var skipFirst = true;
            foreach (CardValue v in _cardValueList)
            {
                if (board.Any(b => b.Suit == suit && b.Value == v))
                    continue;

                if (skipFirst)
                {
                    skipFirst = false;
                    continue;
                }
                return holeCards.Any(hc => hc.Suit == suit && hc.Value == v);
            }

            return false;
        }

        public static int FlushDrawBlockers(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var suitList = board.GroupBy(t => t.Suit).Where(t => t.Count() == 2).Select(t => t.Key).ToList();
            if (!suitList.Any())
                return 0;

            var blockers = 0;
            foreach (var suit in suitList)
                blockers += holeCards.Count(hc => hc.Suit == suit);

            return blockers;
        }

        public static bool FlushDrawBlockerNut(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var suitList = board.GroupBy(t => t.Suit).Where(t => t.Count() == 2).Select(t => t.Key).ToList();
            if (!suitList.Any())
                return false;

            foreach (var suit in suitList)
            {
                foreach (CardValue v in _cardValueList)
                {
                    if (board.Any(b => b.Suit == suit && b.Value == v))
                        continue;

                    if (holeCards.Any(hc => hc.Suit == suit && hc.Value == v))
                        return true;

                    break;
                }
            }

            return false;
        }

        public static bool FlushDrawBlockerNut2(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var suitList = board.GroupBy(t => t.Suit).Where(t => t.Count() == 2).Select(t => t.Key).ToList();
            if (!suitList.Any())
                return false;

            foreach (var suit in suitList)
            {
                var skipFirst = true;
                foreach (CardValue v in _cardValueList)
                {
                    if (board.Any(b => b.Suit == suit && b.Value == v))
                        continue;

                    if (skipFirst)
                    {
                        skipFirst = false;
                        continue;
                    }

                    if (holeCards.Any(hc => hc.Suit == suit && hc.Value == v))
                        return true;

                    break;
                }
            }

            return false;
        }

        private static List<int> GetPossibleStraightBlockers(IEnumerable<Card> board)
        {
            var boardVals = board.Select(t => (int)t.Value).ToList();
            if (boardVals.Contains(14))
                boardVals.Add(1);

            var possibleBlockers = new HashSet<int>();
            for (int i = 14; i > 3; i--)
            {
                var tempList = new List<int>();
                var c = 0;
                for (int j = i; j > i - 5; j--)
                    if (boardVals.Contains(j))
                        c++;
                    else
                        tempList.Add(j);

                if (c == 3)
                {
                    foreach (var t in tempList)
                        possibleBlockers.Add(t);
                }
                else if (c > 3)
                {
                    for (int j = i; j > i - 5; j--)
                        possibleBlockers.Add(j);
                }
            }

            return possibleBlockers.OrderByDescending(t => t).ToList();
        }

        private static List<int> GetPossibleNutsStraightBlockers(IEnumerable<Card> board)
        {
            var boardVals = board.Select(t => (int)t.Value).ToList();
            if (boardVals.Contains(14))
                boardVals.Add(1);

            var possibleBlockers = new HashSet<int>();
            for (int i = 14; i > 3; i--)
            {
                var tempList = new List<int>();
                var c = 0;
                for (int j = i; j > i - 5; j--)
                    if (boardVals.Contains(j))
                        c++;
                    else
                        tempList.Add(j);

                if (c == 3)
                {
                    foreach (var t in tempList)
                        possibleBlockers.Add(t);
                    break;
                }
                else if (c > 3)
                {
                    for (int j = i; j > i - 5; j--)
                        possibleBlockers.Add(j);
                    break;
                }
            }

            return possibleBlockers.OrderByDescending(t => t).ToList();
        }

        public static int StraightBlockers(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var possibleBlockers = GetPossibleStraightBlockers(board);
            return holeCards.Count(t => possibleBlockers.Contains((int)t.Value));
        }

        public static int StraightBlockersNut(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var possibleNutBlockers = GetPossibleNutsStraightBlockers(board);
            return holeCards.Count(t => possibleNutBlockers.Contains((int)t.Value));
        }

        private static List<int> GetPossibleStraightDrawBlockers(IEnumerable<Card> board)
        {
            var boardVals = board.Select(t => (int)t.Value).ToList();
            if (boardVals.Contains(14))
                boardVals.Add(1);

            var possibleBlockers = new HashSet<int>();
            for (int i = 14; i > 3; i--)
            {
                var tempList = new List<int>();
                var c = 0;
                for (int j = i; j > i - 5; j--)
                    if (boardVals.Contains(j))
                        c++;
                    else
                        tempList.Add(j);

                if (c >= 3)
                    continue;

                if (c == 2)
                    foreach (var t in tempList)
                        possibleBlockers.Add(t);
            }

            return possibleBlockers.OrderByDescending(t => t).ToList();
        }

        private static List<int> GetPossibleStraightDrawNutBlockers(IEnumerable<Card> board)
        {
            var boardVals = board.Select(t => (int)t.Value).ToList();
            if (boardVals.Contains(14))
                boardVals.Add(1);

            var possibleBlockers = new HashSet<int>();
            for (int i = 14; i > 3; i--)
            {
                var tempList = new List<int>();
                var c = 0;
                for (int j = i; j > i - 5; j--)
                    if (boardVals.Contains(j))
                        c++;
                    else
                        tempList.Add(j);

                if (c >= 3)
                    continue;

                if (c == 2)
                {
                    foreach (var t in tempList)
                        possibleBlockers.Add(t);
                    break;
                }
            }

            return possibleBlockers.OrderByDescending(t => t).ToList();
        }

        public static int StraightDrawBlockers(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var possibleBlockers = GetPossibleStraightDrawBlockers(board);
            return holeCards.Count(t => possibleBlockers.Contains((int)t.Value));
        }

        public static int StraightDrawBlockersNut(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            var possibleBlockers = GetPossibleStraightDrawNutBlockers(board);
            return holeCards.Count(t => possibleBlockers.Contains((int)t.Value));
        }

        public static bool BackdoorStraightDraw(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            if(board.Count() > 3)
                return false;

            var startVector = CreateStartStraightVector(holeCards, board);
            return BackdoorStraightDrawRecursive(startVector) == 1;
        }
        
        //-1 - has SD or straight, 0 - not found, 1 - BDSD
        private static int BackdoorStraightDrawRecursive(List<int> vector)
        {
            var index = vector.IndexOf(3);
            if (index == -1)
                return BackdoorStraightDrawSingle(vector);
            else
            {
                var vector1 = new List<int>();
                vector1.AddRange(vector);
                vector1[index] = 1;
                var s1 = BackdoorStraightDrawRecursive(vector1);

                if (s1 == -1)
                    return -1;

                var vector2 = new List<int>();
                vector2.AddRange(vector);
                vector2[index] = 2;
                var s2 = BackdoorStraightDrawRecursive(vector2);

                if (s2 == -1)
                    return -1;

                return s1 == 1 || s2 == 1 ? 1 : 0;
            }
        }

        public static int BackdoorStraightDrawCount(IEnumerable<Card> holeCards, IEnumerable<Card> board)
        {
            if (board.Count() > 3)
                return 0;

            var startVector = CreateStartStraightVector(holeCards, board);
            var res = CalcBDSD_recursive(startVector);
            return res.Count();
        }

        //this is debug code, gives value greater then real with duplicates
        private static HashSet<int> CalcBDSD_recursive(List<int> vector)
        {
            var index = vector.IndexOf(3);
            if (index == -1)
                return CalcBDSD_single(vector);
            else
            {
                var vector1 = new List<int>();
                vector1.AddRange(vector);
                vector1[index] = 1;
                var s1 = CalcBDSD_recursive(vector1);

                var vector2 = new List<int>();
                vector2.AddRange(vector);
                vector2[index] = 2;
                var s2 = CalcBDSD_recursive(vector2);

                foreach (var item in s2)
                    s1.Add(item);

                return s1;
            }
        }

        //-1 - has SD or straight, 0 - not found, 1 - BDSD
        private static int BackdoorStraightDrawSingle(List<int> vector)
        {
            var res = 0;
            for (int i = 0; i < 10; i++)
            {
                var c1 = 0;
                var c2 = 0;
                for (int j = i; j < i + 5; j++)
                {
                    if (vector[j] == 1)
                        c1++;
                    else if (vector[j] == 2)
                        c2++;
                }
                if ((c1 == 2 || c1 == 3) && (c2 == 3 || c2 == 2))
                    return -1;
                else if (c1 >= 2 && c2 == 1)
                    res = 1;
            }
            return res;
        }

        private static int Vec2Int(int start, int count)
        {
            var result = 0;
            for (int i = start; i <= start + count; i++)
            {
                result += i * (int)Math.Pow(10, i - start);
            }
            return result;
        }

        private static HashSet<int> CalcBDSD_single(List<int> vector)
        {
            var res = new HashSet<int>();
            for (int i = 0; i < 10; i++)
            {
                var c1 = 0;
                var c2 = 0;
                for (int j = i; j < i + 5; j++)
                {
                    if (vector[j] == 1)
                        c1++;
                    else if (vector[j] == 2)
                        c2++;
                }
                if ((c1 == 2 || c1 == 3) && (c2 == 3 || c2 == 2))
                    return new HashSet<int>();
                else if (c1 >= 2 && c2 == 1)
                    res.Add(Vec2Int(i, 5));
            }
            return res;
        }

       
    }
}
