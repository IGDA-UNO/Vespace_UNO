using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensemble
{
    public class Rule
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string Origin { get; set; }
        public bool IsActive { get; set; }
        public string Msg { get; set; }

        public List<Condition> Conditions { get; set; }
        public List<Effect> Effects { get; set; }

        public Rule ShallowCopy()
        {
            return (Rule)this.MemberwiseClone();
        }

        public Rule DeepCopy()
        {
            Rule deepCopy = (Rule)this.MemberwiseClone();

            if (Name != null)
            {
                deepCopy.Name = String.Copy(Name);
            }

            if (ID != null)
            {
                deepCopy.ID = String.Copy(ID);
            }

            if (Origin != null)
            {
                deepCopy.Origin = String.Copy(Origin);
            }

            if (Msg != null)
            {
                deepCopy.Msg = String.Copy(Msg);
            }

            if (Conditions != null)
            {
                deepCopy.Conditions = Conditions.Select(condition => (Condition)condition.DeepCopy()).ToList();
            }

            if (Effects != null)
            {
                deepCopy.Effects = Effects.Select(effect => (Effect)effect.DeepCopy()).ToList(); ;
            }

            return deepCopy;
        }

        public override string ToString()
        {
            string str = "name: " + this.Name + "\n";

            foreach(Condition condition in this.Conditions)
            {
                str += "condition: " + condition.ToString() + "\n";
            }

            foreach (Effect effect in this.Effects)
            {
                str += "effect: " + effect.ToString() + "\n";
            }

            return str;
        }
    }
}
