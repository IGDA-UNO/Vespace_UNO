using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensemble
{
    internal class Util
    {
        private Dictionary<string, long> iteratorMap;

        public Util()
        {
            this.iteratorMap = new Dictionary<string, long>();
        }

        public long iterator(string value)
        {
            if (iteratorMap.ContainsKey(value))
            {
                iteratorMap[value] += 1;
                return iteratorMap[value];
            }
            else
            {
                iteratorMap[value] = 1;
                return iteratorMap[value];
            }
        }

        public void resetIterator(string value)
        {
            iteratorMap[value] = 0;
        }
    }
}

