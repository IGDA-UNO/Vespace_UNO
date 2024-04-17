using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensemble
{
    public class VolitionInterface
    {
        private string key;
        private VolitionCache cache;

        public VolitionInterface(string key, VolitionCache cache)
        {
            this.key = key;
            this.cache = cache;
        }

        public Predicate getFirst(string first, string second)
        {
            return cache.getFirstVolition(key, first, second);
        }

        public Predicate getNext(string from, string to)
        {
            return cache.getNextVolition(key, from, to);
        }

        public int getWeight(string first, string second, Predicate pred)
        {
            // TODO: implement getWeight
            return 1;
        }

        public VolitionCache GetVolitionCache(){
            return this.cache;
        }

        public void dump()
        {
            // TODO: implement dump
        }

        public VolitionAcceptance isAccepted(string initiator, string responder, Predicate predicate)
        {
            return cache.isAccepted(key, initiator, responder, predicate);
        }
    }
}
