using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensemble
{
    public class Action
    {
        public bool? IsAccept { get; set; }
        public bool IsActive { get; set; }

        public Condition Condition { get; set; }

        public int? Salience { get; set; }
        public int? Weight { get; set; }

        public Intent Intent { get; set; }

        public List<Action> Actions { get; set; }
        public List<Binding> GoodBindings { get; set; }
        public List<Condition> Conditions { get; set; }
        public List<Effect> AcceptEffects { get; set; }
        public List<Effect> Effects { get; set; }
        public List<Effect> RejectEffects { get; set; }
        public List<InfluenceRule> InfluenceRules { get; set; }
        public List<List<Performance>> Performance { get; set; }

        public List<string> LeadsTo { get; set; }

        public string DisplayName { get; set; }
        public string FileName { get; set; }
        public string ID { get; set; }
        public string Lineage { get; set; }
        public string Name { get; set; }
        public string Origin { get; set; }
        
        public Action() { }

        public Action(string name, Intent intent, List<Condition> conditions, List<InfluenceRule> influenceRules, List<string> leadsTo) {
            this.Name = name;
            this.Intent = intent;
            this.Conditions = conditions;
            this.InfluenceRules = influenceRules;
            this.LeadsTo = leadsTo;
        }

        public Action ShallowCopy()
        {
            return (Action)this.MemberwiseClone();
        }

        public Action DeepCopy()
        {
            Action deepCopy = (Action)this.MemberwiseClone();

            if (Name != null)
            {
                deepCopy.Name = String.Copy(Name);
            }

            if (DisplayName != null)
            {
                deepCopy.DisplayName = String.Copy(DisplayName);
            }

            if (FileName != null)
            {
                deepCopy.FileName = String.Copy(FileName);
            }

            if (Origin != null)
            {
                deepCopy.Origin = String.Copy(Origin);
            }

            if (Lineage != null)
            {
                deepCopy.Lineage = String.Copy(Lineage);
            }

            if (ID != null)
            {
                deepCopy.ID = String.Copy(ID);
            }

            if (Conditions != null)
            {
                deepCopy.Conditions = Conditions.Select(condition => (Condition)condition.DeepCopy()).ToList();
            }

            if (Effects != null)
            {
                deepCopy.Effects = Effects.Select(effect => (Effect)effect.DeepCopy()).ToList();
            }

            if (LeadsTo != null)
            {
                deepCopy.LeadsTo = LeadsTo.Select(leadsTo => (string)leadsTo.Clone()).ToList();
            }

            if (InfluenceRules != null)
            {
                deepCopy.InfluenceRules = InfluenceRules.Select(rule => (InfluenceRule)rule.DeepCopy()).ToList();
            }

            if (GoodBindings != null)
            {
                deepCopy.GoodBindings = GoodBindings.Select(binding => (Binding)binding.DeepCopy()).ToList();
            }

            if (Actions != null)
            {
                deepCopy.Actions = Actions.Select(action => (Action)action.DeepCopy()).ToList();
            }

            return deepCopy;
        }

        public override string ToString()
        {
            string str = "name: " + this.Name + "\n";

            if (this.Salience != null) { str += "salience: " + this.Salience + "\n"; }
            if (this.Weight != null) { str += "weight: " + this.Weight + "\n"; }

            if (this.Conditions != null)
            {
                foreach (Condition condition in this.Conditions)
                {
                    str += "condition: " + condition.ToString() + "\n";
                }
            }

            if (this.Effects != null)
            {
                foreach (Effect effect in this.Effects)
                {
                    str += "effect: " + effect.ToString() + "\n";
                }
            }

            if (this.GoodBindings != null)
            {
                foreach (Binding b in this.GoodBindings)
                {
                    str += "good binding: " + b.ToString() + "\n";
                }
            }

            return str;
        }
    }
}
