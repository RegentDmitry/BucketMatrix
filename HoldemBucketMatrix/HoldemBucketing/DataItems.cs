using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HoldemBucketing
{
    public class HandBucketItem
    {
        public bool[] BucketsVector { get; private set; }
        public double Weight { get; set; }
        public RangeItem Hand { get; set; }

        public HandBucketItem(int bucketsCount)
        {
            BucketsVector = new bool[bucketsCount];
        }

        public string GetCodeString()
        {
            return string.Join("", BucketsVector.Select(t => t ? "1" : "0"));
        }
    }

    public class HandBucketAction : Dictionary<long, HandBucketItem>
    {
        public int SituationId { get; set; }
        public string Action { get; set; }
    }

    public class HandBucketList : List<HandBucketAction>
    {

    }

    public class CodedSitiationItem
    {
        public string Code { get; set; }
        public List<string> Actions { get; set; } = new List<string>();
        public List<double> Weights { get; set; } = new List<double>();

        public List<Range> Ranges { get; set; } = new List<Range>();

        public double Confidence => Weights.Max() / Weights.Sum();

        public string ToCSV()
        {
            var result = new List<string>();
            for (int i = 0; i < Actions.Count(); i++)
            {
                result.Add($"{Code},{Actions[i]},{Math.Round(Weights[i],4).ToString().Replace(",",".")}");
            }
            return string.Join(Environment.NewLine, result);
        }

    }

    public class CodedSituationList : List<CodedSitiationItem>
    {
        
    }

    public enum CustomBoolean
    {
        True,
        False,
        Ignore
    }

    public class DetailedFormatItem
    {
        public XmlSerializableDictionary<string, double> Actions { get; set; } = new XmlSerializableDictionary<string, double>();
        public List<Range> Ranges { get; set; } = new List<Range>();

        public int MatchValue(DetailedFormatItem item)
        {
            var count = 0;
            for (int i = 0; i < item.Values.Count(); i++)
            {
                if (item.Values[i] == CustomBoolean.Ignore ||
                    Values[i] == CustomBoolean.Ignore ||
                    item.Values[i] == Values[i])
                {
                    count++;
                }
            }
            return count;
        }

        public double Confidence()
        {
            return Actions.Values.Max() / Actions.Values.Sum();
        }

        public List<CustomBoolean> Values { get; set; } = new List<CustomBoolean>();

        public DetailedFormatItem Clone()
        {
            var clone = new DetailedFormatItem();
            foreach (var a in Actions)
                clone.Actions[a.Key] = a.Value;

            clone.Values.AddRange(Values);

            return clone;
        }

        public string BestAction => Actions.OrderByDescending(t => t.Value).FirstOrDefault().Key;

        public DetailedFormatItem Merge(DetailedFormatItem item)
        {
            if (item == null)
                throw new Exception("wrong item");

            var result = new DetailedFormatItem();
            result.Values = new List<CustomBoolean>();
            result.Values.AddRange(Values);
            for (int i = 0; i < Values.Count(); i++)
                if (Values[i] != item.Values[i])
                    result.Values[i] = CustomBoolean.Ignore;

            foreach (var t in Actions)
                result.Actions[t.Key] = t.Value;

            foreach (var t in item.Actions)
            {
                if (result.Actions.ContainsKey(t.Key))
                    result.Actions[t.Key] += t.Value;
                else
                    result.Actions[t.Key] = t.Value;
            }

            return result;
        }

        public ulong CodeMajor { get; set; }
        public ulong MaskMajor { get; set; }
        public uint CodeMinor { get; set; }
        public uint MaskMinor { get; set; }

        public void UpdateBinary()
        {
            var t = GetCodeAndMask();
            CodeMajor = t.code_major;
            MaskMajor = t.mask_major;
            CodeMinor = t.code_minor;
            MaskMinor = t.mask_minor;
        }

        public static string GetReverseCode(List<CustomBoolean> values)
        {
            var copy = new List<CustomBoolean>();
            copy.AddRange(values);
            copy.Reverse();
            return GetCode(copy);
        }

        public static string GetCode(List<CustomBoolean> values)
        {
            var temp = values.Select(t =>
            {
                switch (t)
                {
                    case CustomBoolean.False: return "0";
                    case CustomBoolean.True: return "1";
                    case CustomBoolean.Ignore: return "-";
                }
                throw new Exception("unknown value");
            });
            return string.Join("", temp);
        }

        private string _code;
        public string Code()
        {
            if (string.IsNullOrEmpty(_code))
                _code = GetCode(Values);
            return _code;
        }

        public override string ToString()
        {
            return GetReverseCode(Values);
        }

        public void RefreshCode()
        {
            _code = string.Empty;
        }

        public DetailedFormatItem()
        {

        }

        public DetailedFormatItem(CodedSitiationItem item)
        {
            for (int i = 0; i < item.Actions.Count(); i++)
                Actions[item.Actions[i]] = item.Weights[i];

            for (int i = 0; i < item.Code.Length; i++)
                Values.Add(item.Code.Substring(i, 1) == "1" ? CustomBoolean.True : CustomBoolean.False);

            Ranges.AddRange(item.Ranges);
        }



        public (ulong code_major, uint code_minor, ulong mask_major, uint mask_minor) GetCodeAndMask()
        {
            return Utils.GetCodeAndMask(Values);
        }
    }

    public class DetailedSituationList : List<DetailedFormatItem>
    {
        public void UpdateBinary()
        {
            foreach (var i in this)
            {
                i.UpdateBinary();
            }
        }

        public List<DetailedFormatItem> FindBestItems(DetailedFormatItem item)
        {
            var result = new List<DetailedFormatItem>();
            var matches = this.Select(t => t.MatchValue(item)).ToList();
            var max = matches.Max();
            var res = new List<DetailedFormatItem>();
            for (int i = 0; i < matches.Count(); i++)
                if (matches[i] == max)
                    res.Add(this[i]);
            return res;
        }

        public void Save(string filename)
        {
            var s = new XmlSerializer(typeof(DetailedSituationList));
            var w = new StreamWriter(filename);
            s.Serialize(w, this);
            w.Flush();
            w.Close();
        }

        public static DetailedSituationList Load(string filename)
        {
            var s = new XmlSerializer(typeof(DetailedSituationList));
            var r = new StreamReader(filename);
            var result = (DetailedSituationList)s.Deserialize(r);
            r.Close();
            return result;
        }

        public DetailedSituationList Clone()
        {
            var clone = new DetailedSituationList();
            foreach (var item in this)
            {
                clone.Add(item);
            }
            return clone;
        }

        public static DetailedSituationList GetDetailedFormatList(CodedSituationList list)
        {
            var res = new DetailedSituationList();
            foreach (var item in list.OrderBy(t => t.Code))
                res.Add(new DetailedFormatItem(item));
            return res;
        }

        public void SetLiveItems(List<int> liveList)
        {
            var length = this.FirstOrDefault().Values.Count();
            foreach (var item in this)
            {
                for (var i = 0; i < item.Values.Count(); i++)
                    item.Values[i] = liveList.Contains(i) ? item.Values[i] : CustomBoolean.Ignore;
                item.RefreshCode();
            }
        }

        public double GetConfidentCoverage(double confidence)
        {
            var totalWeight = this.Sum(t => t.Actions.Sum(tt => tt.Value));
            return this.Where(t => t.Confidence() >= confidence).Sum(t => Math.Round(t.Actions.Sum(tt => tt.Value)*100.0 / totalWeight,2));
        }

        public List<(int index, bool sign)> GetBestItems(List<string> filterList)
        {
            var thisItems = this.Where(t => filterList.Contains(t.ToString()));

            var res = new List<(int index, double value, bool sign)>();
            var firstItem = thisItems.FirstOrDefault();
            if (firstItem == null)
                return null;

            var lenght = firstItem.Values.Count();
            var cb = GetConfidence(this);
            for (int i = 0; i < lenght; i++)
            {
                var negative = thisItems.Where(t => t.Values[i] == CustomBoolean.False);
                var positive = thisItems.Where(t => t.Values[i] == CustomBoolean.True);
                if (!positive.Any() || !negative.Any())
                    continue;

                var cl = GetConfidence(negative);
                var cr = GetConfidence(positive);

                cl = cl - cb;
                cr = cr - cb;

                if (cl > 0)
                {
                    var negativeSum = negative.Sum(t => t.Actions.Sum(tt => tt.Value)) * cl;
                    res.Add((i, negativeSum, false));
                }

                if (cr > 0)
                {
                    var positiveSum = positive.Sum(t => t.Actions.Sum(tt => tt.Value)) * cr;
                    res.Add((i, positiveSum, true));
                }
                
            }

            return res.OrderByDescending(t => t.value).Select(t => (t.index, t.sign)).ToList();
        }

        private static double GetConfidence(IEnumerable<DetailedFormatItem> list)
        {
            var dic = new Dictionary<string, double>();
            foreach (var item in list)
            {
                foreach (var a in item.Actions)
                {
                    if (dic.ContainsKey(a.Key))
                        dic[a.Key] += a.Value;
                    else
                        dic[a.Key] = a.Value;
                }
            }

            return dic.Values.Max() / dic.Values.Sum();  
        }

        public void Optimize(double confidenceThreshold)
        {
            var removedItemsCounter = -1;
            var passedItems = new List<string>();

            while (removedItemsCounter != 0)
            {
                if (removedItemsCounter == -1)
                    removedItemsCounter = 0;
                var sourceCandidates = this.Where(t => t.Confidence() >= confidenceThreshold && !passedItems.Contains(t.Code())).ToList();
                if (!sourceCandidates.Any() && removedItemsCounter != 0)
                {
                    removedItemsCounter = -1;
                    passedItems.Clear();
                    continue;
                }

                var item = sourceCandidates.FirstOrDefault();
                if (item == null)
                    continue;

                var mergeCandidates = new List<Tuple<DetailedFormatItem, double>>();
                for (int i = 0; i < item.Values.Count(); i++)
                {
                    if (item.Values[i] == CustomBoolean.Ignore)
                        continue;

                    var tempList = new List<CustomBoolean>();
                    tempList.AddRange(item.Values);
                    tempList[i] = tempList[i] == CustomBoolean.True ? CustomBoolean.False : CustomBoolean.True;
                    var newCode = DetailedFormatItem.GetCode(tempList);

                    var secondItem = this.FirstOrDefault(t => t.Code() == newCode);
                    if (secondItem == null)
                    {
                        continue;
                    }

                    var newitem = item.Merge(secondItem);
                    if (newitem.BestAction == item.BestAction && newitem.Confidence() >= confidenceThreshold)
                        mergeCandidates.Add(new Tuple<DetailedFormatItem, double>(secondItem, item.Confidence() - secondItem.Confidence()));
                }

                passedItems.Add(item.Code());

                if (!mergeCandidates.Any())
                    continue;

                var mergeList = mergeCandidates.OrderBy(t => t.Item2).Select(t => t.Item1).ToList();
                var mergedItem = item.Merge(mergeList[0]);
                removedItemsCounter++;
                Remove(mergeList[0]);
                Remove(item);

                for (int i = 1; i < mergeList.Count(); i++)
                {
                    var newMergeItem = mergedItem.Merge(mergeCandidates[i].Item1);
                    if (newMergeItem.Confidence() < confidenceThreshold)
                    {
                        mergedItem = newMergeItem;
                        Remove(mergeCandidates[i].Item1);
                        removedItemsCounter++;
                    }
                }
                Add(mergedItem);
            }

            Sort((x, y) => y.Actions.Sum(t => t.Value).CompareTo(x.Actions.Sum(t => t.Value)));
        }

        private char CustomBoolToChar(CustomBoolean b)
        {
            switch (b)
            {
                case CustomBoolean.False: return '0';
                case CustomBoolean.True: return '1';
                case CustomBoolean.Ignore: return '-';
            }
            throw new Exception("unknown value");
        }

        public string GetEspressoInput(double confidenceThreshold)
        {
            var possibleActions = new List<string>();
            foreach (var item in this)
                foreach (var a in item.Actions)
                    if (!possibleActions.Contains(a.Key))
                        possibleActions.Add(a.Key);

            var espressoInput = new List<Tuple<string, string>>();
            foreach (var cs in this.Where(t => t.Confidence() >= confidenceThreshold))
            {
                var argumentArray = new char[cs.Values.Count()];
                for (int i = 0; i < cs.Values.Count(); i++)
                    argumentArray[i] = CustomBoolToChar(cs.Values[i]);

                var bestIndex = cs.Actions.Keys.ToList().IndexOf(cs.BestAction);
                var resultsArray = new char[possibleActions.Count()];
                for (int i = 0; i < resultsArray.Count(); i++)
                    resultsArray[i] = '0';
                resultsArray[bestIndex] = '1';
                espressoInput.Add(new Tuple<string, string>(string.Join("", argumentArray), string.Join("", resultsArray)));
            }

            var result = $".i {this.First().Values.Count()} {Environment.NewLine}.o {possibleActions.Count()}{Environment.NewLine}";
            result = result + string.Join(Environment.NewLine, espressoInput.Select(t => $"{t.Item1} {t.Item2}"));
            return result;
        }

        private int CalcDigitOne(uint num)
        {
            uint l_one = 1;
            int counter = 0;
            for (int i = 0; i < 20; i++)
            {
                var shift = l_one << i;
                if ((shift & num) != 0)
                    counter++;
            }
            return counter;
        }

        private int CalcDigitOne(ulong num)
        {
            ulong l_one = 1;
            int counter = 0;
            for (int i = 0; i < 63; i++)
            {
                var shift = l_one << i;
                if ((shift & num) != 0)
                    counter++;
            }
            return counter;
        }

        public double CompareTo(DetailedSituationList item)
        {
            var totalSim = 0.0;
            var valuesCount = this[0].Values.Count();

            var totalSourceWeight = 0.0;

            foreach (var t in this)
            {
                var maxsim = 0;

                var checkWeight = 0.0;
                var raiseWeight = 0.0;

                //var t_vals = t.Values.ToArray();

                foreach (var tt in item)
                {
                    //var tt_vals = tt.Values.ToArray();
                    //var sim = 0;

                    ///
                    /*for (int i = 0; i < valuesCount; i++)
                    {
                        var a_val = t_vals[i];
                        var b_val = tt_vals[i];
                        if (a_val == b_val)
                            sim++;
                    }*/
                    
                    ///
                    var maskMajor = t.MaskMajor & tt.MaskMajor;
                    var maskMinor = t.MaskMinor & tt.MaskMinor;
                    var maskDiffMajor = t.MaskMajor ^ tt.MaskMajor;
                    var maskDiffMinor = t.MaskMinor ^ tt.MaskMinor;
                    var diffCodeMajor = t.CodeMajor ^ tt.CodeMajor;
                    var diffMajor = diffCodeMajor & maskMajor;
                    var diffCodeMinor = t.CodeMinor ^ tt.CodeMinor;
                    var diffMinor = diffCodeMinor & maskMinor;

                    var v1 = CalcDigitOne(maskDiffMajor | diffMajor);
                    var v2 = CalcDigitOne(maskDiffMinor | diffMinor);
                    var sim = valuesCount - v1 - v2;

                    if (sim >= maxsim)
                    {
                        if (sim > maxsim)
                        {
                            checkWeight = 0;
                            raiseWeight = 0;
                            maxsim = sim;
                        }

                        foreach (var a in tt.Actions)
                        {
                            if (a.Key == "Check")
                                checkWeight += a.Value;
                            else
                                raiseWeight += a.Value;
                        }
                    }
                }
                var sumWeight = checkWeight + raiseWeight;
                checkWeight = checkWeight / sumWeight;
                raiseWeight = raiseWeight / sumWeight;

                var sourceCheckWeight = 0.0;
                var sourceRaiseWeight = 0.0;
                foreach (var i in t.Actions)
                {
                    if (i.Key == "Check")
                        sourceCheckWeight = i.Value;
                    else
                        sourceRaiseWeight = i.Value;
                }
                var sumSourceWeight = sourceCheckWeight + sourceRaiseWeight;
                totalSourceWeight += sumSourceWeight;
                sourceCheckWeight = sourceCheckWeight / sumSourceWeight;
                sourceRaiseWeight = sourceRaiseWeight / sumSourceWeight;

                var diff = Math.Abs(sourceCheckWeight - checkWeight);
                totalSim += (1 - diff) * (maxsim*1.0 / valuesCount) * sumSourceWeight;
            }

            var res = totalSim / totalSourceWeight;

            return res;
        }


    }

}
