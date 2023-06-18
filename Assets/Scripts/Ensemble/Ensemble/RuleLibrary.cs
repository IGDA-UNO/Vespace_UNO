using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unity;
using UnityEngine;

namespace Ensemble
{
    internal class RuleLibrary
    {
        private Dictionary<string, List<Rule>> ruleLibrary;
        private Dictionary<string, Dictionary<string, int>> ruleIndexes;
        private SocialRecord socialRecord;
        private VolitionCache volitionCache;

        public RuleLibrary(SocialRecord socialRecord, VolitionCache volitionCache)
        {
            this.socialRecord = socialRecord;
            this.volitionCache = volitionCache;

            ruleLibrary = new Dictionary<string, List<Rule>>();
            ruleLibrary["triggerRules"] = new List<Rule>();
            ruleLibrary["volitionRules"] = new List<Rule>();
            ruleIndexes = new Dictionary<string, Dictionary<string, int>>();
        }

        public Binding getUniqueBindings(List<Predicate> ruleConditions)
        {
            Binding dictionary = new Binding();
            for (int i = 0; i < ruleConditions.Count; i++)
            {
                Predicate predicate = ruleConditions[i];

                if (!dictionary.ContainsKey(predicate.First) || dictionary[predicate.First] == null)
                {
                    dictionary[predicate.First] = "";
                }

                if (predicate.Second != null)
                {
                    if (!dictionary.ContainsKey(predicate.Second) || dictionary[predicate.Second] == null)
                    {
                        dictionary[predicate.Second] = "";
                    }
                }
            }
            return dictionary;
        }

        public void matchUniqueBindings(Binding uniqueBindings, Cast availableCastMembers, string type, Rule rule, int? timeStep, Cast unaffectedCharacters, VolitionSet calculatedVolitions)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            bool isFilled = true;
            string emptyKey = "";

            foreach (KeyValuePair<string, string> binding in uniqueBindings)
            {
                if (binding.Value == "")
                {
                    emptyKey = binding.Key;
                    isFilled = false;
                    break;
                }
            }

            if (isFilled == true)
            {
                List<Predicate> conditionClones = rule.Conditions.Select(condition => (Predicate)condition.DeepCopy()).ToList();
                List<Condition> boundConditions = doBinding(uniqueBindings, conditionClones).Cast<Condition>().ToList();
                List<Predicate> effectClones = rule.Effects.Select(effect => (Predicate)effect.DeepCopy()).ToList();
                List<Effect> boundEffects = doBinding(uniqueBindings, effectClones).Cast<Effect>().ToList();
                bool atLeastOneGoodEffect = false;

                for (int k = 0; k < boundEffects.Count; k++)
                {
                    if (boundEffects[k].First != null)
                    {
                        if (unaffectedCharacters.Contains(boundEffects[k].First))
                            continue;
                    }
                    if (boundEffects[k].Second != null)
                    {
                        if (unaffectedCharacters.Contains(boundEffects[k].Second))
                            continue;
                    }

                    atLeastOneGoodEffect = true;
                    break;
                }

                var conditionsAreTrue = false;
                if (atLeastOneGoodEffect)
                    conditionsAreTrue = evaluateConditions(boundConditions, timeStep);

                // All the conditions are true, so process all effects.
                if (conditionsAreTrue == true)
                {
                    // ensemble.js has the following duplicate line that seems unecessary
                    //List<Predicate> boundEffects = doBinding(uniqueBindings, rule.Effects.Cast<Predicate>().ToList()).Cast<Effect>().ToList();
                    for (int j = 0; j < boundEffects.Count; j++)
                    {
                        if (type == "trigger")
                            processRuleEffects(boundEffects[j], boundConditions, rule, j, boundEffects.Count - 1, unaffectedCharacters, availableCastMembers, timeStep);
                        else if (type == "volition")
                            adjustWeight(boundEffects[j], boundConditions, rule, j, boundEffects.Count - 1, unaffectedCharacters, availableCastMembers, timeStep, calculatedVolitions);
                    }
                }
            } else
            {
                for (var i = 0; i < availableCastMembers.Count; i += 1)
                {
                    //UnityEngine.Debug.Log("inside match unique bindings...");
                    uniqueBindings[emptyKey] = availableCastMembers[i];
                    Cast updatedCastMembers = availableCastMembers.DeepCopy();
                    updatedCastMembers.RemoveAt(i);
                    matchUniqueBindings(uniqueBindings, updatedCastMembers, type, rule, timeStep, unaffectedCharacters, calculatedVolitions);
                }
                uniqueBindings[emptyKey] = "";
            }
        }

