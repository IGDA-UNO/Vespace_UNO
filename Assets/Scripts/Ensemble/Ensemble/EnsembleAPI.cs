using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("EnsembleTest")]
namespace Ensemble
{
    public class EnsembleAPI
    {
        private ActionLibrary actionLibrary;
        private Dictionary<string, Dictionary<string, string>> savedChars;
        private RuleLibrary ruleLibrary;
        private SocialRecord socialRecord;
        private Util util;
        private Validate validate;
        private VolitionCache volitionCache;

        // A dictionary with top level keys will be each of the social "categories" (a la "relationship", "network", etc.). Each of these contains a dictionary of its subtypes.
        private SocialStructure socialStructure;

        public EnsembleAPI()
        {
            util = new Util();
            socialRecord = new SocialRecord(util);
            validate = new Validate();
            socialStructure = new SocialStructure();
            volitionCache = new VolitionCache();
            ruleLibrary = new RuleLibrary(socialRecord, volitionCache);
            actionLibrary = new ActionLibrary(ruleLibrary, volitionCache, util);
        }

        internal ActionLibrary getActionLibrary()
        {
            return this.actionLibrary;
        }

        internal RuleLibrary getRuleLibrary()
        {
            return this.ruleLibrary;
        }

        internal SocialRecord getSRecord()
        {
            return this.socialRecord;
        }

        internal Util getUtil()
        {
            return this.util;
        }

        internal Validate getValidate()
        {
            return this.validate;
        }

        internal VolitionCache getVolitionCache()
        {
            return this.volitionCache;
        }

        public Dictionary<string, Dictionary<string, Predicate>> loadBaseBlueprints(List<Predicate> bp)
        {
            socialRecord.clearEverything();
            return loadSocialStructure(bp);
        }

        public string readFile(string filename)
        {
            using (StreamReader r = new StreamReader(filename))
            {
                return r.ReadToEnd();
            }
        }

        public List<Action> loadActionsFile(string filename)
        {
            string json = readFile(filename);
            ActionsFile actionsFile = JsonConvert.DeserializeObject<ActionsFile>(json);
            return actionsFile.actions;
        }

        public Dictionary<string, Dictionary<string, string>> loadCastFile(string filename)
        {
            string json = readFile(filename);
            CastFile castFile = JsonConvert.DeserializeObject<CastFile>(json);
            return castFile.cast;
        }

        public List<History> loadHistoryFile(string filename)
        {
            string json = readFile(filename);
            HistoryFile historyFile = JsonConvert.DeserializeObject<HistoryFile>(json);
            return historyFile.history;
        }

        public List<Predicate> loadSchemaFile(string filename)
        {
            string json = readFile(filename);
            SchemaFile schemaFile = JsonConvert.DeserializeObject<SchemaFile>(json);
            return schemaFile.schema;
        }

        public List<Rule> loadTriggerRulesFile(string filename)
        {
            string json = readFile(filename);
            TriggerRulesFile triggerRulesFile = JsonConvert.DeserializeObject<TriggerRulesFile>(json);
            return triggerRulesFile.rules;
        }

        public List<Rule> loadVolitionRulesFile(string filename)
        {
            string json = readFile(filename);
            VolitionRulesFile volitionRuleFile = JsonConvert.DeserializeObject<VolitionRulesFile>(json);
            return volitionRuleFile.rules;
        }

        public List<Rule> loadUserSpecifiedRulesFile(string filename)
        {
            string json = readFile(filename);
            UserRulesFile userRulesFile = JsonConvert.DeserializeObject<UserRulesFile>(json);
            return userRulesFile.rules;
        }

        public VolitionSet loadVolitionSetFile(string filename)
        {
            string json = readFile(filename);
            VolitionSetFile volitionSetFile = JsonConvert.DeserializeObject<VolitionSetFile>(json);
            VolitionSet volitionSet = new VolitionSet(volitionSetFile);
            return volitionSet;
        }

