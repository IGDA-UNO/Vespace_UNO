using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensemble
{
    public class Effect : Predicate
    {
        public Effect() { }
        public Effect(string category, string type) : base(category, type) { }
        public Effect(string category, string type, string first) : base(category, type, first) { }
        public Effect(string category, string type, string first, int value) : base(category, type, first, value) { }
        public Effect(string category, string type, string first, bool value) : base(category, type, first, value) { }
        public Effect(string category, string type, string first, bool value, List<string> turnsAgoBetween) : base(category, type, first, value, turnsAgoBetween) { }
        public Effect(string category, string type, string first, string second) : base(category, type, first, second) { }
        public Effect(string category, string type, string first, string second, int value) : base(category, type, first, second, value) { }
        public Effect(string category, string type, string first, string second, bool value) : base(category, type, first, second, value) { }
        public Effect(string category, string type, string first, string second, int value, bool intentType) : base(category, type, first, second, value, intentType) { }
        public Effect(string category, string type, string first, string second, List<string> turnsAgoBetween, int order) : base(category, type, first, second, turnsAgoBetween, order) { }
        public Effect(string category, string type, string first, string second, string operation, int value, List<string> turnsAgoBetween, int order) : base(category, type, first, second, operation, value, turnsAgoBetween, order) { }
        public Effect(string category, string type, string first, string second, bool value, List<string> turnsAgoBetween) : base(category, type, first, second, value, turnsAgoBetween) { }
        public Effect(string category, string type, string first, string second, int value, string operation) : base(category, type, first, second, value, operation) { }

        new public Effect DeepCopy()
        {
            return (Effect)_DeepCopy();
        }
    }
}
