using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensemble
{
    internal class ActionLibrary
    {
        private List<Action> actions;
        private List<Action> startSymbols;
        private List<Action> nonTerminals;
        private List<Action> terminalActions;
        private Validate validate;
        private RuleLibrary ruleLibrary;
        private VolitionCache volitionCache;
        private Util util;

        public ActionLibrary(RuleLibrary ruleLibrary, VolitionCache volitionCache, Util util)
        {
            this.ruleLibrary = ruleLibrary;
            this.volitionCache = volitionCache;
            this.util = util;

            this.actions = new List<Action>();
            this.startSymbols = new List<Action>();
            this.nonTerminals = new List<Action>();
            this.terminalActions = new List<Action>();
            this.validate = new Validate();
        }

        public List<Action> getAllActions()
        {
            return actions;
        }

        public void dumpActions()
        {
            // log out actions
        }

        public List<Action> getStartSymbols()
        {
            return startSymbols;
        }

        public List<Action> getNonTerminals()
        {
            return nonTerminals;
        }

        public List<Action> getTerminalActions()
        {
            return terminalActions;
        }

        public void clearActionLibrary()
        {
            this.actions = new List<Action>();
            this.startSymbols = new List<Action>();
            this.nonTerminals = new List<Action>();
            this.terminalActions = new List<Action>();
        }

        public List<Action> parseActions(List<Action> inputActions, string fileName, string sourceFile)
        {
            List<Action> actionsToCategorize = new List<Action>();

            for (int i = 0; i < inputActions.Count; i++)
            {
                Action action = inputActions[i];
                action.FileName = fileName;
                action.ID = util.iterator("actions").ToString();
                action.Origin = sourceFile;
                action.IsActive = true;

                validate.action(action, "Validate for ActionLibrary parseActions");

                if (actionAlreadyExists(action))
                {
                    continue;
                }

                actions.Add(action.DeepCopy());
                actionsToCategorize.Add(action);
            }
            categorizeActionGrammar(actionsToCategorize);
            return actions;
        }

        public bool actionAlreadyExists(Action potentialNewAction)
        {
            for (int i = 0; i < actions.Count; i += 1)
            {
                if (actions[i].Name == potentialNewAction.Name)
                {
                    return true;
                }
            }
            return false;
        }

        public bool intentsAreSame(Intent one, Intent two)
        {
            return one.Category == two.Category
                && one.Type == two.Type
                && one.IntentType == two.IntentType
                && one.First == two.First
                && one.Second == two.Second;
        }

        public bool startSymbolAlreadyExists(Action potentialNewAction)
        {
            if (potentialNewAction.Intent != null)
            {
                Intent newStartSymbolIntent = potentialNewAction.Intent;

                for (int i = 0; i < startSymbols.Count; i++)
                {
                    Intent existingStartSymbolIntent = startSymbols[i].Intent;
                    if (intentsAreSame(newStartSymbolIntent, existingStartSymbolIntent))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void categorizeActionGrammar(List<Action> actionPool)
        {
            Action currentAction;

            for (int i = 0; i < actionPool.Count; i++)
            {
                currentAction = actionPool[i].DeepCopy();
                if (actionPool[i].Intent != null)
                {
                    if (startSymbolAlreadyExists(currentAction))
                    {
                        // log error
                    }
                    else
                    {
                        startSymbols.Add(currentAction);
                    }
                }
                if (actionPool[i].LeadsTo != null)
                {
                    nonTerminals.Add(currentAction);
                }

                if (actionPool[i].Effects != null)
                {
                    terminalActions.Add(currentAction);
                }
            }
        }

        public List<Action> getSortedActionsFromVolition(String initiator, String responder, Predicate registeredVolition, bool isAccepted, int? weight, int numActionsPerGroup, Cast cast)
        {
            List<Action> actions = getActionHierarchyFromVolition(initiator, responder, registeredVolition, isAccepted, weight, numActionsPerGroup, cast);
            List<Action> sortedActions = sortActionsByVolitionScore(actions);
            return sortedActions;
        }

        public List<Action> sortActionsByVolitionScore(List<Action> actions)
        {
            List<Action> descSortedActions = actions.OrderBy(o => o.Weight).ToList();
            descSortedActions.Reverse();

            for (int i = 0; i < actions.Count; i++)
            {
                List<Action> nextActions = actions[i].Actions;
                if (nextActions != null)
                {
                    actions[i].Actions = sortActionsByVolitionScore(nextActions);
                }

                if (actions[i].GoodBindings != null)
                {
                    actions[i].GoodBindings = actions[i].GoodBindings.OrderBy(o => o._Weight).ToList();
                    actions[i].GoodBindings.Reverse();
                }
            }

            return descSortedActions;
        }

        public bool actionIntentPertainsToVolition(Intent actionIntent, Predicate volition)
        {
            //return actionIntent.Category == volition.Category
            //    && actionIntent.Type == volition.Type
            //    && actionIntent.IntentType == volition.IntentType;
            return actionIntent.Category.ToLower() == volition.Category.ToLower()
                && actionIntent.Type.ToLower() == volition.Type.ToLower();
        }

        public List<Action> getActionHierarchyFromVolition(string initiator, string responder, Predicate volition, bool isAccepted, int? weight, int numActionsPerGroup, Cast cast)
        {
            Intent actionIntent;
            List<Action> goodTerminals;
            List<Action> returnTerminalList = new List<Action>();
            Binding uniqueBindings = new Binding();
            uniqueBindings["initiator"] = initiator;
            uniqueBindings["responder"] = responder;

            for (int i = 0; i < startSymbols.Count; i++)
            {
                actionIntent = startSymbols[i].Intent;
                if (actionIntentPertainsToVolition(actionIntent, volition))
                {
                    Action rootAction = startSymbols[i].DeepCopy();
                    rootAction.GoodBindings = new List<Binding>();
                    rootAction.GoodBindings.Add(uniqueBindings);
                    rootAction.Weight = weight;

                    goodTerminals = getActionHierarchyFromNonTerminal(rootAction, isAccepted, numActionsPerGroup, uniqueBindings, cast);

                    if (goodTerminals == null)
                    {
                        return null;
                    }

                    returnTerminalList = goodTerminals;
                    break;
                }
            }

            return returnTerminalList;
        }

        // TODO: Action terminalActionParentObject does not seem to be used...

        public TerminalSearchResults terminalFoundInRecursiveSearch(Action terminalAction, Action nonTerminal, Binding uniqueBindings, Cast cast, bool isAccepted, Action terminalActionParentObject)
        {
            terminalAction.GoodBindings = nonTerminal.GoodBindings.Select(binding => (Binding)binding.DeepCopy()).ToList();

            if (nonTerminal.Lineage == null)
            {
                terminalAction.Lineage = nonTerminal.Name;
            }
            else
            {
                terminalAction.Lineage = nonTerminal.Lineage + "-" + nonTerminal.Name;
            }

            Binding currentUniqueBindings = getUniqueActionBindings(terminalAction, uniqueBindings);
            List<Binding> goodBindings = nonTerminal.GoodBindings.Select(binding => (Binding)binding.DeepCopy()).ToList();
            List<Binding> workingBindingCombinations = getWorkingBindingCombinations(terminalAction, currentUniqueBindings.DeepCopy(), cast.DeepCopy(), goodBindings, cast.DeepCopy());

            terminalAction.GoodBindings = workingBindingCombinations;

            if (!actionIsAppropriate(terminalAction, isAccepted, currentUniqueBindings))
            {
                TerminalSearchResults inappropriateActionResults = new TerminalSearchResults();
                inappropriateActionResults.TerminalsAtThisLevel = true;
                inappropriateActionResults.BoundTerminal = null;
                return inappropriateActionResults;
            }

            // TODO: Figure out why the terminalAction weight is showing up as null

            if (terminalAction.Salience == null)
            {
                terminalAction.Salience = 0;
            }

            if (terminalAction.Weight == null)
            {
                terminalAction.Weight = 0;
            }

            computeInfluenceRuleWeight(terminalAction);

            if (terminalAction.Weight < 0)
            {
                TerminalSearchResults negativeWeightResults = new TerminalSearchResults();
                negativeWeightResults.TerminalsAtThisLevel = true;
                negativeWeightResults.BoundTerminal = null;
                return negativeWeightResults;
            }

            terminalAction.Salience = terminalAction.Weight + terminalAction.Salience;

            TerminalSearchResults returnObject = new TerminalSearchResults();
            returnObject.TerminalsAtThisLevel = true;
            returnObject.BoundTerminal = terminalAction;
            return returnObject;
        }

        public Action nonTerminalFoundInRecursiveSearch(String actionName, Action nonTerminal, Binding uniqueBindings, bool isAccepted, int actionsPerGroup, Cast cast)
        {
            Action nonTerminalAction = getActionFromNameInArray(actionName, nonTerminals);
            
            nonTerminalAction.GoodBindings = nonTerminal.GoodBindings;
            nonTerminalAction.Weight = nonTerminal.Weight;

            if (nonTerminal.Lineage == null)
            {
                nonTerminalAction.Lineage = nonTerminal.Name;
            }
            else
            {
                nonTerminalAction.Lineage = nonTerminal.Lineage + "-" + nonTerminal.Name;
            }

            Binding currentUniqueBindings = getUniqueActionBindings(nonTerminalAction, uniqueBindings);
            if (!actionIsAppropriate(nonTerminalAction, isAccepted, currentUniqueBindings))
            {
                return null;
            }

            List<Action> diggingDeeperActions = getActionHierarchyFromNonTerminal(nonTerminalAction, isAccepted, actionsPerGroup, currentUniqueBindings.DeepCopy(), cast.DeepCopy());
            if (diggingDeeperActions == null || diggingDeeperActions.Count <= 0)
            {
                return null;
            }

            nonTerminalAction.Actions = new List<Action>();

            for (int ddActionIndex = 0; ddActionIndex < diggingDeeperActions.Count; ddActionIndex++)
            {
                Action thingToAdd = diggingDeeperActions[ddActionIndex];
                nonTerminalAction.Actions.Add(thingToAdd.DeepCopy());
            }

            return nonTerminalAction;
        }

        public List<Action> getActionHierarchyFromNonTerminal(Action nonTerminal, bool isAccepted, int actionsPerGroup, Binding uniqueBindings, Cast cast)
        {
            List<Action> returnList = new List<Action>();
            bool terminalsAtThisLevel = false;

            if (nonTerminal.LeadsTo == null)
            {
                return null;
            }

            Binding currentUniqueBindings = getUniqueActionBindings(nonTerminal, uniqueBindings);
            List<Binding> goodBindings = nonTerminal.GoodBindings.Select(binding => (Binding)binding.DeepCopy()).ToList();
            List<Binding> nonTerminalWorkingBindingCombinations = getWorkingBindingCombinations(nonTerminal, uniqueBindings.DeepCopy(), cast.DeepCopy(), goodBindings, cast);

            if (nonTerminalWorkingBindingCombinations.Count <= 0)
            {
                return null;
            }

            nonTerminal.GoodBindings = nonTerminalWorkingBindingCombinations;

            computeInfluenceRuleWeight(nonTerminal);

            Action terminalActionParentObject = new Action();
            terminalActionParentObject.Name = nonTerminal.Name;
            terminalActionParentObject.Weight = nonTerminal.Weight;
            terminalActionParentObject.GoodBindings = nonTerminal.GoodBindings;
            terminalActionParentObject.Actions = new List<Action>();

            for (int i = 0; i < nonTerminal.LeadsTo.Count; i++)
            {
                string actionName = nonTerminal.LeadsTo[i];
                Action terminalAction = getActionFromNameInArray(actionName, terminalActions);

                if (terminalAction != null)
                {
                    TerminalSearchResults response = terminalFoundInRecursiveSearch(terminalAction, nonTerminal, uniqueBindings, cast, isAccepted, terminalActionParentObject);
                    terminalsAtThisLevel = response.TerminalsAtThisLevel;
                    Action foundTerminal = response.BoundTerminal;

                    if (foundTerminal != null)
                    {
                        terminalActionParentObject.Actions.Add(foundTerminal);

                        // Reverse() function modifies the array
                        List<Action> sortedActionsBySalience = terminalActionParentObject.Actions.OrderBy(o => o.Salience).ToList();
                        sortedActionsBySalience.Reverse();
                        terminalActionParentObject.Actions = sortedActionsBySalience;

                        if (terminalActionParentObject.Actions.Count > actionsPerGroup)
                        {
                            terminalActionParentObject.Actions.RemoveRange(actionsPerGroup, terminalActionParentObject.Actions.Count - actionsPerGroup);
                        }
                    }
                }
                else
                {
                    Action nonTerminalWithNewRoles = nonTerminalFoundInRecursiveSearch(actionName, nonTerminal, uniqueBindings, isAccepted, actionsPerGroup, cast);
                    if (nonTerminalWithNewRoles != null)
                    {
                        returnList.Add(nonTerminalWithNewRoles);
                    }
                }
            }

            if (terminalsAtThisLevel)
            {
                for (int terminalsToPushUpIndex = 0; terminalsToPushUpIndex < terminalActionParentObject.Actions.Count; terminalsToPushUpIndex++)
                {
                    returnList.Add(terminalActionParentObject.Actions[terminalsToPushUpIndex].DeepCopy());
                }
            }

            return returnList;
        }

        // TODO: method is unused... decide if it's still needed

        // var computeActionSalience = function(terminalAction){
        // 	var returnValue;
        // 	var multiplier = 5; // Maybe this should live in some constants thing? Or, better yet, be something that the user can specify?
        // 	if(terminalAction.salience !== undefined){
        // 		returnValue = terminalAction.salience;
        // 	}
        // 	else{
        // 		var numConditions = terminalAction.conditions.length;
        // 		if(numConditions === undefined){
        // 			numConditions = 0;
        // 		}
        // 		var salienceCalculation = numConditions * multiplier;
        // 		returnValue = salienceCalculation;
        // 	}
        // 	return returnValue;
        // };

        public void computeInfluenceRuleWeight(Action action)
        {
            int? bestWeightFoundSoFar = -999999;
            for (int goodBindingIndex = 0; goodBindingIndex < action.GoodBindings.Count; goodBindingIndex++)
            {
                Binding tempGoodBindings = action.GoodBindings[goodBindingIndex];
                int? oldWeight = tempGoodBindings._Weight;

                // Debug.WriteLine("action.Weight: " + action.Weight);
                
                if (oldWeight == null)
                {
                    oldWeight = action.Weight;
                }

                // Debug.WriteLine("oldWeight: " + oldWeight);

                int? scoreFromInfluenceRules = evaluateActionInfluenceRules(action, tempGoodBindings);
                int? candidateWeight = oldWeight + scoreFromInfluenceRules;

                if (candidateWeight > bestWeightFoundSoFar)
                {
                    action.Weight = candidateWeight;
                    bestWeightFoundSoFar = candidateWeight;
                }

                // Debug.WriteLine("_Weight before: " + action.GoodBindings[goodBindingIndex]._Weight);
                action.GoodBindings[goodBindingIndex]._Weight = candidateWeight;
                // Debug.WriteLine("_Weight after: " + action.GoodBindings[goodBindingIndex]._Weight);
            }
        }

        public bool actionIsAppropriate(Action action, bool isAccepted, Binding uniqueBindings)
        {
            if (action.IsActive == false)
            {
                return false;
            }

            if (action.IsAccept != null)
            {
                if (isAccepted != action.IsAccept)
                {
                    return false;
                }
            }

            if (action.IsAccept == null && action.LeadsTo == null && action.Effects != null)
            {
                if (isAccepted == false)
                {
                    return false;
                }
            }

            if (action.GoodBindings.Count <= 0)
            {
                return false;
            }

            return true;
        }

        public Action getActionFromNameInArray(string actionName, List<Action> actionArray)
        {
            for (int i = 0; i < actionArray.Count; i++)
            {
                if (actionArray[i].Name == actionName)
                {
                    return actionArray[i].DeepCopy();
                }
            }
            return null;
        }

        public Action getActionFromName(string actionName)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].Name == actionName)
                {
                    return actions[i].DeepCopy();
                }
            }
            return null;
        }

        public Action bindActionEffects(Action actionObject, Binding bindingsToUse)
        {
            for (int i = 0; i < actionObject.Effects.Count; i++)
            {
                if (actionObject.Effects[i].First != null)
                {
                    actionObject.Effects[i].First = bindingsToUse[actionObject.Effects[i].First];
                }

                if (actionObject.Effects[i].Second != null)
                {
                    actionObject.Effects[i].Second = bindingsToUse[actionObject.Effects[i].Second];
                }
            }
            return actionObject;
        }

        public Binding getUniqueActionBindings(Action actionObject, Binding uniqueBindings)
        {
            List<Condition> conditions = actionObject.Conditions;

            for (int i = 0; i < conditions.Count; i++)
            {
                Condition predicate = conditions[i];

                if (!uniqueBindings.ContainsKey(predicate.First) || uniqueBindings[predicate.First] == null)
                {
                    uniqueBindings[predicate.First] = "";
                }

                if (predicate.Second != null)
                {
                    if (!uniqueBindings.ContainsKey(predicate.Second) || uniqueBindings[predicate.Second] == null)
                    {
                        uniqueBindings[predicate.Second] = "";
                    }
                }
            }

            return uniqueBindings;
        }

        public List<Binding> getWorkingBindingCombinations(Action action, Binding uniqueBindings, Cast availableCastMembers, List<Binding> combinationsToUse, Cast allCastMembers)
        {
            // Debug.WriteLine("action.Name: " + action.Name);

            List<Binding> returnArray = new List<Binding>();
            List<Binding> newCombinationsToUse = new List<Binding>();

            for (int workingCombinationIndex = 0; workingCombinationIndex < combinationsToUse.Count; workingCombinationIndex++)
            {
                newCombinationsToUse = new List<Binding>();
                newCombinationsToUse.Add(combinationsToUse[workingCombinationIndex].DeepCopy());
                availableCastMembers = allCastMembers.DeepCopy();

                foreach (KeyValuePair<string, string> binding in combinationsToUse[workingCombinationIndex])
                {
                    string characterName = combinationsToUse[workingCombinationIndex][binding.Key];
                    if (characterName != "")
                    {
                        if (availableCastMembers != null)
                        {
                            int castIndex = availableCastMembers.IndexOf(characterName);
                            if (castIndex >= 0)
                            {
                                availableCastMembers.RemoveAt(castIndex);
                            }
                        }
                    }
                }

                bool isFilled = true;
                string emptyKey = "";

                foreach (KeyValuePair<string, string> binding in uniqueBindings)
                {
                    if (uniqueBindings[binding.Key] == "")
                    {
                        emptyKey = binding.Key;
                        isFilled = false;
                        break;
                    }
                }

                if (isFilled == true)
                {
                    List<Predicate> conditionsAsPredicates = action.Conditions.Cast<Predicate>().ToList().Select(condition => (Predicate)condition.DeepCopy()).ToList();
                    List<Condition> boundConditions = ruleLibrary.doBinding(uniqueBindings, conditionsAsPredicates).Cast<Condition>().ToList();
                    bool evaluationResult = ruleLibrary.evaluateConditions(boundConditions, null);

                    if (evaluationResult == true)
                    {
                        returnArray.Add(uniqueBindings.DeepCopy());
                    }
                    else
                    {
                        // log failure
                    }
                }
                else
                {
                    for (int i = 0; i < availableCastMembers.Count; i++)
                    {
                        uniqueBindings[emptyKey] = availableCastMembers[i];
                        Cast updatedCastMembers = availableCastMembers.DeepCopy();
                        updatedCastMembers.RemoveAt(i);
                        List<Binding> potentialCombinations = getWorkingBindingCombinations(action, uniqueBindings, updatedCastMembers, newCombinationsToUse, allCastMembers);


                        for (int k = 0; k < potentialCombinations.Count; k++)
                        {
                            returnArray.Add(potentialCombinations[k].DeepCopy());
                        }
                    }

                    uniqueBindings[emptyKey] = "";
                }
            }

            //foreach (Binding b in returnArray)
            //{
            //    Debug.WriteLine("returnArray binding: " + b.ToString());
            //}

            //Debug.WriteLine("");

            return returnArray;
        }

        public List<Condition> bindActionCondition(List<Condition> conditions, Binding bindingToUse)
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                if (conditions[i].First != null)
                {
                    conditions[i].First = bindingToUse[conditions[i].First];
                }
                if (conditions[i].Second != null)
                {
                    conditions[i].Second = bindingToUse[conditions[i].Second];
                }
            }
            return conditions;
        }

        public int evaluateActionInfluenceRules(Action action, Binding bindingToUse)
        {
            int volitionSum = 0;

            for (int i = 0; i < action.InfluenceRules.Count; i++)
            {
                InfluenceRule rule = action.InfluenceRules[i];
                List<Condition> boundConditions = bindActionCondition(rule.Conditions.Select(condition => (Condition)condition.DeepCopy()).ToList(), bindingToUse);
                bool isRuleTrue = ruleLibrary.evaluateConditions(boundConditions, null);
                if (isRuleTrue == true)
                {
                    volitionSum += rule.Weight;
                }
            }

            return volitionSum;
        }

        public Action getBestTerminalFromActionList(List<Action> actionList)
        {
            if (actionList.Count <= 0)
            {
                return null;
            }

            if (actionList[0].Actions != null && actionList[0].Actions.Count > 0)
            {
                return getBestTerminalFromActionList(actionList[0].Actions);
            }
            else
            {
                Action terminal = actionList[0];
                List<Binding> potentialBestBindings = new List<Binding>();

                for (int i = 0; i < terminal.GoodBindings.Count; i++)
                {
                    if (terminal.GoodBindings[i]._Weight == terminal.Weight)
                    {
                        potentialBestBindings.Add(terminal.GoodBindings[i]);
                    }
                }

                Random rnd = new Random();
                int goodBindingIndex = rnd.Next(0, potentialBestBindings.Count - 1);
                Binding bindingsToUse = potentialBestBindings[goodBindingIndex];
                return bindActionEffects(terminal, bindingsToUse);
            }
        }

        public Action getAction(string initiator, string responder, VolitionInterface volitionInterface, Cast cast, int? numActionsPerGroup)
        {
            if(numActionsPerGroup == null){
                numActionsPerGroup = 1;
            }

            List<Action> actionList = new List<Action>();
            Predicate volitionInstance = volitionInterface.getFirst(initiator, responder);

            while (actionList.Count == 0 && volitionInstance != null)
            {
                VolitionAcceptance acceptedObject = volitionCache.isAccepted("main", initiator, responder, volitionInstance);
                bool isAccepted = acceptedObject.Accepted;
                int? weight = volitionInstance.Weight;
                actionList = getSortedActionsFromVolition(initiator, responder, volitionInstance, isAccepted, weight, (int)numActionsPerGroup, cast);
                volitionInstance = volitionCache.getNextVolition("main", initiator, responder);
            }

            Action boundAction = getBestTerminalFromActionList(actionList);
            return boundAction;
        }

        public List<Action> getActions(string initiator, string responder, VolitionInterface volitionInterface, Cast cast, int numIntents, int numActionsPerIntent, int? numActionsPerGroup)
        {
            if(numActionsPerGroup == null) numActionsPerGroup = 1;

            List<Action> returnList = new List<Action>();
            List<Action> actionList = new List<Action>();
            Predicate volitionInstance = volitionInterface.getFirst(initiator, responder);

            if (volitionInstance == null)
            {
                return returnList;
            }

            int intentsRepresented = 0;
            int numActionsFromThisIntent = 0;
            bool thisIntentCountedYet = false;

            while (intentsRepresented < numIntents)
            {
                thisIntentCountedYet = false;
                numActionsFromThisIntent = 0;

                VolitionAcceptance acceptedObject = volitionCache.isAccepted("main", initiator, responder, volitionInstance);
                bool isAccepted = acceptedObject.Accepted;
                int? weight = volitionInstance.Weight;

                actionList = getSortedActionsFromVolition(initiator, responder, volitionInstance, isAccepted, weight, (int)numActionsPerGroup, cast);

                for (int i = 0; i < actionList.Count; i++)
                {
                    returnList.Add(actionList[i].DeepCopy());
                    if (thisIntentCountedYet == false)
                    {
                        intentsRepresented++;
                        thisIntentCountedYet = true;
                    }

                    numActionsFromThisIntent++;
                    if (numActionsFromThisIntent == numActionsPerIntent)
                    {
                        break;
                    }
                }

                volitionInstance = volitionCache.getNextVolition("main", initiator, responder);

                if (volitionInstance == null)
                {
                    break;
                }
            }

            List<Action> allTerminals = grabAllTerminals(returnList);
            List<Action> boundActions = sortAndBindTerminals(allTerminals);
            return boundActions;
        }

        public bool setActionById(string id, Action newAction)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                Action action = actions[i];
                if (action.ID == id)
                {
                    actions[i] = newAction;
                    return true;
                }
            }
            return false;
        }

        // TODO: Neither of the following methods are used... are they still needed?

        //for each action in the action list, go through and find how many total terminal actions we have.
        // var getNumberOfTerminalsReachablebyAnActionList = function(actionList){	
        // 	var sum = 0;
        // 	for(var i = 0; i < actionList.length; i += 1){
        // 		sum += getNumberOfTerminalsReachablebyAnAction(actionList[i]);
        // 	}
        // 	return sum;
        // }

        //public int getNumberOfTerminalsReachablebyAnAction(Action action)
        //{
        //    int sum = 0;
        //
        //    if (action.LeadsTo == null)
        //    {
        //        return 1;
        //    }
        //    else
        //    {
        //        for (int i = 0; i < action.LeadsTo.Count; i++)
        //        {
        //            sum += getNumberOfTerminalsReachablebyAnAction(action.LeadsTo[i]);
        //        }
        //        return sum;
        //    }
        //}

        public List<Action> grabAllTerminals(List<Action> actionList)
        {
            List<Action> terminalsFoundHere = new List<Action>();
            List<Action> terminalsFoundDeeper = new List<Action>();
            List<Action> deeperTerminalRecord = new List<Action>();

            for (int i = 0; i < actionList.Count; i++)
            {
                if (actionList[i].Actions != null)
                {
                    terminalsFoundDeeper = grabAllTerminals(actionList[i].Actions);
                    if (terminalsFoundDeeper != null)
                    {
                        if (deeperTerminalRecord == null)
                        {
                            deeperTerminalRecord = terminalsFoundDeeper;
                        }
                        else
                        {
                            deeperTerminalRecord = deeperTerminalRecord.Concat(terminalsFoundDeeper).ToList();
                        }
                        terminalsFoundDeeper = new List<Action>();
                    }
                }
                else
                {
                    terminalsFoundHere.Add(actionList[i]);
                }
            }

            List<Action> allTerminals = new List<Action>();

            if (deeperTerminalRecord != null)
            {
                allTerminals = terminalsFoundHere.Concat(deeperTerminalRecord).ToList();
            }
            else
            {
                allTerminals = terminalsFoundHere;
            }

            return allTerminals;
        }

        public List<Action> sortAndBindTerminals(List<Action> terminalArray)
        {
            List<Action> sortedTerminals = sortActionsByVolitionScore(terminalArray);
            for (int k = 0; k < sortedTerminals.Count; k++)
            {
                Binding bestBindings = getBestBindingFromTerminal(sortedTerminals[k]);
                sortedTerminals[k] = bindActionEffects(sortedTerminals[k], bestBindings);
            }
            return sortedTerminals;
        }

        public Binding getBestBindingFromTerminal(Action terminal)
        {
            Debug.WriteLine("getBestBindingFromTerminal terminal: " + terminal.Name);
            Debug.WriteLine("getBestBindingFromTerminal terminal.Weight: " + terminal.Weight);
            foreach (Binding b in terminal.GoodBindings)
            {
                Debug.WriteLine("terminal GoodBinding : " + b.ToString());
                Debug.WriteLine("terminal GoodBinding Weight: " + b._Weight.ToString());
            }

            List<Binding> potentialBestBindings = new List<Binding>();

            for (int i = 0; i < terminal.GoodBindings.Count; i++)
            {
                if (terminal.GoodBindings[i]._Weight == terminal.Weight)
                {
                    potentialBestBindings.Add(terminal.GoodBindings[i]);
                }
            }

            Random rnd = new Random();
            int goodBindingIndex = rnd.Next(0, potentialBestBindings.Count - 1);
            Binding bindingsToUse = potentialBestBindings[goodBindingIndex];
            return bindingsToUse;
        }

    }
}