        private Predicate registerSocialType(Predicate blueprint)
        {
            Predicate factory = new Predicate();
            factory.Category = blueprint.Category;
            factory.Type = blueprint.Type;
            factory.DirectionType = blueprint.DirectionType;
            factory.IsBoolean = blueprint.IsBoolean;
            factory.CloneEachTimeStep = blueprint.CloneEachTimeStep;
            factory.Duration = blueprint.Duration;
            factory.MinValue = blueprint.MinValue;
            factory.MaxValue = blueprint.MaxValue;
            factory.DefaultValue = blueprint.DefaultValue;
            factory.Actionable = blueprint.Actionable;
            return factory;
        }

        public SocialStructure loadSocialStructure(List<Predicate> schema)
        {
            bool atLeastOneCategoryAllowsIntent = false;
            for (int i = 0; i < schema.Count; i++)
            {
                if (schema[i].Actionable == true)
                {
                    atLeastOneCategoryAllowsIntent = true;
                    break;
                }
            }

            if (!atLeastOneCategoryAllowsIntent)
            {
                throw new Exception("SCHEMA ERROR: A schema must include at least one category where actionable is true, otherwise there are no possible actions for characters to take.");
            }

            for (int i = 0; i < schema.Count; i++)
            {
                loadBlueprint(schema[i], i);
            }

            validate.registerSocialStructure(socialStructure);
            return socialStructure;
        }

        private void loadBlueprint(Predicate categoryBlueprint, int num)
        {
            if (socialStructure.ContainsKey(categoryBlueprint.Category))
            {
                throw new Exception("DATA ERROR in ensemble.loadSocialStructure: the category '" + categoryBlueprint.Category + "' is defined more than once.");
            }

            validate.blueprint(categoryBlueprint, "Examining blueprint #" + num);

            socialRecord.registerDuration(categoryBlueprint);
            socialRecord.registerDefaultValue(categoryBlueprint);
            socialRecord.registerDirection(categoryBlueprint);
            socialRecord.registerIsBoolean(categoryBlueprint);
            socialRecord.registerMaxValue(categoryBlueprint);
            socialRecord.registerMinValue(categoryBlueprint);

            socialStructure[categoryBlueprint.Category] = new Dictionary<string, Predicate>();
            for (int j = 0; j < categoryBlueprint.Types.Count; j++)
            {
                string type = categoryBlueprint.Types[j];
                Predicate typeBlueprint = categoryBlueprint.DeepCopy();
                typeBlueprint.Type = type;
                socialStructure[categoryBlueprint.Category][type] = registerSocialType(typeBlueprint);
            }
        }

        private void updateCategory(string categoryKey, Predicate blueprint)
        {
            socialStructure.Remove(categoryKey);
            if (blueprint != null)
            {
                loadBlueprint(blueprint, 0);
            }
        }

        public Dictionary<string, Dictionary<string, Predicate>> getSocialStructure()
        {
            return socialStructure;
        }

        public List<Predicate> getSchema()
        {
            List<Predicate> schemaItems = new List<Predicate>();
            foreach (KeyValuePair<string, Dictionary<string, Predicate>> category in socialStructure)
            {
                if (category.Key == "schemaOrigin")
                {
                    continue;
                }

                Predicate item = new Predicate();
                item = getCategoryDescriptors(category.Key);
                item.Category = category.Key;
                item.Types = category.Value.Keys.ToList();

                schemaItems.Add(item);
            }
            return schemaItems;
        }

        public Predicate getCategoryDescriptors(string categoryName)
        {
            Predicate descriptors = new Predicate();
            Dictionary<string, Predicate> c = socialStructure[categoryName];

            if (c == null)
            {
                return null;
            }

            foreach (KeyValuePair<string, Predicate> type in c)
            {
                string typeName = type.Key;
                Predicate t = type.Value;
                string representativeType = c.Keys.ToList()[0];
                descriptors.Actionable = c[representativeType].Actionable;
                descriptors.DirectionType = t.DirectionType;
                descriptors.IsBoolean = t.IsBoolean;
                descriptors.CloneEachTimeStep = t.CloneEachTimeStep == null ? true : t.CloneEachTimeStep;
                descriptors.Duration = t.Duration;
                descriptors.MinValue = t.MinValue;
                descriptors.MaxValue = t.MaxValue;
                descriptors.DefaultValue = t.DefaultValue;
                return descriptors;
            }

            return null;
        }

