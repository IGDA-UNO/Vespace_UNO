using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensemble
{
    internal class SocialRecord
    {
        private List<List<Predicate>> socialRecord;
        private int currentTimeStep = -1; // initialize to -1 (assumes we start at time 0 when playing)
        private Dictionary<string, dynamic> defaultValues;
        private Dictionary<string, int> maxValues;
        private Dictionary<string, int> minValues;
        private Dictionary<string, string> directions;
        private Dictionary<string, bool> isBooleans;
        private Dictionary<string, int?> durations;
        private Cast offstageCharacters; // characters that aren't in the current level
        private Cast eliminatedCharacters; // characters that are never coming back and have had references to/from removed
        private List<Predicate> matchedResults;
        private Dictionary<string, bool> matchedResultsStrings;
        private Util util;

        public SocialRecord(Util util)
        {
            this.util = util;

            this.socialRecord = new List<List<Predicate>>();
            this.defaultValues = new Dictionary<string, dynamic>();
            this.maxValues = new Dictionary<string, int>();
            this.minValues = new Dictionary<string, int>();
            this.directions = new Dictionary<string, string>();
            this.isBooleans = new Dictionary<string, bool>();
            this.durations = new Dictionary<string, int?>();
            this.offstageCharacters = new Cast();
            this.eliminatedCharacters = new Cast();
        }

        public int getLength()
        {
            return socialRecord.Count;
        }

        public int getLengthAtTimeStep(int timeStep)
        {
            if (timeStep < 0)
            {
                return -1;
            } else
            {
                return socialRecord[timeStep].Count;
            }
        }

        public int getCurrentTimeStep()
        {
            return currentTimeStep;
        }

        public void dumpSocialRecord()
        {
            // print social record
        }

        public List<Predicate> getSocialRecordCopyAtTimestep(int timeStep)
        {
            List<Predicate> record = socialRecord[timeStep].Select(predicate => (Predicate)predicate.DeepCopy()).ToList();

            if (record == null)
            {
                return new List<Predicate>();
            } else
            {
                return record;
            }
        }

        public List<List<Predicate>> getSocialRecordCopy()
        {
            return socialRecord.Select(list => list.Select(predicate => (Predicate)predicate.DeepCopy()).ToList()).ToList();
        }

        public void registerMaxValue(Predicate predicate)
        {
            string category = predicate.Category;
            int? maxValue = predicate.MaxValue;
            maxValues[category] = maxValue != null ? (int)maxValue : 100;
        }

        public int? getRegisteredMaxValue(Predicate predicate)
        {
            if (maxValues.ContainsKey(predicate.Category))
            {
                return maxValues[predicate.Category];
            }
            else
            {
                return null;
            }
        }

        public void registerMinValue(Predicate predicate)
        {
            string category = predicate.Category;
            int? minValue = predicate.MinValue;
            minValues[category] = minValue != null ? (int)minValue : 0;
        }

        public int? getRegisteredMinValue(Predicate predicate)
        {
            if (minValues.ContainsKey(predicate.Category))
            {
                return minValues[predicate.Category];
            }
            else
            {
                return null;
            }
        }

        public void registerDuration(Predicate predicate)
        {
            durations[predicate.Category] = predicate.Duration;
        }

        public int? getRegisteredDuration(Predicate predicate)
        {
            if (durations.ContainsKey(predicate.Category))
            {
                return durations[predicate.Category];
            } else
            {
                return null;
            }
        }

        public void registerDirection(Predicate predicate)
        {
            directions[predicate.Category] = predicate.DirectionType;
        }

        public string getRegisteredDirection(Predicate predicate)
        {
            if (directions.ContainsKey(predicate.Category))
            {
                return directions[predicate.Category];
            }
            else
            {
                return null;
            }
        }

        public void registerDefault(Predicate predicate)
        {
            registerDefaultValue(predicate);
        }

        public void registerDefault(Condition condition)
        {
            registerDefaultValue(condition);
        }

        public void registerDefault(Effect effect)
        {
            registerDefaultValue(effect);
        }

        public void registerDefaultValue(Predicate predicate)
        {
            defaultValues[predicate.Category] = predicate.DefaultValue;
        }

        public dynamic getRegisteredDefaultValue(Predicate predicate)
        {
            return defaultValues[predicate.Category];
        }

        public void registerIsBoolean(Predicate predicate)
        {
            isBooleans[predicate.Category] = (bool)predicate.IsBoolean;
        }

        public bool? getRegisteredIsBoolean(Predicate predicate)
        {
            if (predicate.Category == null)
            {
                return null;
            }
            else if (isBooleans.ContainsKey(predicate.Category))
            {
                return isBooleans[predicate.Category];
            }
            else
            {
                return null;
            }
        }

        public int setupNextTimeStep()
        {
            return setupNextTimeStep(currentTimeStep + 1);
        }

        public int setupNextTimeStep(int? timeStep)
        {
            if (currentTimeStep == -1)
            {
                currentTimeStep += 1;
            }

            if (timeStep == null)
            {
                timeStep = currentTimeStep + 1;
            }

            if (socialRecord.Count <= currentTimeStep)
            {
                socialRecord.Add(new List<Predicate>());
            }

            for (int i = currentTimeStep + 1; i <= timeStep; i++)
            {
                socialRecord.Add(new List<Predicate>());
                if (socialRecord[i - 1] != null)
                {
                    for (int k = 0; k < socialRecord[i - 1].Count; k++)
                    {
                        if (getRegisteredDuration(socialRecord[i - 1][k]) != 0)
                        {
                            Predicate newRec = socialRecord[i - 1][k].DeepCopy();
                            socialRecord[i].Add(newRec);
                        }
                    }
                }

                for (int j = 0; j < socialRecord[i].Count; j++)
                {
                    if (getRegisteredIsBoolean(socialRecord[i][j]) == true)
                    {
                        if (socialRecord[i][j].Duration != null)
                        {
                            socialRecord[i][j].Duration = socialRecord[i][j].Duration - 1;
                            if (socialRecord[i][j].Duration <= 0)
                            {
                                socialRecord[i][j].Duration = null;

                                if (socialRecord[i][j].IsBoolean == true && socialRecord[i][j].Value != 0)
                                {
                                    socialRecord[i][j].Value = 0;
                                    socialRecord[i][j].TimeHappened = (int)timeStep;
                                }
                            }
                        }
                    }
                }
            }

            if (timeStep > currentTimeStep)
            {
                currentTimeStep = (int)timeStep;
            }
            else if (timeStep < currentTimeStep)
            {
                for (int i = currentTimeStep; i > timeStep; i--)
                {
                    socialRecord[i] = new List<Predicate>();
                }
                currentTimeStep = (int)timeStep;
            }

            return currentTimeStep;
        }

        public bool checkValueMatch(dynamic socialRecordValue, dynamic searchValue, string opt)
        {
            if (searchValue is string)
            {
                if (searchValue == "any")
                {
                    return true;
                }

                if (socialRecordValue is string && Equals(searchValue, socialRecordValue))
                {
                    return true;
                }
            }

            if (searchValue is bool && socialRecordValue is bool)
            {
                return searchValue == socialRecordValue;
            }

            if ((searchValue is int && (searchValue == 1 || searchValue == 0)) && socialRecordValue is bool)
            {
                bool searchValueBool = searchValue == 1 ? true : false;
                return searchValueBool == socialRecordValue;
            }

            if ((socialRecordValue is int && (socialRecordValue == 1 || socialRecordValue == 0)) is int && searchValue is bool)
            {
                bool socialRecordValueBool = socialRecordValue == 1 ? true : false;
                return searchValue == socialRecordValueBool;
            }

            if ((searchValue is int || searchValue is long) && (socialRecordValue is int || socialRecordValue is long))
            {
                if (opt == "=" && socialRecordValue != searchValue)
                {
                    return false;
                }

                if (opt == ">" && socialRecordValue <= searchValue)
                {
                    return false;
                }

                if (opt == "<" && socialRecordValue >= searchValue)
                {
                    return false;
                }

                return true;
            }

            return true;
        }

        public void addResult(Predicate predicateRef, dynamic value, bool addAsReference)
        {
            Predicate matchResult;

            //Debug.WriteLine("addResult: " + predicateRef);

            if (addAsReference)
            {
                matchResult = predicateRef.DeepCopy();
            }
            else
            {
                matchResult = predicateRef;
                if (value != null)
                {
                    matchResult.Value = value;
                }
            }

            foreach(Predicate res in matchedResults.ToList())
            {
                if (matchResult.Equals(res))
                {
                    return;
                }
            }

            matchedResults.Add(matchResult);
        }

        public void checkForDefaultMatch(Predicate searchPredicate, dynamic defaultValue, dynamic searchValue, bool isBooleanPred)
        {
            bool matchesDefault;

            if (searchPredicate.Value != null)
            {
                string operation = searchPredicate.Operator;

                matchesDefault = checkValueMatch(defaultValue, searchValue, operation != null ? operation : "=");

                if (matchesDefault)
                {
                    addResult(searchPredicate, defaultValue, false);
                }
            }
            else if (searchPredicate.Value == null && !isBooleanPred && defaultValue != null)
            {
                Predicate tempPred = searchPredicate.DeepCopy();
                tempPred.Value = defaultValue;
                // TODO: Figure out what this socialRecord.Add call was for

                socialRecord[currentTimeStep].Add(tempPred);
                addResult(tempPred, defaultValue, true);
            }
            else if (searchPredicate.Value == null && isBooleanPred && defaultValue != null)
            {
                string operation = searchPredicate.Operator;

                matchesDefault = checkValueMatch(defaultValue, searchValue, operation != null ? operation : "=");
                if (matchesDefault)
                {
                    Predicate tempBoolPred = searchPredicate.DeepCopy();
                    addResult(tempBoolPred, defaultValue, false);
                }
            }
        }

        public List<Predicate> get(Predicate _searchPredicate, int mostRecentTime, int lessRecentTime, bool useDefaultValue, int? timeStep)
        {
            Predicate searchPredicate = _searchPredicate.DeepCopy();
            // Debug.WriteLine("searchPredicate: " + searchPredicate.ToString());

            //int index = 0;
            //foreach (List<Predicate> record in socialRecord)
            //{
            //    Debug.WriteLine("socialRecord: " + index);

            //    foreach (Predicate pred in record)
            //    {
            //        Debug.WriteLine("socialRecord entry: " + pred.ToString());
            //    }

            //    index++;
            //}

            if (searchPredicate.Value is bool)
            {
                searchPredicate.IsBoolean = true;
                searchPredicate.Value = searchPredicate.Value ? 1 : 0;
            }

            dynamic searchValue = searchPredicate.Value;
            dynamic defaultValue = searchPredicate.Category != null && defaultValues.ContainsKey(searchPredicate.Category) ? defaultValues[searchPredicate.Category] : null;
            bool? isBooleanPred = getRegisteredIsBoolean(searchPredicate);
            bool foundPatternMatch = false;

            matchedResults = new List<Predicate>();
            matchedResultsStrings = new Dictionary<string, bool>();

            if (searchValue == null && useDefaultValue == true)
            {
                if (isBooleanPred == true)
                {
                    searchValue = 1;
                }
            }

            // Set default operator to "=" even for boolean predicates, because
            // we use 0 and 1 for boolean predicates.
            if (searchValue != null && searchPredicate.Operator == null)
            {
                searchPredicate.Operator = "=";
            }

            int currentTimeStepToUse = currentTimeStep;

            if (timeStep != null) {
                currentTimeStepToUse = (int)timeStep;
            }

            mostRecentTime = currentTimeStepToUse - mostRecentTime;
            lessRecentTime = currentTimeStepToUse - lessRecentTime;

            bool foundAnySocialRecordTimeSteps = false;

            if (mostRecentTime > (socialRecord.Count - 1)) mostRecentTime = socialRecord.Count - 1;
            if (lessRecentTime > (socialRecord.Count - 1)) lessRecentTime = socialRecord.Count - 1;

            if (mostRecentTime >= 0 || lessRecentTime >= 0)
            {
                if (mostRecentTime < 0) mostRecentTime = 0;
                if (lessRecentTime < 0) lessRecentTime = 0;

                if (socialRecord.Count > 0)
                {
                    for (int i = (int)lessRecentTime; i <= mostRecentTime; i++)
                    {
                        if (socialRecord[i] != null)
                        {
                            foundAnySocialRecordTimeSteps = true;
                            for (int j = 0; j < socialRecord[i].Count; j++)
                            {
                                Predicate socialRecordPredicate = socialRecord[i][j];

                                if (socialRecordPredicate.Active == false)
                                {
                                    continue;
                                }
                                if (searchPredicate.Category != null && searchPredicate.Category != socialRecordPredicate.Category)
                                {
                                    continue;
                                }
                                if (searchPredicate.Type != null && searchPredicate.Type != socialRecordPredicate.Type)
                                {
                                    continue;
                                }
                                if (searchPredicate.First != null && searchPredicate.First != socialRecordPredicate.First)
                                {
                                    continue;
                                }
                                if (searchPredicate.Second != null && searchPredicate.Second != socialRecordPredicate.Second)
                                {
                                    continue;
                                }

                                foundPatternMatch = true;

                                bool doesValueMatch = checkValueMatch(socialRecordPredicate.Value, searchValue, searchPredicate.Operator);

                                if (doesValueMatch)
                                {
                                    addResult(socialRecordPredicate, searchValue, true);
                                }
                            }

                            if (!foundPatternMatch)
                            {
                                checkForDefaultMatch(searchPredicate, defaultValue, searchValue, isBooleanPred == true);
                            }
                            foundPatternMatch = false;
                        }
                    }
                }
            }

            if (!foundAnySocialRecordTimeSteps)
            {
                checkForDefaultMatch(searchPredicate, defaultValue, searchValue, isBooleanPred == true);
            }

            //Debug.WriteLine("matchResults.Count: " + matchedResults.Count);
            //foreach (Predicate pred in matchedResults)
            //{
            //    Debug.WriteLine("SocialRecord.get() result: " + pred);
            //}

            return matchedResults;
        }

        public List<Predicate> get(Predicate searchPredicate, int mostRecentTime, int lessRecentTime, bool useDefaultValue)
        {
            return get(searchPredicate, mostRecentTime, lessRecentTime, useDefaultValue, null);
        }

        public List<Predicate> get(Predicate searchPredicate, int mostRecentTime, int lessRecentTime)
        {
            return get(searchPredicate, mostRecentTime, lessRecentTime, true, null);
        }

        public List<Predicate> get(Predicate searchPredicate, int mostRecentTime)
        {
            return get(searchPredicate, mostRecentTime, 0, true, null);
        }

        public List<Predicate> get(Predicate searchPredicate)
        {
            return get(searchPredicate, 0, 0, true, null);
        }

        public void addHistory(string sourceFile, List<History> history)
        {
            long lastPos = -9999999999;
            for (int i = 0; i < history.Count; i++)
            {
                History historyAtTime = history[i];
                if (historyAtTime.Pos <= lastPos)
                {
                    // log error
                    return;
                }
                lastPos = historyAtTime.Pos;
                setupNextTimeStep(historyAtTime.Pos);
                for (int j = 0; j < historyAtTime.data.Count; j++)
                {
                    Predicate pred = historyAtTime.data[j];
                    pred.Origin = sourceFile;
                    try
                    {
                        set(pred);
                    } catch(Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        return;
                    }
                }
            }
        }

        public string predicateHash(Object obj)
        {
            return "abcde";
        }

        //var predicateToString = function(){
        //    var returnString = "";
		//    for (var key in this) {
        //        returnString += key + ": " + this[key] + ", ";
		//	    console.log(key + ": " + this[key]);
		//    }
		// return returnString;
	    //};

        // TODO: Currently, you MUST include IsBoolean when setting a predicate. We need to change this.

        public void set(Predicate setPredicate)
        {
            Predicate pattern = new Predicate();
            pattern.Category = setPredicate.Category;
            pattern.Type = setPredicate.Type;
            pattern.First = setPredicate.First;
            pattern.Second = setPredicate.Second;
            pattern.Origin = setPredicate.Origin;
            pattern.IsBoolean = setPredicate.IsBoolean;

            if (setPredicate.IsBoolean == true && setPredicate.Value is bool)
            {
                setPredicate.Value = setPredicate.Value == true ? 1 : 0;
            }

            dynamic value = setPredicate.Value;
            string operation = setPredicate.Operator;

            bool? isBooleanPred = getRegisteredIsBoolean(setPredicate);
            bool isReciprocal = getRegisteredDirection(setPredicate) == "reciprocal";
            dynamic defaultValue = defaultValues.ContainsKey(pattern.Category) ? defaultValues[pattern.Category] : null;
            int? duration = getRegisteredDuration(setPredicate);
            int? max = getRegisteredMaxValue(setPredicate);
            int? min = getRegisteredMinValue(setPredicate);

            int timeStep = getCurrentTimeStep();
            if (timeStep == -1)
            {
                setupNextTimeStep(0);
                timeStep = 0;
            }

            // rather than use true and false, we could just stick to 1 and 0 like in C
            // but... we probably shouldn't let users pass in a boolean predicate without a value
            if (isBooleanPred == true && value == null)
            {
                value = 1;
            }

            Predicate socialRecordPredicate = new Predicate();
            List<Predicate> searchResult = get(pattern, 0, 0, false);

            if (searchResult.Count == 0)
            {
                socialRecordPredicate = pattern;
                socialRecordPredicate.Value = defaultValue;
                if (socialRecordPredicate.Active == null && setPredicate.Active != null)
                {
                    socialRecordPredicate.Active = setPredicate.Active;
                }
                socialRecordPredicate.ID = util.iterator("socialRecords").ToString();
            }
            else if (searchResult.Count == 1)
            {
                socialRecordPredicate = searchResult[0];
                socialRecordPredicate.ID = util.iterator("socialRecords").ToString();
            }
            else
            {
                // log bad predicate

            }

            socialRecordPredicate.TimeHappened = timeStep;
            socialRecordPredicate.Duration = duration;

            if (operation == null || operation == "=")
            {
                socialRecordPredicate.Value = value;
            }
            else
            {
                if (operation == "+")
                {
                    socialRecordPredicate.Value += value;
                }
                else if (operation == "-")
                {
                    socialRecordPredicate.Value -= value;
                }
            }

            if (setPredicate.IsBoolean == false && (isBooleanPred == null || isBooleanPred == false))
            {
                if (socialRecordPredicate.Value > max)
                {
                    socialRecordPredicate.Value = max;
                }
                if (socialRecordPredicate.Value < min)
                {
                    socialRecordPredicate.Value = min;
                }
            }

            if (searchResult.Count == 1)
            {
                bool updatedMatch = false;

                for (int i = 0; i < socialRecord[timeStep].Count; i++)
                {
                    Predicate recordCopy = socialRecord[timeStep][i].DeepCopy();
                    Predicate searchCopy = searchResult[0].DeepCopy();
                    recordCopy.Value = null;
                    searchCopy.Value = null;

                    //Debug.WriteLine("recordCopy: " + recordCopy.ToString());
                    //Debug.WriteLine("searchCopy: " + searchCopy.ToString());

                    if (recordCopy.ExistingPredicate(searchCopy))
                    {
                        //Debug.WriteLine("found match!!!: " + socialRecordPredicate.ToString());
                        socialRecord[timeStep][i] = socialRecordPredicate;
                        updatedMatch = true;
                    }
                }

                if (!updatedMatch)
                {
                    socialRecord[timeStep].Add(socialRecordPredicate);
                }
            } else
            {
                socialRecord[timeStep].Add(socialRecordPredicate);
            }

            if (isReciprocal)
            {
                Predicate recipPredicate = setPredicate.DeepCopy();
                string temp = recipPredicate.Second;
                recipPredicate.Second = recipPredicate.First;
                recipPredicate.First = temp;
                // use null for unset searchValue instead of undefined, as in ensemble.js
                recipPredicate.Value = null;
                recipPredicate.Active = setPredicate.Active;
                recipPredicate.ID = null;

                Predicate rPred;
                List<Predicate> recipSearchResult = get(recipPredicate, 0, 0, false);
                if (recipSearchResult.Count == 1)
                {
                    rPred = recipSearchResult[0];
                } else
                {
                    rPred = recipPredicate;
                }

                rPred.TimeHappened = socialRecordPredicate.TimeHappened;
                rPred.Duration = socialRecordPredicate.Duration;
                rPred.Value = socialRecordPredicate.Value;

                if (recipSearchResult.Count == 1)
                {
                    bool updatedRecipMatch = false;

                    for (int i = 0; i < socialRecord[timeStep].Count; i++)
                    {
                        Predicate recipRecordCopy = socialRecord[timeStep][i].DeepCopy();
                        Predicate recipSearchCopy = recipSearchResult[0].DeepCopy();
                        recipRecordCopy.Value = null;
                        recipSearchCopy.Value = null;

                        if (recipRecordCopy.ExistingPredicate(recipSearchCopy))
                        {
                            socialRecord[timeStep][i] = rPred;
                            updatedRecipMatch = true;
                        }
                    }

                    if (!updatedRecipMatch)
                    {
                        socialRecord[timeStep].Add(rPred);
                    }
                }
                else
                {
                    socialRecord[timeStep].Add(rPred);
                }
            }
        }

        public bool setById(string ID, Predicate newRecord)
        {
            if (ID == null)
            {
                return false;
            }

            for (int timeStep = 0; timeStep < socialRecord.Count; timeStep++)
            {
                for (int j = 0; j < socialRecord[timeStep].Count; j++)
                {
                    if (socialRecord[timeStep][j].ID == ID)
                    {
                        socialRecord[timeStep][j] = newRecord;
                        return true;
                    }
                }
            }
            return false;
        }

        public void clearHistory()
        {
            socialRecord = new List<List<Predicate>>();
            currentTimeStep = -1;
        }

        public void clearEverything()
        {
            socialRecord = new List<List<Predicate>>();
            currentTimeStep = -1;
            defaultValues = new Dictionary<string, dynamic>();
            maxValues = new Dictionary<string, int>();
            minValues = new Dictionary<string, int>();
            directions = new Dictionary<string, string>();
            isBooleans = new Dictionary<string, bool>();
            durations = new Dictionary<string, int?>();
            offstageCharacters = new Cast();
            eliminatedCharacters = new Cast();
        }

        public string socialRecordHistoryToString(int? specifiedTimeStep)
        {
            int timeStep;
            if (specifiedTimeStep == null)
            {
                timeStep = currentTimeStep;
            } else
            {
                timeStep = (int)specifiedTimeStep;
            }

            string historyString = "******socialRecord At Time " + timeStep + "********\n";

            for (var i = 0; i < socialRecord[timeStep].Count; i += 1)
            {
                historyString += "<PREDICATE " + i + ">\n";
                historyString += "category: " + socialRecord[timeStep][i].Category + "\n";
                historyString += "type: " + socialRecord[timeStep][i].Type + "\n";
                historyString += "first: " + socialRecord[timeStep][i].First + "\n";
                historyString += "second: " + socialRecord[timeStep][i].Second + "\n";
                historyString += "value: " + socialRecord[timeStep][i].Value + "\n";
                historyString += "timeHappened: " + socialRecord[timeStep][i].TimeHappened + "\n";
                historyString += "---------------------------\n";
            }

            historyString += "Total Length: " + socialRecord[timeStep].Count + "\n";
            historyString += "******************************";
            return historyString;
        }

        public string socialRecordFullHistoryToString()
        {
            string returnString = "";
            for (int i = 0; i < socialRecord.Count; i++)
            {
                returnString += socialRecordHistoryToString(i);
            }
            return returnString;
        }

        public void putCharacterOffstage(string characterName)
        {
            if (!offstageCharacters.Contains(characterName))
            {
                offstageCharacters.Add(characterName);
            }
            else
            {
                Debug.WriteLine("Character was already offstage, doing nothing...");
            }
        }

        public bool getIsCharacterOffstage(string characterName)
        {
            return offstageCharacters.Contains(characterName);
        }

        public void eliminateCharacter(string characterName)
        {
            if (!eliminatedCharacters.Contains(characterName))
            {
                eliminatedCharacters.Add(characterName);

                if (offstageCharacters.Contains(characterName))
                {
                    offstageCharacters.Remove(characterName);
                }
                
                removeAllSocialFactsFromCharacter(characterName);
            }
            else
            {
                Debug.WriteLine("Character was already eliminated, doing nothing...");
            }
        }

        public bool getIsCharacterEliminated(string characterName)
        {
            return eliminatedCharacters.Contains(characterName);
        }

        public void putCharacterOnstage(string characterName)
        {
            offstageCharacters.Remove(characterName);
        }

        public void removeAllSocialFactsFromCharacter(string characterName)
        {
            removeSocialFactsByDirection(characterName, "undirected");
            removeSocialFactsByDirection(characterName, "directed");
            removeSocialFactsByDirection(characterName, "reciprocal");
        }

        public void removeSocialFactsByDirection(string characterName, string direction)
        {
            for (int i = 0; i < socialRecord[currentTimeStep].Count; i++)
            {
                Predicate socialFact = socialRecord[currentTimeStep][i];
                if (getRegisteredDirection(socialFact) == direction)
                {
                    if (socialFact.First == characterName || socialFact.Second == characterName)
                    {
                        socialRecord[currentTimeStep].RemoveAt(i);
                    }
                }
            }
        }

        public Cast getOffstageCharacters()
        {
            return offstageCharacters;
        }

        public Cast getEliminatedCharacters()
        {
            return eliminatedCharacters;
        }

        public List<Predicate> publicGet(Predicate predicate, int earliestTime, int latestTime, bool useDefaultValue, int timeStep)
        {
            return get(predicate, earliestTime, latestTime, useDefaultValue, timeStep);
        }

        public void init(int? initialTimeStep)
        {
            if (initialTimeStep != null)
            {
                currentTimeStep = (int)initialTimeStep;
            }
        }
    }
}
