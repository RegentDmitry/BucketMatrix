using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoldemBucketing
{
    public class Situation
    {
        public string Board { get; set; }
        public List<Range> Ranges = new List<Range>();

        private HandBucketAction CalcHandBucketAction(Range detailedRange, List<Card> board, bool parallel)
        {
            var handBucketActionItem = new HandBucketAction();

            for (int i = 0; i < Data.BucketsLibrary.Count(); i++)
            {
                var bucketRange = Data.BucketsLibrary.RecognizeBucket(detailedRange, board, Data.BucketsLibrary[i], parallel);
                foreach (var hand in bucketRange)
                {
                    HandBucketItem handBucketItem = null;
                    if (!handBucketActionItem.ContainsKey(hand.HashCode))
                    {
                        handBucketItem = new HandBucketItem(Data.BucketsLibrary.Count())
                        {
                            Hand = hand,
                            Weight = hand.Weight
                        };
                        handBucketActionItem.Add(hand.HashCode, handBucketItem);
                    }
                    else
                    {
                        handBucketItem = handBucketActionItem[hand.HashCode];
                    }

                    handBucketItem.BucketsVector[i] = true;
                }
            }

            return handBucketActionItem;
        }

        public async Task<DetailedSituationList> FindBuckets(bool parallel = false, bool sortHand = true)
        {
            var task = await Task<DetailedSituationList>.Factory.StartNew(() =>
            {
                var result = new List<BucketItem>();
                var board = Card.ParseCards(Board);
                var tempHandBucketList = new HandBucketList();

                foreach (var range in Ranges)
                {
                    var handBucketActionItem = CalcHandBucketAction(range, board, parallel);
                    handBucketActionItem.Action = range.Name;
                    tempHandBucketList.Add(handBucketActionItem);
                }

                var liveBuckets = new List<int>();
                for (var i = 0; i < Data.BucketsLibrary.Count; i++)
                {
                    if (tempHandBucketList.Any(t => t.Values.Any(tt => tt.BucketsVector[i])))
                        liveBuckets.Add(i);
                }

                if (liveBuckets.Count() == 0)
                    return null;

                var codedSituationList = new CodedSituationList();
                foreach (var item in tempHandBucketList)
                {
                    var groupedData = item.Values.GroupBy(t => t.GetCodeString());
                    var codeWeights = groupedData.ToDictionary(t => t.Key, t => t.Sum(tt => tt.Weight));

                    var handDic = new Dictionary<string, List<RangeItem>>();
                    foreach (var gd in groupedData)
                    {
                        handDic[gd.Key] = new List<RangeItem>();
                        foreach (var gdItem in gd)
                            handDic[gd.Key].Add(new RangeItem()
                            {
                                Hand = gdItem.Hand.Hand,
                                Weight = gdItem.Hand.Weight
                            });
                    }
                    foreach (var cw in codeWeights)
                    {
                        var newRange = new Range();
                        newRange.AddRange(handDic[cw.Key]);
                        newRange.Name = item.Action;

                        var situation = codedSituationList.FirstOrDefault(t => t.Code == cw.Key);
                        if (situation == null)
                        {
                            situation = new CodedSitiationItem() { Code = cw.Key };
                            codedSituationList.Add(situation);
                        }
                        situation.Actions.Add(item.Action);
                        situation.Weights.Add(cw.Value);
                        situation.Ranges.Add(newRange);
                    }
                }

                var detailedList = DetailedSituationList.GetDetailedFormatList(codedSituationList);
                detailedList.SetLiveItems(liveBuckets);
                //detailedList.Optimize(0.8);
                //var c = detailedList.GetConfidentCoverage(0.8);
                return detailedList;

            });
            return task;
        }

    }
}