        public string getCategoryFromType(string type)
        {
            foreach (KeyValuePair<string, Dictionary<string, Predicate>> category in socialStructure)
            {
                if (socialStructure[category.Key][type] != null)
                {
                    return category.Key;
                }
            }
            return null;
        }

        public bool isValidTypeForCategory(string type, string categoryName)
        {
            Dictionary<string, Predicate> category = socialStructure[categoryName];

            if (category == null)
            {
                return false;
            }

            if (category[type] == null)
            {
                return false;
            }

            return true;
        }

        public List<string> getSortedTurnsTuple(List<string> tab)
        {
            string t0Val = tab[0];
            string t1Val = tab[1];
            long t0ValNum;
            long t1ValNum;

            bool t0Parsed = long.TryParse(t0Val, out t0ValNum);
            bool t1Parsed = long.TryParse(t1Val, out t1ValNum);

            if (t0Val == "START")
            {
                t0ValNum = 9999999999;
            }

            if (t0Val == "NOW")
            {
                t0ValNum = 0;
            }

            if (t1Val == "START")
            {
                t1ValNum = 9999999999;
            }

            if (t1Val == "NOW")
            {
                t1ValNum = 0;
            }

            if (t0ValNum > t1ValNum)
            {
                string tmp = t0Val;
                tab[0] = tab[1];
                tab[1] = tmp;
            }

            return tab;
        }

        public List<Predicate> get(Predicate searchPredicate, int? mostRecentTime, int? lessRecentTime)
        {
            return getSocialRecord(searchPredicate, mostRecentTime, lessRecentTime);
        }

        public List<Predicate> get(Predicate searchPredicate, int? mostRecentTime)
        {
            return getSocialRecord(searchPredicate, mostRecentTime, null);
        }

        public List<Predicate> get(Predicate searchPredicate)
        {
            return getSocialRecord(searchPredicate, null, null);
        }

        public List<Predicate> getSocialRecord(Predicate searchPredicate, int? _mostRecentTime, int? _lessRecentTime)
        {
            // TODO: Make sure operator is not + or -

            // Ensure time window. Set to 0 if undefined.
            int mostRecentTime = _mostRecentTime == null ? 0 : (int)_mostRecentTime;
            int lessRecentTime = _lessRecentTime == null ? 0 : (int)_lessRecentTime;

            // TODO: I'm not sure how this worked always in Ensemble.js
            // since there, turnsAgoBetween could hold both ints and strings...
            // What would happen then if the value was a string?

            // Convert turnsAgoBetween to time window.
            if (searchPredicate.TurnsAgoBetween != null)
            {
                int turnsAgoBetween0;
                int turnsAgoBetween1;
                bool hasNumericTurnsAgoBetween0 = int.TryParse(searchPredicate.TurnsAgoBetween[0], out turnsAgoBetween0);
                bool hasNumericTurnsAgoBetween1 = int.TryParse(searchPredicate.TurnsAgoBetween[1], out turnsAgoBetween1);

                if (hasNumericTurnsAgoBetween0 && hasNumericTurnsAgoBetween1)
                {
                    mostRecentTime += turnsAgoBetween0;
                    lessRecentTime += turnsAgoBetween1;
                }
            }

            // Ensure proper time window ordering.
            if (mostRecentTime > lessRecentTime)
            {
                var tmp = mostRecentTime;
                mostRecentTime = lessRecentTime;
                lessRecentTime = tmp;
            }

            // Ensure socialRecord has been initialized.
            if (socialRecord.getCurrentTimeStep() == -1)
            {
                socialRecord.setupNextTimeStep(0);
            }

            return socialRecord.get(searchPredicate, mostRecentTime, lessRecentTime);
        }

        public List<string> addCharacters(Dictionary<string, Dictionary<string, string>> chars)
        {
            savedChars = chars;
            return getCharacters();
        }