        public bool evaluateConditions(List<Condition> conditions)
        {
            return evaluateConditions(conditions, null);
        }

        public bool evaluateConditions(List<Condition> conditions, int? timeStep)
        {
            List<Condition> sortedConditions = sortConditionsByOrder(conditions);
            int orderCounter = -9999;
            int timeStart = 0;
            int timeEnd;

            if (timeStep != null) {
                timeEnd = (int)timeStep;
            } else
            {
                timeEnd = socialRecord.getCurrentTimeStep();
            }

            for (int i = 0; i < sortedConditions.Count; i++)
            {
                Condition condition = sortedConditions[i];
                List<Predicate> results = new List<Predicate>();

                string smallerRelTimeStr = "0";
                string largerRelTimeStr = "0";
                int timeOfLastMatch = -1;
                int currentTimeStep = timeEnd;

                // TODO: TurnsAgoBetween can't mix strings and ints...
                // we need to figure out how to represent "now" and "start
                if (condition.TurnsAgoBetween != null)
                {
                    smallerRelTimeStr = condition.TurnsAgoBetween[0];
                    if (smallerRelTimeStr.ToLower() == "now")
                    {
                        smallerRelTimeStr = "0";
                    }
                    else if (smallerRelTimeStr.ToLower() == "start")
                    {
                        smallerRelTimeStr = currentTimeStep.ToString();
                    }

                    largerRelTimeStr = condition.TurnsAgoBetween[1];
                    if (largerRelTimeStr.ToLower() == "now")
                    {
                        largerRelTimeStr = "0";
                    }
                    else if (largerRelTimeStr.ToLower() == "start")
                    {
                        largerRelTimeStr = currentTimeStep.ToString();
                    }
                }

                int smallerRelTime;
                int largerRelTime;

                bool smallerRelTimeParsed = Int32.TryParse(smallerRelTimeStr, out smallerRelTime);
                bool largerRelTimeParsed = Int32.TryParse(largerRelTimeStr, out largerRelTime);

                if (!smallerRelTimeParsed)
                {
                    Console.WriteLine("Error Parsing condition.TurnsAgoBetween[0]: " + condition.TurnsAgoBetween[0]);
                    return false;
                }

                if (!largerRelTimeParsed)
                {
                    Console.WriteLine("Error Parsing condition.TurnsAgoBetween[1]: " + condition.TurnsAgoBetween[1]);
                    return false;
                }

                if (smallerRelTime > largerRelTime)
                    throw new Exception("found smallerRelTime " + smallerRelTime + " and largerRelTime " + largerRelTime);

                if (condition.Value == null && socialRecord.getRegisteredIsBoolean(condition) == true)
                    condition.Value = 1;

                Condition searchCondition = (Condition)condition.DeepCopy();
                searchCondition.TurnsAgoBetween = null;

                if (condition.Order == null)
                {
                    results = socialRecord.get(condition, smallerRelTime, largerRelTime, true, timeStep);
                    if (results.Count == 0)
                        return false;
                } else
                {
                    if (condition.Order > orderCounter)
                    {
                        timeStart = currentTimeStep - (timeOfLastMatch + 1);
                        timeEnd = smallerRelTime;
                        orderCounter = (int)condition.Order;
                    }

                    if (largerRelTime < timeStart)
                        timeStart = largerRelTime;

                    if (smallerRelTime > timeEnd)
                        timeEnd = smallerRelTime;

                    if (timeEnd > timeStart)
                        return false;

                    results = socialRecord.get(condition, timeEnd, timeStart);

                    if (results.Count == 0)
                        return false;

                    if (results[0].TimeHappened > timeOfLastMatch)
                        timeOfLastMatch = results[0].TimeHappened;
                }
            }

            return true;
        }

