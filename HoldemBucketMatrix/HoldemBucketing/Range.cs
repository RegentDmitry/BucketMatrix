using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldemBucketing
{
    public class RangeItem
    {
        public List<Card> Hand { get; set; }
        public string HandString { get; set; }
        public double Weight { get; set; }

        public override string ToString()
        {
            return $"{HandString} {Weight}";
        }

        private long _hashCode = 0;
        public long HashCode
        {
            get
            {
                if (_hashCode == 0)
                    _hashCode = GetCardListHashCode(Hand);
                return _hashCode;
            }
        }

        private static long GetCardListHashCode(List<Card> Hand)
        {
            var intList = Hand.Select(t => (int)t.Value).ToList();
            intList.AddRange(Hand.Select(t => (int)t.Suit));
            long res = 0;
            foreach (var i in intList)
                res = res * 31 + i;

            return res;
        }
    }

    public class Range : List<RangeItem>
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Name} : {Count}";
        }
    }
}
