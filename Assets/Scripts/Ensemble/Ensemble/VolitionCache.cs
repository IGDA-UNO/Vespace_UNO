using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * This is the class VolitionCache, for caching and accessing calculated volitions for characters in Ensemble.
 *
 * The internal format for a volitions objects should be structured like this:
 * 		{
			"Simon": {
				"Monica": [
					{ "category": "network", "type": "buddy", "intentType": true, "weight": 20 },
					{ "category": "relationship", "type": "involved with", "intentType": true, "weight": 19 }
				]
			},
			"Monica": {
				"Simon": [
					{ "category": "network", "type": "romance", "intentType": false, "weight": 12 }
				]
			}
		}
 *
 */

namespace Ensemble
{
    public class VolitionCache
    {
        public Dictionary<string, VolitionSet> volitionCache = new Dictionary<string, VolitionSet>();
        private Dictionary<string, int> cachePositions = new Dictionary<string, int>();

        public Predicate getFirstVolition(string key, string from, string to)
        {
            VolitionSet vSet = volitionCache[key];
            if (vSet == null)
            {
                return null;
            }

            List<Predicate> volitionsForCharacterSet = vSet.getVolitionsForCharacterSet(from, to);

            if (volitionsForCharacterSet == null || volitionsForCharacterSet.Count == 0)
            {
                return null;
            }

            string cachePositionsKey = key + from + to;

            if (cachePositions.ContainsKey(cachePositionsKey))
            {
                cachePositions.Remove(cachePositionsKey);
            }

            cachePositions.Add(cachePositionsKey, 0);

            return volitionsForCharacterSet[0];
        }

        public Predicate getNextVolition(string key, string from, string to)
        {
            string cachePositionsKey = key + from + to;
            VolitionSet vSet = volitionCache[key];
            int pos;

            // If we have no cached position, act like getFirstVolition
            if (!cachePositions.ContainsKey(cachePositionsKey))
            {
                return getFirstVolition(key, from, to);
            } else {
                pos = cachePositions[cachePositionsKey];
            }

            List<Predicate> volitionsForCharacterSet = vSet.getVolitionsForCharacterSet(from, to);

            // If we are out of volitions, return undefined
            if (volitionsForCharacterSet == null || volitionsForCharacterSet.Count <= pos + 1 || volitionsForCharacterSet[pos+1] == null)
            {
                return null;
            }

            if (cachePositions.ContainsKey(cachePositionsKey))
            {
                cachePositions[cachePositionsKey] = cachePositions[cachePositionsKey] + 1;
            } else
            {
                cachePositions.Add(cachePositionsKey, 1);
            }
            
            pos = cachePositions[cachePositionsKey];
            return volitionsForCharacterSet[pos];
        }

        public Boolean isVolitionMatch(Predicate a, Predicate b)
        {
            return a.Category == b.Category && a.Type == b.Type && a.IntentType == b.IntentType;
        }

        public VolitionAcceptance isAccepted(string key, string initiator, string responder, Predicate predicate)
        {
            bool acceptIfNoMatch = true; // If no matching rules affect the decision, should the character accept or reject the game?
            int minimumWeightForAccept = 0;

            VolitionAcceptance returnObject = new VolitionAcceptance();
            returnObject.Accepted = acceptIfNoMatch;
            returnObject.ReasonsWhy = new List<Predicate>();

            Predicate thisV = getFirstVolition(key, responder, initiator);

            while (thisV != null)
            {
                if (isVolitionMatch(thisV, predicate)) {
                    returnObject.Weight = thisV.Weight;
                    if (thisV.Weight < minimumWeightForAccept)
                    {
                        returnObject.ReasonsWhy.Add(thisV);
                        returnObject.Accepted = false;
                        return returnObject;
                    } else
                    {
                        returnObject.ReasonsWhy.Add(thisV);
                        returnObject.Accepted = true;
                        return returnObject;
                    }
                }

                thisV = getNextVolition(key, responder, initiator);
            }

            return returnObject;
        }

        public void sortSet(VolitionSet unsortedSet)
        {
            List<string> characters = unsortedSet.getCharacters();

            foreach (string from in characters)
            {
                foreach (string to in characters)
                {
                    List<Predicate> unsortedVolitions = unsortedSet.getVolitionsForCharacterSet(from, to);

                    if (unsortedVolitions != null)
                    {
                        unsortedSet.setVolitionsForCharacterSet(from, to, unsortedVolitions.OrderBy(o => -1 * o.Weight).ToList());
                    }
                }
            }
        }

        public VolitionInterface register(string key, VolitionSet volitionSet)
        {
            sortSet(volitionSet);

            if (volitionCache.ContainsKey(key))
            {
                volitionCache.Remove(key);
            }

            volitionCache.Add(key, volitionSet);
            return new VolitionInterface(key, this);
        }

        //public void logSet(VolitionSet setToLog)
        //{
        //    foreach (var item in setToLog.set)
        //    {
        //        string key = item.Key;
        //        Debug.WriteLine("newSet from: " + key);
        //        Dictionary<string, List<Predicate>> volitions = item.Value;

        //        foreach (var inner in volitions)
        //        {
        //            string innerKey = inner.Key;
        //            Debug.WriteLine("newSet to: " + innerKey);

        //            List<Predicate> characterVolitions = inner.Value;

        //            foreach (Predicate pred in characterVolitions)
        //            {
        //                Debug.WriteLine("pred: " + pred.ToString());
        //            }
        //        }
        //    }
        //}

        public VolitionSet newSet(List<string> cast)
        {
            VolitionSet newSet = new VolitionSet();

            foreach(string from in cast)
            {
                foreach (string to in cast)
                {
                    newSet.initVolitionsForCharacterSet(from, to);
                }
            }

            return newSet;
        }

        private VolitionSet getVolitionCacheByKey(string key)
        {
            return volitionCache[key];
        }

        public List<Predicate> getAllVolitionsByKeyFromTo(string key, string from, string to)
        {
            return volitionCache[key].getVolitionsForCharacterSet(from, to);
        }

    }
}
