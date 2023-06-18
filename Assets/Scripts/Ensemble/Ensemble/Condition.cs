using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensemble
{
    public class Condition : Predicate
    {
        public int? Order { get; set; }

        public Condition() { }
        public Condition(string category, string type) : base(category, type) {}
        public Condition(string category, string type, string first) : base(category, type, first) { }
        public Condition(string category, string type, string first, int value) : base(category, type, first, value) { }
        public Condition(string category, string type, string first, bool value) : base(category, type, first, value) { }
        public Condition(string category, string type, string first, List<string> turnsAgoBetween) : base(category, type, first, turnsAgoBetween) { }
        public Condition(string category, string type, string first, bool value, List<string> turnsAgoBetween) : base(category, type, first, value, turnsAgoBetween) { }
        public Condition(string category, string type, string first, string second) : base(category, type, first, second) { }
        public Condition(string category, string type, string first, string second, int value) : base(category, type, first, second, value) { }
        public Condition(string category, string type, string first, string second, bool value) : base(category, type, first, second, value) { }
        public Condition(string category, string type, string first, string second, int value, bool intentType) : base(category, type, first, second, value, intentType) { }
        public Condition(string category, string type, string first, string second, bool value, List<string> turnsAgoBetween) : base(category, type, first, second, value, turnsAgoBetween) { }
        public Condition(string category, string type, string first, string second, List<string> turnsAgoBetween, int order) : base(category, type, first, second, turnsAgoBetween, order) { }
        public Condition(string category, string type, string first, string second, string operation, int value, List<string> turnsAgoBetween, int order) : base(category, type, first, second, operation, value, turnsAgoBetween, order) { }
        public Condition(string category, string type, string first, string second, int value, string operation) : base(category, type, first, second, value, operation) { }
        public Condition(string category, string type, string first, string second, string operation, int value, int order, List<string> turnsAgoBetween) : base(category, type, first, second, operation, value, turnsAgoBetween, order) { }

        new public Condition DeepCopy()
        {
            return (Condition)_DeepCopy();
        }
    }
}