        public List<string> getCharacters()
        {
            return savedChars.Keys.ToList();
        }

        public Dictionary<string, Dictionary<string, string>> getCharactersWithMetadata()
        {
            return savedChars;
        }

        public string getCharData(string id, string key)
        {
            if (savedChars.ContainsKey(id) && savedChars[id].ContainsKey(key))
            {
                return savedChars[id][key];
            } else
            {
                return null;
            }
        }


        public string getCharName(string id)
        {
            string name = null;
            if (savedChars.ContainsKey(id) && savedChars[id].ContainsKey("name"))
            {
                name = savedChars[id]["name"];
            }
            if (name == null)
            {
                return id;
            } else
            {
                return name;
            }
        }

        public List<string> addProcessedRules(string ruleType, string fileName, List<Rule> rules)
        {
            bool triggerOrVolition = ruleType == "trigger" || ruleType == "volition";
            List<string> ids = new List<string>();

            if (triggerOrVolition)
            {
                ruleType = ruleType + "Rules";
            }

            for (int i = 0; i < rules.Count; i++)
            {
                Rule rule = rules[i];
                if (rule.Name == null)
                {
                    // log warning
                }

                rule.Origin = fileName;
                string newId = ruleType + "_" + util.iterator("rules");
                ids.Add(newId);
                rule.ID = newId;

                if (triggerOrVolition)
                {
                    for (int j = 0; j < rule.Conditions.Count; j++)
                    {
                        Condition condRef = rule.Conditions[j];
                        condRef = (Condition)standardizePredicate(condRef);

                        if (ruleType == "trigger")
                            validate.triggerCondition(condRef, "Examining " + ruleType + " rule #" + i + ": '" + rule.Name + "' Validating condition at position " + j);
                        else
                            validate.volitionCondition(condRef, "Examining " + ruleType + " rule #" + i + ": '" + rule.Name + "' Validating condition at position " + j);
                    }

                    for (int j = 0; j < rule.Effects.Count; j++)
                    {
                        Effect effectRef = rule.Effects[j];
                        effectRef = (Effect)standardizePredicate(effectRef);

                        if (ruleType == "trigger")
                            validate.triggerEffect(effectRef, "Examining " + ruleType + " rule #" + i + ": '" + rule.Name + "' Validating effect at position " + j);
                        else
                            validate.volitionEffect(effectRef, "Examining " + ruleType + " rule #" + i + ": '" + rule.Name + "' Validating effect at position " + j);
                    }
                }
            }

            if (rules.Count > 0)
            {
                ruleLibrary.addRuleSet(ruleType, rules);
                return ids;
            }
            else
            {
                return new List<string>();
            }
        }

        public Predicate standardizePredicate(Predicate pred)
        {
            Predicate descriptors = getCategoryDescriptors(pred.Category);

            if (pred.IntentType is string)
            {
                if (pred.IntentType == "start")
                {
                    if (descriptors.IsBoolean != true)
                    {
                        throw new Exception("Invalid IntentType value");
                    }
                    else
                    {
                        pred.IntentType = true;
                    }
                }
                else if (pred.IntentType == "increase")
                {
                    if (descriptors.IsBoolean == true)
                    {
                        throw new Exception("Invalid IntentType value");
                    }
                    else
                    {
                        pred.IntentType = true;
                    }
                }
                else if (pred.IntentType == "stop")
                {
                    if (descriptors.IsBoolean != true)
                    {
                        throw new Exception("Invalid IntentType value");
                    }
                    else
                    {
                        pred.IntentType = false;
                    }
                }
                else if (pred.IntentType == "decrease")
                {
                    if (descriptors.IsBoolean == true)
                    {
                        throw new Exception("Invalid IntentType value");
                    }
                    else
                    {
                        pred.IntentType = false;
                    }
                }
                else
                {
                    throw new Exception("Incorrect intent type: " + pred.IntentType);
                }
            }

            return pred;
        }

        public List<string> addRules(string ruleType, string fileName, List<Rule> rules)
        {
            // method originally did JSON parsing/processing
            return addProcessedRules(ruleType, fileName, rules);
        }

