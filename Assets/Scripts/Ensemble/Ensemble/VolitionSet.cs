using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Ensemble
{
    public class VolitionSet
    {
        public Dictionary<string, Dictionary<string, List<Predicate>>> set;

        public VolitionSet()
        {
            set = new Dictionary<string, Dictionary<string, List<Predicate>>>();
        }

        public VolitionSet(Dictionary<string, Dictionary<string, List<Predicate>>> set)
        {
            this.set = set;
        }

        public List<string> getCharacters()
        {
            return set.Keys.ToList();
        }

        public List<Predicate> getVolitionsForCharacterSet(string from, string to)
        {
            if (!set.ContainsKey(from))
            {
                return null;
            } else
            {
                if (!set[from].ContainsKey(to)) {
                    return null;
                } else {
                    if (set[from] == null || set[from][to] == null)
                    {
                        return null;
                    }

                    return set[from][to];
                }
            }
        }

        public void setVolitionsForCharacterSet(string from, string to, List<Predicate> volitions)
        {
            set[from][to] = volitions;
        }

        public void addVolitionForCharacterSet(string from, string to, Predicate volition)
        {
            set[from][to].Add(volition);
        }

        public void initVolitionsForCharacterSet(string from, string to)
        {
            if (!set.ContainsKey(from))
            {
                set[from] = new Dictionary<string, List<Predicate>>();
            }
            
            set[from][to] = new List<Predicate>();
        }
    }
}
