using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensemble
{
    public class InfluenceRule
    {
        public string Name { get; set; }
        public List<Condition> Conditions { get; set; }
        public int Weight { get; set; }

        public InfluenceRule ShallowCopy()
        {
            return (InfluenceRule)this.MemberwiseClone();
        }

        public InfluenceRule DeepCopy()
        {
            InfluenceRule deepCopy = (InfluenceRule)this.MemberwiseClone();

            deepCopy.Name = String.Copy(Name);
            deepCopy.Conditions = Conditions.Select(condition => (Condition)condition.DeepCopy()).ToList();

            return deepCopy;
        }
    }
}
