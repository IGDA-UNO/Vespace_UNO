using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * This is the class Validate, for verification of predicates and other data.
 *
 */

namespace Ensemble
{
    internal class Validate
    {
        string[] allowedDirTypes = { "directed", "undirected", "reciprocal" };
        string[] allowedOpsConditions = { ">", "<", "=" };
        string[] allowedOpsEffects = { "+", "-", "=" };
        string[] allowedTurnConstants = { "now", "start" };

        object socialStructure;

        public void registerSocialStructure(object ss)
        {
            socialStructure = ss;
        }

        public Predicate triggerCondition(Predicate pred)
        {
            checkPredicate(pred, "condition", "trigger", null);
            return pred;
        }

        public Predicate triggerCondition(Predicate pred, string preamble)
        {
            checkPredicate(pred, "condition", "trigger", preamble);
            return pred;
        }

        public Predicate triggerEffect(Predicate pred)
        {
            checkPredicate(pred, "effect", "trigger", null);
            return pred;
        }

        public Predicate triggerEffect(Predicate pred, string preamble)
        {
            checkPredicate(pred, "effect", "trigger", preamble);
            return pred;
        }

        public Predicate volitionCondition(Predicate pred, string preamble)
        {
            checkPredicate(pred, "condition", "volition", preamble);
            return pred;
        }

        public Predicate volitionEffect(Predicate pred, string preamble)
        {
            checkPredicate(pred, "effect", "volition", preamble);
            return pred;
        }

        public Predicate blueprint(Predicate pred, string preamble)
        {
            checkPredicate(pred, "blueprint", "", preamble);
            return pred;
        }

        public Rule rule(Rule rule)
        {
            bool isVolition = rule.Effects[0].Weight != null;

            try
            {
                foreach (Effect effect in rule.Effects)
                {
                    if (isVolition)
                    {
                        volitionEffect(effect, "Volition Rule Effect #");
                    } else
                    {
                        triggerEffect(effect, "Trigger Rule Effect #");
                    }
                }

                foreach(Condition condition in rule.Conditions)
                {
                    if (isVolition)
                    {
                        volitionCondition(condition, "Volition Rule Effect #");
                    }
                    else
                    {
                        triggerCondition(condition, "Trigger Rule Effect #");
                    }
                }
            } catch(Exception e)
            {
                throw e;
            }

            return rule;
        }

        public Action action(Action pred, string preamble)
        {
            // TODO: validate
            return pred;
        }

        public bool checkPredicate(Predicate pred, string type, string category, string preamble)
        {
            bool result = isPredBad(pred, type, category);
            if (result != false)
            {
                throw new Exception(preamble + " and found a malformed predicate: " + result + ".");
            }
            return result;
        }

        // TODO: Finish translating isPredBad
        public bool isPredBad(Predicate pred, string type, string category)
        {
            return false;
        }
    }
}