        public List<Rule> getRules(string ruleSet)
        {
            if (ruleSet == "trigger")
            {
                return ruleLibrary.getTriggerRules();
            }
            if (ruleSet == "volition")
            {
                return ruleLibrary.getVolitionRules();
            }
            return new List<Rule>();
        }
    
        public List<Rule> filterRules(string ruleSet, Dictionary<string, string> criteria)
        {
            List<Rule> rules = getRules(ruleSet);
            List<string> predicateArrays = new List<string>() { "conditions", "effects" };
            SortedSet<Rule> matches = new SortedSet<Rule>();
            foreach (KeyValuePair<string, string> criterion in criteria)
            {
                matches.UnionWith(rules.Where(o => (string)o.GetType().GetProperty(criterion.Key).GetValue(o) == criterion.Value).ToList());
            }
            return matches.ToList();
        }

        public List<Action> filterActions(Dictionary<string, string> criteria)
        {
            List<Action> actions = actionLibrary.getAllActions();
            List<string> predicateArrays = new List<string>(){ "conditions", "effects", "influenceRules" };
            SortedSet<Action> matches = new SortedSet<Action>();
            foreach (KeyValuePair<string, string> criterion in criteria)
            {
                matches.UnionWith(actions.Where(o => (string)o.GetType().GetProperty(criterion.Key).GetValue(o) == criterion.Value).ToList());
            }
            return matches.ToList();
        }

        public bool setRuleById(string label, Rule rule)
        {
            Rule validatedRule = validate.rule(rule);
            if (validatedRule == null)
            {
                // throw error
                return false;
            }

            return ruleLibrary.setRuleById(label, rule);
        }

        public Rule getRuleById(string label)
        {
            return ruleLibrary.getRuleById(label);
        }

        public void deleteRuleById(string label)
        {
            ruleLibrary.deleteRuleById(label);
        }

        public void setPredicates(List<Predicate> predicateArray)
        {
            foreach(Predicate pred in predicateArray)
            {
                socialRecord.set(pred);
            }
        }

        public VolitionInterface calculateVolition(Cast cast)
        {
            return ruleLibrary.calculateVolition(cast);
        }

        public void calculateVolition(Cast cast, int timestep)
        {
            ruleLibrary.calculateVolition(cast, timestep);
        }

        public void runTriggerRules(Cast cast, int timestep)
        {
            ruleLibrary.runTriggerRules(cast, timestep);
        }

        public string ruleToEnglish(Rule rule)
        {
            return ruleLibrary.ruleToEnglish(rule);
        }

        public Phrase predicateToEnglish(Predicate pred)
        {
            return ruleLibrary.predicateToEnglish(pred);
        }

        public void dumpSocialRecord()
        {
            socialRecord.dumpSocialRecord();
        }

        public void dumpActionLibrary()
        {
            actionLibrary.dumpActions();
        }

        public void set(Predicate pred)
        {
            socialRecord.set(pred);
        }

        public int? getValue(string first, string second, string category, string type, int mostRecentTime, int lessRecentTime)
        {
            Predicate searchPredicate = new Predicate();
            searchPredicate.First = first;
            searchPredicate.Second = second;
            searchPredicate.Category = category;
            searchPredicate.Type = type;
            List<Predicate> returnArray = getSocialRecord(searchPredicate, mostRecentTime, lessRecentTime);
            Predicate firstResult = returnArray[0];
            int? value = firstResult.Value;
            return value;
        }

        // verify which checks still matter
        public List<Predicate> getSocialRecord(Predicate searchPredicate, int mostRecentTime, int lessRecentTime)
        {
            if (mostRecentTime > lessRecentTime)
            {
                int tmp = mostRecentTime;
                mostRecentTime = lessRecentTime;
                lessRecentTime = tmp;
            }

            return socialRecord.get(searchPredicate, mostRecentTime, lessRecentTime);
        }

        public void setCharacterOffstage(string characterName)
        {
            socialRecord.putCharacterOffstage(characterName);
        }