        public List<Condition> sortConditionsByOrder(List<Condition> conditions)
        {
            List<Condition> nonOrderConditions = new List<Condition>();
            List<Condition> orderConditions = new List<Condition>();
            List<Condition> sortedConditions = new List<Condition>();

            for (int i = 0; i < conditions.Count; i++)
            {
                if (conditions[i].Order == null)
                    nonOrderConditions.Add(conditions[i]);
                else
                    orderConditions.Add(conditions[i]);
            }

            orderConditions = orderConditions.OrderBy(o => o.Order).ToList();

            for (int i = 0; i < nonOrderConditions.Count; i++)
                sortedConditions.Add(nonOrderConditions[i]);
            for (int i = 0; i < orderConditions.Count; i++)
                sortedConditions.Add(orderConditions[i]);

            return sortedConditions;
        }

        public List<Predicate> doBinding(Binding characters, List<Predicate> predicates)
        {
            List<Predicate> resultsArray = new List<Predicate>();

            for (int i = 0; i < predicates.Count; i++)
            {
                Predicate predicate = predicates[i].DeepCopy();
                predicate.First = characters[predicate.First];
               
                if (predicate.Second != null)
                    predicate.Second = characters[predicate.Second];

                resultsArray.Add(predicate);
            }

            return resultsArray;
        }

        public void runRules(String ruleSet, Cast cast, string type, int? timeStep, Cast unaffectedCharacters, VolitionSet calculatedVolitions)
        {
            List<Rule> rules;

            if (ruleSet == "triggerRules")
                rules = ruleLibrary["triggerRules"];
            else
                rules = ruleLibrary["volitionRules"];

            if (rules == null)
                return;

            UnityEngine.Debug.Log("Rule Count: " + rules.Count);
            

            for (int i = 0; i < rules.Count; i++)
            {
                if (rules[i].Conditions == null)
                    // throw error
                if (rules[i].IsActive == false)
                    continue;

                List<Predicate> allPredicates = rules[i].Conditions.Cast<Predicate>().Concat(rules[i].Effects.Cast<Predicate>()).ToList();
                Binding uniqueBindings = getUniqueBindings(allPredicates);
                matchUniqueBindings(uniqueBindings, cast, type, rules[i], timeStep, unaffectedCharacters, calculatedVolitions);
            }
        }

        public void processRuleEffects(Effect effect, List<Condition> conditions, Rule rule, int effectNumber, int lastNumber, Cast unaffectedCharacters, Cast cast, int? timeStep) {
            TriggerRule triggerObj = new TriggerRule();
            triggerObj.Effects = new List<Effect>();
            triggerObj.Explanations = new List<string>();
            triggerObj.InCharMsgs = new List<string>();

            string explanation = "";

            // Debug.WriteLine("rule: " + rule.ToString());

			if (effectNumber == 0) {
				explanation = "TRIGGER RULE: Because ";
				for (int i = 0; i < conditions.Count; i++) {
                    // Debug.WriteLine("conditions[i]: " + conditions[i].ToString());
                    explanation += i + ") " + predicateToEnglish(conditions[i]).Text + (i+1 == conditions.Count? "":" ");
				}
                explanation += ", now ";
			} else {
				explanation += " and ";
			}
			explanation += predicateToEnglish(effect).Text;
			if(isEffectValid(effect, unaffectedCharacters)){
				socialRecord.set(effect);
				triggerObj.Effects.Add(effect);
			}
			if (effectNumber == lastNumber) {
				triggerObj.InCharMsgs.Add(rule.Msg != null ? rule.Msg : "I feel different...");
				triggerObj.Explanations.Add(explanation);
			}

            runRules("triggerRules", cast, "trigger", timeStep, unaffectedCharacters, null);
        }

