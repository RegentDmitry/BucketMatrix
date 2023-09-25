using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace HoldemBucketing
{
    public class BucketItem
    {
        public string Bucket { get; set; }
        public int Order { get; set; }

        public BucketItem() { }

        private static int MaxOrder { get; set; } = 0;

        public BucketItem(string name, int order = 0)
        {
            Bucket = name;

            if (order > 0)
            {
                Order = order;
                MaxOrder = Math.Max(MaxOrder, order);
            }
            else
            {
                Order = ++MaxOrder;
            }
        }

        public override string ToString()
        {
            return $"{Bucket}";
        }


        private Range RecognizeBucketSequential(Range range, List<Card> board)
        {
            var newRange = new Range();
            foreach (var item in range)
            {
                if (InBuiltIn(item, board))
                    newRange.Add(item);
            }
            return newRange;
        }

        private Range RecognizeBucketParallel(Range range, List<Card> board)
        {
            var newRange = new Range();
            var tempRange = new ConcurrentBag<RangeItem>();
            Parallel.ForEach(range, item =>
            {
                if (InBuiltIn(item, board))
                    tempRange.Add(item);
            });
            var cnt = tempRange.Count(t => t == null);
            newRange.AddRange(tempRange);
            return newRange;
        }

        public Range RecognizeBucket(Range range, List<Card> board, bool parallel = true)
        {
            return parallel ? RecognizeBucketParallel(range, board) : RecognizeBucketSequential(range, board);
        }

        public bool InBuiltIn(RangeItem item, List<Card> board)
        {
            if (!board.Any())
                return false;

            switch (Bucket)
            {
                case "RoyalFlush": return Categories.FlushRoyal(item.Hand, board);
                case "StraightFlush": return Categories.StraightFlush(item.Hand, board);
                case "Quads": return Categories.FourOfKind(item.Hand, board);
                case "FullHouse": return Categories.FullHouse(item.Hand, board);
                case "Flush": return Categories.Flush(item.Hand, board);
                case "Straight": return Categories.Straight(item.Hand, board);
                case "TopSet": return Categories.TopSet(item.Hand, board);
                case "MidSet": return Categories.MiddleSet(item.Hand, board);
                case "BtmSet": return Categories.BottomSet(item.Hand, board);
                case "Set": return Categories.Sets(item.Hand, board) > 0;
                case "Trips": return Categories.Trips(item.Hand, board);
                case "TopTwo": return Categories.TopTwoPairs(item.Hand, board);
                case "TopAndBottom": return Categories.TopAndBottomPairs(item.Hand, board);
                case "BottomTwo": return Categories.BottomTwoPairs(item.Hand, board);
                case "TwoPairs": return Categories.TwoPairs(item.Hand, board);
                case "OverPair": return Categories.OverPair(item.Hand, board);
                case "TopPair": return Categories.TopPair(item.Hand, board);
                case "MiddlePair": return Categories.MiddlePair(item.Hand, board);
                case "BottomPair": return Categories.BottomPair(item.Hand, board);
                case "Pair": return Categories.Pair(item.Hand, board);
                case "NutFlush": return Categories.NutFlush(item.Hand, board);
                case "NutFlush2": return Categories.NutFlush2(item.Hand, board);
                case "FlushDraw": return Categories.FlushDraw(item.Hand, board);
                case "NutFlushDraw": return Categories.NutFlushDraw(item.Hand, board);
                case "NutFlushDraw2": return Categories.NutFlushDraw2(item.Hand, board);
                case "FlushDrawNotNut": return Categories.NotNutFlushDraw(item.Hand, board);
                case "TPTK": return Categories.TpTk(item.Hand, board);
                case "OverCard": return Categories.OverCards(item.Hand, board) > 0;
                case "OverCard1": return Categories.OverCards(item.Hand, board) == 1;
                case "OverCard2": return Categories.OverCards(item.Hand, board) == 2;
                case "StraightDraw": return Categories.StraightDraw(item.Hand, board);
                case "NoDraw": return Categories.NoDraw(item.Hand, board);
                case "Gutshot": return Categories.StraightDrawOuts(item.Hand, board) == 4;
                case "OESD": return Categories.StraightDrawOuts(item.Hand, board) == 8;
                case "BackdoorFlushdraw1": return Categories.BDFD(item.Hand, board) == 1;
                case "BackdoorFlushdraw2": return Categories.BDFD(item.Hand, board) == 2;
                case "BackdoorFlushdraw": return Categories.BDFD(item.Hand, board) > 0;
                case "BackdoorFlushdrawNut": return Categories.BDFDN(item.Hand, board);
                case "PocketPair": return Categories.PocketPair(item.Hand);
                case "FlushNotNut": return Categories.NotNutFlush(item.Hand, board);
                case "FullHouseNut": return Categories.FullHouseN(item.Hand, board);
                case "FullHouseNotNut": return Categories.FullHouseNN(item.Hand, board);
                case "StraightNut": return Categories.StraightNut1(item.Hand, board);
                case "StraightNut2": return Categories.StraightNut2(item.Hand, board);
                case "FlushBlocker": return !Categories.Flush(item.Hand, board) && Categories.FlushBlockers(item.Hand, board) > 0;
                case "FlushBlockerNut": return !Categories.Flush(item.Hand, board) && Categories.FlushBlockerNut(item.Hand, board);
                case "FlushBlockerNut2": return !Categories.Flush(item.Hand, board) && Categories.FlushBlockerNut2(item.Hand, board);
                case "FlushDrawBlocker": return Categories.FlushDrawBlockers(item.Hand, board) > 0;
                case "FlushDrawBlocker1": return !Categories.FlushDraw(item.Hand, board) && Categories.FlushDrawBlockers(item.Hand, board) == 1;
                case "FlushDrawBlocker2": return !Categories.FlushDraw(item.Hand, board) && Categories.FlushDrawBlockers(item.Hand, board) == 2;
                case "FlushDrawBlockerNut": return Categories.FlushDrawBlockerNut(item.Hand, board);
                case "FlushDrawBlockerNut2": return Categories.FlushDrawBlockerNut2(item.Hand, board);
                case "StraightBlocker": return Categories.StraightBlockers(item.Hand, board) > 0;
                case "StraightBlocker1": return Categories.StraightBlockers(item.Hand, board) == 1;
                case "StraightBlocker2": return Categories.StraightBlockers(item.Hand, board) == 2;
                case "StraightBlockerNut": return Categories.StraightBlockersNut(item.Hand, board) > 0;
                case "StraightBlockerNut1": return Categories.StraightBlockersNut(item.Hand, board) == 1;
                case "StraightBlockerNut2": return Categories.StraightBlockersNut(item.Hand, board) == 2;
                case "StraightDrawBlocker": return Categories.StraightDrawBlockers(item.Hand, board) > 0;
                case "StraightDrawBlocker1": return Categories.StraightDrawBlockers(item.Hand, board) == 1;
                case "StraightDrawBlocker2": return Categories.StraightDrawBlockers(item.Hand, board) == 2;
                case "StraightDrawBlockerNut": return Categories.StraightDrawBlockersNut(item.Hand, board) > 0;
                case "StraightDrawBlockerNut1": return Categories.StraightDrawBlockersNut(item.Hand, board) == 1;
                case "StraightDrawBlockerNut2": return Categories.StraightDrawBlockersNut(item.Hand, board) == 2;
                case "BackdoorStraightdraw": return Categories.BackdoorStraightDraw(item.Hand, board);
            }
            return false;
        }
    }

    public class Buckets : List<BucketItem>
    {
        public override string ToString()
        {
            return string.Join(";", this.Select(t => t.ToString()));
        }

        public Range RecognizeBucket(Range range, List<Card> board, BucketItem bucket, bool parallel = false)
        {
            return bucket.RecognizeBucket(range, board, parallel);
        }

        public void InitBuckets()
        {
            if (this.Any())
                Clear();

            Add(new BucketItem("RoyalFlush"));
            Add(new BucketItem("StraightFlush"));
            Add(new BucketItem("Quads"));
            Add(new BucketItem("FullHouse"));
            Add(new BucketItem("Flush"));
            Add(new BucketItem("Straight"));
            Add(new BucketItem("TopSet"));
            Add(new BucketItem("MidSet"));
            Add(new BucketItem("BtmSet"));
            Add(new BucketItem("Set"));
            Add(new BucketItem("Trips"));
            Add(new BucketItem("TopTwo"));
            Add(new BucketItem("TopAndBottom"));
            Add(new BucketItem("BottomTwo"));
            Add(new BucketItem("TwoPairs"));
            Add(new BucketItem("OverPair"));
            Add(new BucketItem("TopPair"));
            Add(new BucketItem("MiddlePair"));
            Add(new BucketItem("BottomPair"));
            Add(new BucketItem("Pair"));
            Add(new BucketItem("NutFlush"));
            Add(new BucketItem("NutFlush2"));
            Add(new BucketItem("FlushDraw"));
            Add(new BucketItem("NutFlushDraw"));
            Add(new BucketItem("NutFlushDraw2"));
            Add(new BucketItem("FlushDrawNotNut"));
            Add(new BucketItem("TPTK"));
            Add(new BucketItem("OverCard"));
            Add(new BucketItem("OverCard1"));
            Add(new BucketItem("OverCard2"));
            Add(new BucketItem("StraightDraw"));
            Add(new BucketItem("NoDraw"));
            Add(new BucketItem("Gutshot"));
            Add(new BucketItem("OESD"));
            Add(new BucketItem("BackdoorFlushdraw1"));
            Add(new BucketItem("BackdoorFlushdraw2"));
            Add(new BucketItem("BackdoorFlushdraw"));
            Add(new BucketItem("BackdoorFlushdrawNut"));
            Add(new BucketItem("PocketPair"));
            Add(new BucketItem("FlushNotNut"));
            Add(new BucketItem("FullHouseNut"));
            Add(new BucketItem("FullHouseNotNut"));
            Add(new BucketItem("StraightNut"));
            Add(new BucketItem("StraightNut2"));
            Add(new BucketItem("FlushBlocker"));
            Add(new BucketItem("FlushBlockerNut"));
            Add(new BucketItem("FlushBlockerNut2"));
            Add(new BucketItem("FlushDrawBlocker"));
            Add(new BucketItem("FlushDrawBlocker1"));
            Add(new BucketItem("FlushDrawBlocker2"));
            Add(new BucketItem("FlushDrawBlockerNut"));
            Add(new BucketItem("FlushDrawBlockerNut2"));
            Add(new BucketItem("StraightBlocker"));
            Add(new BucketItem("StraightBlocker1"));
            Add(new BucketItem("StraightBlocker2"));
            Add(new BucketItem("StraightBlockerNut"));
            Add(new BucketItem("StraightBlockerNut1"));
            Add(new BucketItem("StraightBlockerNut2"));
            Add(new BucketItem("StraightDrawBlocker"));
            Add(new BucketItem("StraightDrawBlocker1"));
            Add(new BucketItem("StraightDrawBlocker2"));
            Add(new BucketItem("StraightDrawBlockerNut"));
            Add(new BucketItem("StraightDrawBlockerNut1"));
            Add(new BucketItem("StraightDrawBlockerNut2"));
            Add(new BucketItem("BackdoorStraightdraw"));
        }

        public void Init()
        {
            InitBuckets();
        }

    }
}