        public bool getIsCharacterOffstage(string characterName)
        {
            return socialRecord.getIsCharacterOffstage(characterName);
        }

        public void setCharacterOnstage(string characterName)
        {
            socialRecord.putCharacterOnstage(characterName);
        }

        public bool getIsCharacterOnstage(string characterName)
        {
            return !socialRecord.getIsCharacterOffstage(characterName);
        }

        public void setCharacterEliminated(string characterName)
        {
            socialRecord.eliminateCharacter(characterName);
        }

        public bool getIsCharacterEliminated(string characterName)
        {
            return socialRecord.getIsCharacterEliminated(characterName);
        }

        public void setupNextTimeStep()
        {
            socialRecord.setupNextTimeStep(null);
        }

        public void setupNextTimeStep(int timeStep)
        {
            socialRecord.setupNextTimeStep(timeStep);
        }

        public string getRegisteredDirection(Predicate pred)
        {
            return socialRecord.getRegisteredDirection(pred);
        }

        public Action getAction(string initiator, string responder, VolitionInterface volitionInterface, Cast cast)
        {
            return actionLibrary.getAction(initiator, responder, volitionInterface, cast, null);
        }

        public Action getAction(string initiator, string responder, VolitionInterface volitionInterface, Cast cast, int numActionsPerGroup)
        {
            return actionLibrary.getAction(initiator, responder, volitionInterface, cast, numActionsPerGroup);
        }

        public List<Action> getActions(string initiator, string responder, VolitionInterface volitionInterface, Cast cast, int numIntents, int numActionsPerIntent)
        {
            return actionLibrary.getActions(initiator, responder, volitionInterface, cast, numIntents, numActionsPerIntent, null);
        }

        public List<Action> getActions(string initiator, string responder, VolitionInterface volitionInterface, Cast cast, int numIntents, int numActionsPerIntent, int numActionsPerGroup)
        {
            return actionLibrary.getActions(initiator, responder, volitionInterface, cast, numIntents, numActionsPerIntent, numActionsPerGroup);
        }

        public Boolean takeAction(Action action)
        {
            if (action != null)
            {
                try
                {
                    foreach (Effect effect in action.Effects)
                    {
                        set(effect);
                    }

                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return false;
        }

        public List<Action> getAllActions()
        {
            return actionLibrary.getAllActions();
        }

        public void addActions(List<Action> inputActions, string fileName, string sourceFile)
        {
            actionLibrary.parseActions(inputActions, fileName, sourceFile);
        }

        public void addHistory(string sourceFile, List<History> history)
        {
            socialRecord.addHistory(sourceFile, history);
        }

        public void clearHistory()
        {
            socialRecord.clearHistory();
        }

        public List<Predicate> getSocialRecordCopyAtTimestep(int timestep)
        {
            return socialRecord.getSocialRecordCopyAtTimestep(timestep);
        }

        public List<List<Predicate>> getSocialRecordCopy()
        {
            return socialRecord.getSocialRecordCopy();
        }

        public int getCurrentTimeStep()
        {
            return socialRecord.getCurrentTimeStep();
        }

        public void setActionById(string label, Action newAction)
        {
            actionLibrary.setActionById(label, newAction);
        }

        //public facing function to make two characters perform an action.
        //TODO: doActon doesn't seem to exist anymore?
        //In theory this is a means to just run an action... though it seems as if the corresponding function in ActionLibrary.js hasn't actually been written? Very odd.
        public void doAction(string actionName, string initiator, string responder, List<Predicate> registeredVolitions)
        {
            // actionLibrary.doAction(actionName, initiator, responder, registeredVolitions);
        }

        public void setSocialRecordById(string ID, Predicate newRecord)
        {
            socialRecord.setById(ID, newRecord);
        }

        public void clearSocialStructure()
        {
            socialStructure = new SocialStructure();
            socialRecord.clearEverything();
        }

        public void reset()
        {
            socialStructure = new SocialStructure();
            socialRecord.clearEverything();
            ruleLibrary.clearRuleLibrary();
        }

        public bool init()
        {
            socialRecord.init(null);
            return true;
        }
    }
}