        public void runTriggerRules(Cast cast)
        {
            Cast unaffectedCharacters = socialRecord.getEliminatedCharacters().DeepCopy();
            runRules("triggerRules", cast, "trigger", null, unaffectedCharacters, null);
        }

        public void runTriggerRules(Cast cast, int? timeStep)
        {
            Cast unaffectedCharacters = socialRecord.getEliminatedCharacters().DeepCopy();
            runRules("triggerRules", cast, "trigger", timeStep, unaffectedCharacters, null);
        }

        public bool isEffectValid(Effect effect, List<string> charactersToIgnore)
        {
            for (int i = 0; i < charactersToIgnore.Count; i += 1)
            {
                if (effect.First != null)
                {
                    if (effect.First == charactersToIgnore[i])
                    {
                        return false;
                    }
                }
                if (effect.Second != null)
                {
                    if (effect.Second == charactersToIgnore[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void adjustWeight(Effect effect, List<Condition> conditions, Rule rule, int effectNumber, int lastNumber, List<string> unaffectedCharacters, List<string> cast, int? timeStep, VolitionSet calculatedVolitions)
        {
            Predicate result = effect.DeepCopy();
            bool skipToNextPredicate = false;
            result.Weight = null;

            if (!isEffectValid(effect, unaffectedCharacters))
            {
                return;
            }
            
            string direction = socialRecord.getRegisteredDirection(effect);

            if (effect.Second == null)
            {
                if (direction == "undirected")
                {
                    effect.Second = effect.First;
                }
            }

            List<Predicate> volitionsForCharacterSet = calculatedVolitions.getVolitionsForCharacterSet(effect.First, effect.Second);
            int lengthOfPairsEffectsArray = volitionsForCharacterSet.Count;

            for (int i = 0; i <= lengthOfPairsEffectsArray; i++)
            {
                if (i == lengthOfPairsEffectsArray)
                {
                    result.Weight = 0;

                    if (effect.Weight != null)
                    {
                        result.Weight += (int)effect.Weight;
                    }
                    
                    result.EnglishInfluences = new List<EnglishData>();

                    EnglishData englishData = new EnglishData();
                    String englishInfluence = predicateArrayToEnglish(conditions.Cast<Predicate>().ToList());
                    englishData.EnglishRule = englishInfluence;
                    englishData.RuleName = rule.Name;
                    englishData.Weight = (int)result.Weight;
                    englishData.Origin = rule.Origin;
                    result.EnglishInfluences.Add(englishData);

                    calculatedVolitions.addVolitionForCharacterSet(effect.First, effect.Second, result);
                    break;
                }

                Predicate currentVolition = volitionsForCharacterSet[i];

                if (!result.ExistingPredicate(currentVolition))
                {
                    skipToNextPredicate = true;
                }

                //foreach (PropertyInfo property in currentVolition.GetType().GetProperties())
                //{
                //    if (property.Name == "englishInfluences")
                //    {
                //        continue;
                //    }
                //    if (result.GetType().GetProperty(property.Name).GetValue(result) != currentVolition.GetType().GetProperty(property.Name).GetValue(currentVolition))
                //    {
                //        skipToNextPredicate = true;
                //        break;
                //    }
                //}

                if (skipToNextPredicate == true)
                {
                    skipToNextPredicate = false;
                    continue;
                } else
                {
                    currentVolition.Weight += (int)effect.Weight;

                    EnglishData englishData = new EnglishData();
                    string englishInfluence = predicateArrayToEnglish(conditions.Cast<Predicate>().ToList());
                    englishData.EnglishRule = englishInfluence;
                    englishData.RuleName = rule.Name;
                    englishData.Weight = (int)effect.Weight;
                    englishData.Origin = rule.Origin;

                    currentVolition.EnglishInfluences.Add(englishData);

                    //Ok! And now let's actually sort the english influences based on the weight
                    //so most important comes first!
                    currentVolition.EnglishInfluences = currentVolition.EnglishInfluences.OrderBy(o => o.Weight).ToList();
                    break;  // there will only be one unique effect to update, so we are done; break out of the loop
                }
            }
        }

        public VolitionInterface calculateVolition(Cast cast)
        {
            return calculateVolition(cast, null);
        }

        public VolitionInterface calculateVolition(Cast cast, int? timeStep)
        {
            VolitionSet calculatedVolitions = volitionCache.newSet(cast);

            Cast charactersToSkipVolitionCalculation = new Cast();
            Cast offstageCharacters = socialRecord.getOffstageCharacters();
            Cast eliminatedCharacters = socialRecord.getEliminatedCharacters();

            for (int i = 0; i < offstageCharacters.Count; i++)
            {
                if (!charactersToSkipVolitionCalculation.Contains(offstageCharacters[i]))
                {
                    charactersToSkipVolitionCalculation.Add(offstageCharacters[i]);
                }
            }

            for (int i = 0; i < eliminatedCharacters.Count; i++)
            {
                if (!charactersToSkipVolitionCalculation.Contains(eliminatedCharacters[i]))
                {
                    charactersToSkipVolitionCalculation.Add(eliminatedCharacters[i]);
                }
            }

            UnityEngine.Debug.Log("Value of cast: " + cast.ToString() + " value of cast to skip: " + charactersToSkipVolitionCalculation.ToString());

            runRules("volitionRules", cast, "volition", timeStep, charactersToSkipVolitionCalculation, calculatedVolitions);
            return volitionCache.register("main", calculatedVolitions);
        }

        public void addRuleSet(string key, List<Rule> set)
        {
            if (!ruleIndexes.ContainsKey(key))
            {
                ruleIndexes[key] = new Dictionary<string, int>();
            }

            for (int i = 0; i < set.Count; i++)
            {
                if (isRuleAlreadyInRuleSet(key, set[i]))
                {
                    // log error
                }
                addRule(key, set[i]);

                Rule rule = set[i];
                if (rule.ID != null)
                {
                    int lastPos = ruleLibrary[key].Count - 1;
                    ruleIndexes[key][rule.ID] = lastPos;
                }
            }
        }

        public void addRule(string key, Rule rule)
        {
            if (!ruleLibrary.ContainsKey(key) || ruleLibrary[key] == null)
            {
                ruleLibrary[key] = new List<Rule>();
            }
            ruleLibrary[key].Add(rule);
        }

        public bool isRuleAlreadyInRuleSet(string key, Rule rule)
        {
            Rule storedRule;
            if (ruleLibrary.ContainsKey(key))
            {
                for (int i = 0; i < ruleLibrary[key].Count; i++)
                {
                    storedRule = ruleLibrary[key][i];
                    if (areRulesEqual(storedRule, rule))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /* 
         * Generic object property comparison method via:
         * https://stackoverflow.com/questions/506096/comparing-object-properties-in-c-sharp
         * 
         */
        public bool PublicInstancePropertiesEqual<T>(T self, T to, params string[] ignore) where T : class
        {
            if (self != null && to != null)
            {
                Type type = typeof(T);
                List<string> ignoreList = new List<string>(ignore);
                foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!ignoreList.Contains(pi.Name))
                    {
                        object selfValue = type.GetProperty(pi.Name).GetValue(self, null);
                        object toValue = type.GetProperty(pi.Name).GetValue(to, null);

                        if (selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue)))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return self == to;
        }

        public bool arePredicatesEqual(Predicate pred1, Predicate pred2)
        {
            return pred1.Equals(pred2);
        }

        public bool areRulesEqual(Rule rule1, Rule rule2)
        {
            if (rule1.Conditions == null && rule2.Conditions == null && rule1.Effects == null && rule2.Effects == null)
                return true;

            if ((rule1.Conditions != null && rule2.Conditions == null) || (rule2.Conditions != null && rule1.Conditions == null))
                return false;

            if ((rule1.Effects != null && rule2.Effects == null) || (rule2.Effects != null && rule1.Effects == null))
                return false;

            if (rule1.Conditions != null && rule2.Conditions != null)
            {
                if (rule1.Conditions.Count != rule2.Conditions.Count)
                    return false;

                for (int i = 0; i < rule1.Conditions.Count; i++)
                {
                    if (!this.arePredicatesEqual(rule1.Conditions[i], rule2.Conditions[i]))
                    {
                        return false;
                    }
                }
            }

            if (rule1.Effects != null && rule2.Effects != null)
            {
                if (rule1.Effects.Count != rule2.Effects.Count)
                    return false;

                for (int j = 0; j < rule1.Effects.Count; j++)
                {
                    if (!this.arePredicatesEqual(rule1.Effects[j], rule2.Effects[j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public string ruleToEnglish(Rule rule)
        {
            string returnString = "If: ";
            returnString += predicateArrayToEnglish(rule.Conditions.Cast<Predicate>().ToList());
            returnString += ", Then: ";
            returnString += predicateArrayToEnglish(rule.Effects.Cast<Predicate>().ToList());
            return returnString;
        }

        public string predicateArrayToEnglish(List<Predicate> conditions)
        {
            string returnString = "";
            for (int i = 0; i < conditions.Count; i++)
            {
                Predicate pred = (Predicate)conditions[i];
                if (i >= 1)
                {
                    returnString += ", and ";
                }
                returnString += predicateToEnglish(pred).Text;
            }
            return returnString;
        }

        public Phrase addPhrase(string text)
        {
            Phrase ph = new Phrase();
            ph.Text = text;
            ph.Label = "";
            return ph;
        }

        public Phrase addPhrase(string text, string label) {
            Phrase ph = new Phrase();
            ph.Text = text;
			ph.Label = label != null ? label : "";
            return ph;
		}

        public Phrase addPhrase(string text, string label, string opt)
        {
            Phrase ph = new Phrase();
            ph.Text = text;
            ph.Label = label != null ? label : "";
            ph.Meta = opt;
            return ph;
        }

        public Phrase predicateToEnglish(Predicate pred) {
		    List<Phrase> result = new List<Phrase>();

	        if (pred.Name != null || pred.First == null) {
		        // For complicated predicates, for now just return the human-authored rule name.
		        Phrase o = new Phrase();
		        o.Text = pred.Name;
		        return o;
	        }

            if (pred.Value == null)
            {
                pred.Value = true;
            }

		    bool? isBoolean = socialRecord.getRegisteredIsBoolean(pred);
		    string directionType = socialRecord.getRegisteredDirection(pred);
		    int? duration = socialRecord.getRegisteredDuration(pred);
		    bool isPersistent = (duration != 0) ? true : false;

		    string nameFirst = pred.First;
		    string nameSecond = pred.Second != null ? pred.Second : "";

		    string predType = "fact";
		    if (pred.Operator != null && (pred.Operator == "+" || pred.Operator == "-")) {
			    predType = "change";
		    }
            if (pred.Operator != null && (pred.Operator == ">" || pred.Operator == "<" || pred.Operator == "="))
            {
                predType = "compare";
            }
		    if (pred.Weight != null) {
                predType = "volition";
		    }

		    string isWord = "is";
		    string hasWord = "has";
            string moreRecent = null;
            string lessRecent = null;

		    if (pred.TurnsAgoBetween != null) {
			    moreRecent = pred.TurnsAgoBetween[0];
			    lessRecent = pred.TurnsAgoBetween[1];
			    if (moreRecent == "NOW") moreRecent = "0";
			    if (lessRecent == "NOW") lessRecent = "0";
			    if (moreRecent == "START") moreRecent = "Infinity";
			    if (lessRecent == "START") lessRecent = "Infinity";
			    if (moreRecent == "0" && lessRecent == "0") {
				    // Leave as is; skip further custom text.
				    moreRecent = null;
				    lessRecent = null;
			    }
			    else if (moreRecent == "0") {
				    isWord = "has been";
				    hasWord = "has had";
			    } else {
				    isWord = "was";
				    hasWord = "had";
			    }
		    }

            //string notWord = pred.Value == 0 ? " not" : "";

            string notWord;
            if (pred.Value is Boolean)
            {
                notWord = pred.Value == false ? " not" : "";
            }
            else
            {
                notWord = pred.Value == 0 ? " not" : "";
            }

            string directionWord;

		    switch(pred.Operator) {
			    case "+": directionWord = "more"; break;
			    case ">": directionWord = "more than"; break;
			    case "-": directionWord = "less"; break;
			    case "<": directionWord = "less than"; break;
			    default: directionWord = "exactly"; break;
		    }

		    if (pred.Weight != null) {
			    if (pred.Weight == 0) directionWord = "unchanged";
			    else directionWord = pred.Weight > 0 ? "more" : "less";
		    }

            if (predType == "fact" || predType == "compare") {
			    result.Add(addPhrase(nameFirst, "first"));
			    if (!isPersistent) {
				    var didWord = ((isBoolean == true && pred.Value == 1) || pred.Value == null ? "did" : "did not do");
				    result.Add(addPhrase(didWord, "beVerb"));
				    result.Add(addPhrase("something"));
			    }
			    else if (isBoolean == true) {
				    result.Add(addPhrase(isWord+notWord, "beVerb"));
			    } else {
				    result.Add(addPhrase(hasWord));
				    result.Add(addPhrase(directionWord, "direction"));
				    result.Add(addPhrase(pred.Value.ToString(), "value"));
			    }
		    }
		    if (predType == "change") {
			    result.Add(addPhrase(nameFirst, "first"));
			    result.Add(addPhrase(hasWord));
			    result.Add(addPhrase(pred.Value.ToString(), "value"));
			    result.Add(addPhrase(directionWord, "direction"));
		    }
		    if (predType == "volition") {
			    var intentWord = pred.IntentType == true ? "become" : "stop being";
			    if (!isBoolean == true) {
				    intentWord = pred.IntentType == true ? "increase" : "decrease";
			    }
			    result.Add(addPhrase(nameFirst, "first"));
			    result.Add(addPhrase(hasWord, "beVerb"));
			    result.Add(addPhrase(directionWord, ".."));
			    result.Add(addPhrase("volition"));
			    var sign = pred.Weight >= 0 ? "+" : "";
			    result.Add(addPhrase("("));
			    result.Add(addPhrase(sign+pred.Weight, "weight"));
			    result.Add(addPhrase(")"));
			    result.Add(addPhrase("to"));
			    result.Add(addPhrase(intentWord, "intentType"));
		    }

		    result.Add(addPhrase(pred.Type, "type", pred.Category));

		    if (directionType != "undirected") {
			    var helper = "";
			    if (!isPersistent) {
				    helper = "to";
			    } else if (!isBoolean == true) {
				    helper = "for";
			    }
			    result.Add(addPhrase(helper));
			    result.Add(addPhrase(nameSecond, "second"));
		    }

		    // Explanation of past tense parameters.
		    if (moreRecent != null) {
			    result.Add(addPhrase("", "timeOrderStart"));
			    string printedMoreRecent = pred.TurnsAgoBetween[0];
			    if (printedMoreRecent == "NOW") {
				    printedMoreRecent = "0";
			    }
			    string printedLessRecent = pred.TurnsAgoBetween[1];
			    if (printedLessRecent == "NOW") {
				    printedLessRecent = "0";
			    }
			    if (lessRecent == "Infinity") {
				    if (moreRecent == "0") {
					    result.Add(addPhrase("at any point"));
				    } else if (moreRecent == "Infinity") {
					    result.Add(addPhrase("at the very beginning"));
				    } else {
					    result.Add(addPhrase("sometime up until"));
					    result.Add(addPhrase(printedMoreRecent));
					    result.Add(addPhrase("turns ago"));
				    }
				    result.Add(addPhrase("["));
				    result.Add(addPhrase(printedMoreRecent, "moreRecent"));
				    result.Add(addPhrase(","));
				    result.Add(addPhrase(printedLessRecent, "lessRecent"));
				    result.Add(addPhrase("]"));
			    } else {
				    result.Add(addPhrase("sometime between"));
				    result.Add(addPhrase(printedMoreRecent, "moreRecent"));
				    result.Add(addPhrase("and"));
				    result.Add(addPhrase(printedLessRecent, "lessRecent"));
				    result.Add(addPhrase("turns ago"));
			    }
			    result.Add(addPhrase("", "timeOrderEnd"));
		    }

		    // Assemble the result object. Generate the single string of text by turning our array of objects into an array of texts, then filtering any empty texts from the array, then putting a space between each element to make a string.
		    Phrase resultObj = new Phrase();
		    resultObj.Diagram = result;
            resultObj.Text = string.Join(" ", result.Where(ph => ph.Text != "").Select(ph => ph.Text));

            return resultObj;
	    }

        public List<Rule> getTriggerRules()
        {
            if (ruleLibrary["triggerRules"] != null)
            {
                return ruleLibrary["triggerRules"].Select(rule => (Rule)rule.DeepCopy()).ToList();
            } else
            {
                return new List<Rule>();
            }
        }

        public List<Rule> getVolitionRules()
        {
            if (ruleLibrary["volitionRules"] != null)
            {
                List<Rule> rules = ruleLibrary["volitionRules"].Select(rule => (Rule)rule.DeepCopy()).ToList();

                //foreach(Rule rule in rules)
                //{
                //    Debug.WriteLine(Convert.ToString((object)rule.Effects[0].IntentType));
                //}

                return rules;
            }
            else
            {
                return new List<Rule>();
            }
        }

        public Rule getRuleById(string label)
        {
            List<string> labelParts = label.Split('_').ToList();
            string ruleSet = labelParts[0];
            string id = labelParts[1];

            try
            {
                List<Rule> r1 = ruleLibrary[ruleSet];

                if (r1 == null)
                {
                    // log error
                    return null;
                }

                int pos = ruleIndexes[ruleSet][label];
                return ruleLibrary[ruleSet][pos].DeepCopy();
            } catch (Exception e)
            {
                return null;
            }
        }

        public bool setRuleById(string label, Rule rule)
        {
            List<string> labelParts = label.Split('_').ToList();
            string ruleSet = labelParts[0];
            string id = labelParts[1];

            List<Rule> r1 = ruleLibrary[ruleSet];
            if (r1 == null)
            {
                // log error
                return false;
            }
            int? pos = ruleIndexes[ruleSet][label];
            if (pos == null)
            {
                // log error
                return false;
            }

            ruleLibrary[ruleSet][(int)pos] = rule.DeepCopy();
            return true;
        }

        public bool deleteRuleById(string label)
        {
            if (label == null) return false;

            List<string> labelParts = label.Split('_').ToList();
            string ruleSet = labelParts[0];
            string id = labelParts[1];

            List<Rule> r1 = ruleLibrary[ruleSet];
            if (r1 == null)
            {
                // log error
                return false;
            }
            int? pos = ruleIndexes[ruleSet][label];
            if (pos == null)
            {
                // log error
                return false;
            }

            List<Rule> lib = ruleLibrary[ruleSet];
            Dictionary<string, int> ind = ruleIndexes[ruleSet];
            int posOfDyingRule = ind[label];
            int posOfFinalRule = lib.Count - 1;
            string finalRuleId = lib[posOfFinalRule].ID;

            lib[posOfDyingRule] = lib[posOfFinalRule];
            // TODO: ensemble.js has lib.length = lib.length - 1... can we just remove the last rule instead?
            lib.RemoveAt(lib.Count - 1);

            ind.Remove(label);
            ind[finalRuleId] = posOfDyingRule;

            return true;
        }

        public void clearRuleLibrary()
        {
            ruleLibrary = new Dictionary<string, List<Rule>>();
            ruleIndexes = new Dictionary<string, Dictionary<string, int>>();
        }

        public VolitionCache GetVolitionCache(){
            return this.volitionCache;
        }

        public SocialRecord GetSocialRecord(){
            return this.socialRecord;
        }
    }
}
 